using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading;

using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;
using Enyim.Collections;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Represents a Memcached node in the pool.
	/// </summary>
	[DebuggerDisplay("{{MemcachedNode [ Address: {EndPoint}, IsAlive = {IsAlive} ]}}")]
	public class MemcachedNode : IMemcachedNode
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(MemcachedNode));
		private static readonly object _syncRoot = new object();

		private bool _isDisposed;

		private readonly IPEndPoint _endPoint;
		private ISocketPoolConfiguration _config;
		private InternalPoolImpl _internalPoolImpl;
		private bool _isInitialized;

		/// <summary>
		///
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="socketPoolConfig"></param>
		public MemcachedNode(IPEndPoint endpoint, ISocketPoolConfiguration socketPoolConfig)
		{
			_endPoint = endpoint;
			_config = socketPoolConfig;

			if (socketPoolConfig.ConnectionTimeout.TotalMilliseconds >= int.MaxValue)
			{
				throw new InvalidOperationException("ConnectionTimeout must be < Int32.MaxValue");
			}

			_internalPoolImpl = new InternalPoolImpl(this, socketPoolConfig);
		}

		/// <summary></summary>
		public event Action<IMemcachedNode> Failed;
		private INodeFailurePolicy _failurePolicy;

		/// <summary>
		///
		/// </summary>
		protected INodeFailurePolicy FailurePolicy
		{
			get { return _failurePolicy ?? (_failurePolicy = _config.FailurePolicyFactory.Create(this)); }
		}

		/// <summary>
		/// Gets the <see cref="T:IPEndPoint"/> of this instance
		/// </summary>
		public IPEndPoint EndPoint
		{
			get { return _endPoint; }
		}

		/// <summary>
		/// <para>Gets a value indicating whether the server is working or not. Returns a <b>cached</b> state.</para>
		/// <para>To get real-time information and update the cached state, use the <see cref="M:Ping"/> method.</para>
		/// </summary>
		/// <remarks>Used by the <see cref="T:ServerPool"/> to quickly check if the server's state is valid.</remarks>
		public bool IsAlive
		{
			get { return _internalPoolImpl.IsAlive; }
		}

		/// <summary>
		/// Gets a value indicating whether the server is working or not.
		///
		/// If the server is back online, we'll ercreate the internal socket pool and mark the server as alive so operations can target it.
		/// </summary>
		/// <returns>true if the server is alive; false otherwise.</returns>
		public bool Ping()
		{
			// is the server working?
			if (_internalPoolImpl.IsAlive)
			{
				return true;
			}

			// this codepath is (should be) called very rarely
			// if you get here hundreds of times then you have bigger issues
			// and try to make the memcached instaces more stable and/or increase the deadTimeout
			try
			{
				// we could connect to the server, let's recreate the socket pool
				lock (_syncRoot)
				{
					if (_isDisposed)
					{
						return false;
					}

					// try to connect to the server
					using (var socket = CreateSocket()) { }

					if (_internalPoolImpl.IsAlive)
					{
						return true;
					}

					// it's easier to create a new pool than reinitializing a dead one
					// rewrite-then-dispose to avoid a race condition with Acquire (which does no locking)
					var oldPool = _internalPoolImpl;
					InternalPoolImpl newPool = new InternalPoolImpl(this, _config);

					Interlocked.Exchange(ref _internalPoolImpl, newPool);

					try { oldPool.Dispose(); }
					catch { }
				}

				return true;
			}
			//could not reconnect
			catch { return false; }
		}

		/// <summary>
		/// Acquires a new item from the pool
		/// </summary>
		/// <returns>An <see cref="T:PooledSocket"/> instance which is connected to the memcached server, or <value>null</value> if the pool is dead.</returns>
		public IPooledSocketResult Acquire()
		{
			if (!_isInitialized)
			{
				lock (_internalPoolImpl)
				{
					if (!_isInitialized)
					{
						_internalPoolImpl.InitPool();
						_isInitialized = true;
					}
				}
			}

			try
			{
				return _internalPoolImpl.Acquire();
			}
			catch (Exception e)
			{
				string message = "Acquire failed. Maybe we're already disposed?";
				_log.Error(message, e);
				PooledSocketResult result = new PooledSocketResult();
				result.Fail(message, e);
				return result;
			}
		}

		/// <summary>
		///
		/// </summary>
		~MemcachedNode()
		{
			try { ((IDisposable)this).Dispose(); }
			catch { }
		}

		/// <summary>
		/// Releases all resources allocated by this instance
		/// </summary>
		public void Dispose()
		{
			if (_isDisposed)
			{
				return;
			}

			GC.SuppressFinalize(this);

			// this is not a graceful shutdown
			// if someone uses a pooled item then it's 99% that an exception will be thrown
			// somewhere. But since the dispose is mostly used when everyone else is finished
			// this should not kill any kittens
			lock (_syncRoot)
			{
				if (_isDisposed)
				{
					return;
				}

				_isDisposed = true;
				_internalPoolImpl.Dispose();
			}
		}

		/// <summary>
		///
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose();
		}

		#region [ InternalPoolImpl             ]

		private class InternalPoolImpl : IDisposable
		{
			private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(InternalPoolImpl).FullName.Replace("+", "."));

			/// <summary>
			/// A list of already connected but free to use sockets
			/// </summary>
			private InterlockedStack<PooledSocket> _freeItems;

			private bool _isDisposed;
			private bool _isAlive;
			private DateTime _markedAsDeadUtc;

			private readonly int _minItems;
			private readonly int _maxItems;

			private MemcachedNode _ownerNode;
			private readonly IPEndPoint _endPoint;
			private readonly TimeSpan _queueTimeout;
			private Semaphore _semaphore;

			private readonly object _initLock = new object();

			internal InternalPoolImpl(MemcachedNode ownerNode, ISocketPoolConfiguration config)
			{
				if (config.MinPoolSize < 0)
				{
					throw new InvalidOperationException("minItems must be larger >= 0", null);
				}

				if (config.MaxPoolSize < config.MinPoolSize)
				{
					throw new InvalidOperationException("maxItems must be larger than minItems", null);
				}

				if (config.QueueTimeout < TimeSpan.Zero)
				{
					throw new InvalidOperationException("queueTimeout must be >= TimeSpan.Zero", null);
				}

				_ownerNode = ownerNode;
				_isAlive = true;
				_endPoint = ownerNode.EndPoint;
				_queueTimeout = config.QueueTimeout;

				_minItems = config.MinPoolSize;
				_maxItems = config.MaxPoolSize;

				_semaphore = new Semaphore(_maxItems, _maxItems);
				_freeItems = new InterlockedStack<PooledSocket>();
			}

			internal void InitPool()
			{
				try
				{
					if (_minItems > 0)
					{
						for (int i = 0; i < _minItems; i++)
						{
							_freeItems.Push(CreateSocket());

							// cannot connect to the server
							if (!_isAlive)
							{
								break;
							}
						}
					}

					if (_log.IsDebugEnabled)
					{
						_log.DebugFormat("Pool has been inited for {0} with {1} sockets", _endPoint, _minItems);
					}
				}
				catch (Exception e)
				{
					_log.Error("Could not init pool.", e);

					MarkAsDead();
				}
			}

			private PooledSocket CreateSocket()
			{
				var ps = _ownerNode.CreateSocket();
				ps.CleanupCallback = ReleaseSocket;

				return ps;
			}

			public bool IsAlive
			{
				get { return _isAlive; }
			}

			public DateTime MarkedAsDeadUtc
			{
				get { return _markedAsDeadUtc; }
			}

			/// <summary>
			/// Acquires a new item from the pool
			/// </summary>
			/// <returns>An <see cref="T:PooledSocket"/> instance which is connected to the memcached server, or <value>null</value> if the pool is dead.</returns>
			public IPooledSocketResult Acquire()
			{
				PooledSocketResult result = new PooledSocketResult();
				string message = string.Empty;

				bool hasDebug = _log.IsDebugEnabled;

				if (hasDebug)
				{
					_log.Debug("Acquiring stream from pool. " + _endPoint);
				}

				if (!_isAlive || _isDisposed)
				{
					message = "Pool is dead or disposed, returning null. " + _endPoint;
					result.Fail(message);

					if (hasDebug)
					{
						_log.Debug(message);
					}

					return result;
				}

				if (!_semaphore.WaitOne(_queueTimeout))
				{
					message = "Pool is full, timeouting. " + _endPoint;
					if (hasDebug)
					{
						_log.Debug(message);
					}

					result.Fail(message, new TimeoutException());

					// everyone is so busy
					return result;
				}

				// maybe we died while waiting
				if (!_isAlive)
				{
					message = "Pool is dead, returning null. " + _endPoint;
					if (hasDebug)
					{
						_log.Debug(message);
					}

					result.Fail(message);

					return result;
				}

				// do we have free items?
				if (_freeItems.TryPop(out PooledSocket retval))
				{
					#region [ get it from the pool         ]

					try
					{
						retval.Reset();

						message = "Socket was reset. " + retval.InstanceId;
						if (hasDebug)
						{
							_log.Debug(message);
						}

						result.Pass(message);
						result.Value = retval;
						return result;
					}
					catch (Exception e)
					{
						message = "Failed to reset an acquired socket.";
						_log.Error(message, e);

						MarkAsDead();
						result.Fail(message, e);
						return result;
					}

					#endregion
				}

				// free item pool is empty
				message = "Could not get a socket from the pool, Creating a new item. " + _endPoint;
				if (hasDebug)
				{
					_log.Debug(message);
				}

				try
				{
					// okay, create the new item
					retval = CreateSocket();
					result.Value = retval;
					result.Pass();
				}
				catch (Exception e)
				{
					message = "Failed to create socket. " + _endPoint;
					_log.Error(message, e);

					// eventhough this item failed the failure policy may keep the pool alive
					// so we need to make sure to release the semaphore, so new connections can be
					// acquired or created (otherwise dead conenctions would "fill up" the pool
					// while the FP pretends that the pool is healthy)
					_semaphore.Release();

					MarkAsDead();
					result.Fail(message);
					return result;
				}

				if (hasDebug)
				{
					_log.Debug("Done.");
				}

				return result;
			}

			private void MarkAsDead()
			{
				if (_log.IsDebugEnabled)
				{
					_log.DebugFormat("Mark as dead was requested for {0}", _endPoint);
				}

				bool shouldFail = _ownerNode.FailurePolicy.ShouldFail();

				if (_log.IsDebugEnabled)
				{
					_log.Debug("FailurePolicy.ShouldFail(): " + shouldFail);
				}

				if (shouldFail)
				{
					if (_log.IsWarnEnabled)
					{
						_log.WarnFormat("Marking node {0} as dead", _endPoint);
					}

					_isAlive = false;
					_markedAsDeadUtc = DateTime.UtcNow;

					_ownerNode.Failed?.Invoke(_ownerNode);
				}
			}

			/// <summary>
			/// Releases an item back into the pool
			/// </summary>
			/// <param name="socket"></param>
			private void ReleaseSocket(PooledSocket socket)
			{
				if (_log.IsDebugEnabled)
				{
					_log.Debug("Releasing socket " + socket.InstanceId);
					_log.Debug("Are we alive? " + _isAlive);
				}

				if (_isAlive)
				{
					// is it still working (i.e. the server is still connected)
					if (socket.IsAlive)
					{
						// mark the item as free
						_freeItems.Push(socket);

						// signal the event so if someone is waiting for it can reuse this item
						_semaphore.Release();
					}
					else
					{
						// kill this item
						socket.Destroy();

						// mark ourselves as not working for a while
						MarkAsDead();

						// make sure to signal the Acquire so it can create a new conenction
						// if the failure policy keeps the pool alive
						_semaphore.Release();
					}
				}
				else
				{
					// one of our previous sockets has died, so probably all of them
					// are dead. so, kill the socket (this will eventually clear the pool as well)
					socket.Destroy();
				}
			}

			~InternalPoolImpl()
			{
				try { ((IDisposable)this).Dispose(); }
				catch { }
			}

			/// <summary>
			/// Releases all resources allocated by this instance
			/// </summary>
			public void Dispose()
			{
				// this is not a graceful shutdown
				// if someone uses a pooled item then 99% that an exception will be thrown
				// somewhere. But since the dispose is mostly used when everyone else is finished
				// this should not kill any kittens
				if (!_isDisposed)
				{
					_isAlive = false;
					_isDisposed = true;

					while (_freeItems.TryPop(out PooledSocket ps))
					{
						try { ps.Destroy(); }
						catch { }
					}

					_ownerNode = null;
					_semaphore.Close();
					_semaphore = null;
					_freeItems = null;
				}
			}

			void IDisposable.Dispose()
			{
				Dispose();
			}
		}

		#endregion

		#region [ Comparer                     ]
		internal sealed class Comparer : IEqualityComparer<IMemcachedNode>
		{
			public static readonly Comparer Instance = new Comparer();

			bool IEqualityComparer<IMemcachedNode>.Equals(IMemcachedNode x, IMemcachedNode y)
			{
				return x.EndPoint.Equals(y.EndPoint);
			}

			int IEqualityComparer<IMemcachedNode>.GetHashCode(IMemcachedNode obj)
			{
				return obj.EndPoint.GetHashCode();
			}
		}
		#endregion

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal virtual PooledSocket CreateSocket()
		{
			return new PooledSocket(_endPoint, _config.ConnectionTimeout, _config.ReceiveTimeout, _config.KeepAliveStartDelay, _config.KeepAliveInterval);
		}

		//protected internal virtual PooledSocket CreateSocket(IPEndPoint endpoint, TimeSpan connectionTimeout, TimeSpan receiveTimeout)
		//{
		//    PooledSocket retval = new PooledSocket(endPoint, connectionTimeout, receiveTimeout);

		//    return retval;
		//}

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		protected virtual IPooledSocketResult ExecuteOperation(IOperation op)
		{
			var result = Acquire();
			if (result.Success && result.HasValue)
			{
				try
				{
					var socket = result.Value;
					var b = op.GetBuffer();

					socket.Write(b);

					var readResult = op.ReadResponse(socket);
					if (readResult.Success)
					{
						result.Pass();
					}
					else
					{
						readResult.Combine(result);
					}
					return result;
				}
				catch (IOException e)
				{
					_log.Error(e);

					result.Fail("Exception reading response", e);
					return result;
				}
				finally
				{
					((IDisposable)result.Value).Dispose();
				}
			}
			else
			{
				result.Fail("Failed to obtain socket from pool");
				return result;
			}

		}

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		protected virtual bool ExecuteOperationAsync(IOperation op, Action<bool> next)
		{
			var socket = Acquire().Value;
			if (socket == null)
			{
				return false;
			}

			var b = op.GetBuffer();

			try
			{
				socket.Write(b);

				bool rrs = op.ReadResponseAsync(socket, readSuccess =>
				{
					((IDisposable)socket).Dispose();

					next(readSuccess);
				});

				return rrs;
			}
			catch (IOException e)
			{
				_log.Error(e);
				((IDisposable)socket).Dispose();

				return false;
			}
		}

		#region [ IMemcachedNode               ]

		/// <summary>
		///
		/// </summary>
		IPEndPoint IMemcachedNode.EndPoint
		{
			get { return EndPoint; }
		}

		/// <summary>
		///
		/// </summary>
		bool IMemcachedNode.IsAlive
		{
			get { return IsAlive; }
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		bool IMemcachedNode.Ping()
		{
			return Ping();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		IOperationResult IMemcachedNode.Execute(IOperation op)
		{
			return ExecuteOperation(op);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		bool IMemcachedNode.ExecuteAsync(IOperation op, Action<bool> next)
		{
			return ExecuteOperationAsync(op, next);
		}

		/// <summary>
		///
		/// </summary>
		event Action<IMemcachedNode> IMemcachedNode.Failed
		{
			add { Failed += value; }
			remove { Failed -= value; }
		}

		#endregion
	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    Copyright (c) 2010 Attila Kiskó, enyim.com
 *
 *    Licensed under the Apache License, Version 2.0 (the "License");
 *    you may not use this file except in compliance with the License.
 *    You may obtain a copy of the License at
 *
 *        http://www.apache.org/licenses/LICENSE-2.0
 *
 *    Unless required by applicable law or agreed to in writing, software
 *    distributed under the License is distributed on an "AS IS" BASIS,
 *    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *    See the License for the specific language governing permissions and
 *    limitations under the License.
 *
 * ************************************************************/
#endregion
