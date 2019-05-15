using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading;

using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class DefaultServerPool : IServerPool, IDisposable
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(DefaultServerPool));

		private IMemcachedNode[] _allNodes;

		private IMemcachedClientConfiguration _configuration;
		private readonly IOperationFactory _factory;
		private IMemcachedNodeLocator _nodeLocator;

		private readonly object _deadSync = new object();
		private System.Threading.Timer _resurrectTimer;
		private bool _isTimerActive;
		private readonly long _deadTimeoutMsec;
		private bool _isDisposed;
		private event Action<IMemcachedNode> NodeFailed;

		/// <summary>
		///
		/// </summary>
		/// <param name="configuration"></param>
		/// <param name="opFactory"></param>
		public DefaultServerPool(IMemcachedClientConfiguration configuration, IOperationFactory opFactory)
		{
			_configuration = configuration ?? throw new ArgumentNullException("socketConfig");
			_factory = opFactory ?? throw new ArgumentNullException("opFactory");

			_deadTimeoutMsec = (long)_configuration.SocketPool.DeadTimeout.TotalMilliseconds;
		}

		/// <summary>
		///
		/// </summary>
		~DefaultServerPool()
		{
			try { ((IDisposable)this).Dispose(); }
			catch { }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="endpoint"></param>
		/// <returns></returns>
		protected virtual IMemcachedNode CreateNode(IPEndPoint endpoint)
		{
			return new MemcachedNode(endpoint, _configuration.SocketPool);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="state"></param>
		private void RezCallback(object state)
		{
			bool isDebug = _log.IsDebugEnabled;

			if (isDebug)
			{
				_log.Debug("Checking the dead servers.");
			}

			// how this works:
			// 1. timer is created but suspended
			// 2. Locate encounters a dead server, so it starts the timer which will trigger after deadTimeout has elapsed
			// 3. if another server goes down before the timer is triggered, nothing happens in Locate (isRunning == true).
			//		however that server will be inspected sooner than Dead Timeout.
			//		   S1 died   S2 died    dead timeout
			//		|----*--------*------------*-
			//           |                     |
			//          timer start           both servers are checked here
			// 4. we iterate all the servers and record it in another list
			// 5. if we found a dead server whihc responds to Ping(), the locator will be reinitialized
			// 6. if at least one server is still down (Ping() == false), we restart the timer
			// 7. if all servers are up, we set isRunning to false, so the timer is suspended
			// 8. GOTO 2
			lock (_deadSync)
			{
				if (_isDisposed)
				{
					if (_log.IsWarnEnabled)
					{
						_log.Warn("IsAlive timer was triggered but the pool is already disposed. Ignoring.");
					}

					return;
				}

				var nodes = _allNodes;
				List<IMemcachedNode> aliveList = new List<IMemcachedNode>(nodes.Length);
				bool changed = false;
				int deadCount = 0;

				for (int i = 0; i < nodes.Length; i++)
				{
					var n = nodes[i];
					if (n.IsAlive)
					{
						if (isDebug)
						{
							_log.DebugFormat("Alive: {0}", n.EndPoint);
						}

						aliveList.Add(n);
					}
					else
					{
						if (isDebug)
						{
							_log.DebugFormat("Dead: {0}", n.EndPoint);
						}

						if (n.Ping())
						{
							changed = true;
							aliveList.Add(n);

							if (isDebug)
							{
								_log.Debug("Ping ok.");
							}
						}
						else
						{
							if (isDebug)
							{
								_log.Debug("Still dead.");
							}

							deadCount++;
						}
					}
				}

				// reinit the locator
				if (changed)
				{
					if (isDebug)
					{
						_log.Debug("Reinitializing the locator.");
					}

					_nodeLocator.Initialize(aliveList);
				}

				// stop or restart the timer
				if (deadCount == 0)
				{
					if (isDebug)
					{
						_log.Debug("deadCount == 0, stopping the timer.");
					}

					_isTimerActive = false;
				}
				else
				{
					if (isDebug)
					{
						_log.DebugFormat("deadCount == {0}, starting the timer.", deadCount);
					}

					_resurrectTimer.Change(_deadTimeoutMsec, Timeout.Infinite);
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		private void NodeFail(IMemcachedNode node)
		{
			bool isDebug = _log.IsDebugEnabled;
			if (isDebug)
			{
				_log.DebugFormat("Node {0} is dead.", node.EndPoint);
			}

			// the timer is stopped until we encounter the first dead server
			// when we have one, we trigger it and it will run after DeadTimeout has elapsed
			lock (_deadSync)
			{
				if (_isDisposed)
				{
					if (_log.IsWarnEnabled)
					{
						_log.Warn("Got a node fail but the pool is already disposed. Ignoring.");
					}

					return;
				}

				// bubble up the fail event to the client
				NodeFailed?.Invoke(node);

				// re-initialize the locator
				var newLocator = _configuration.CreateNodeLocator();
				newLocator.Initialize(_allNodes.Where(n => n.IsAlive).ToArray());
				Interlocked.Exchange(ref _nodeLocator, newLocator);

				// the timer is stopped until we encounter the first dead server
				// when we have one, we trigger it and it will run after DeadTimeout has elapsed
				if (!_isTimerActive)
				{
					if (isDebug)
					{
						_log.Debug("Starting the recovery timer.");
					}

					if (_resurrectTimer == null)
					{
						_resurrectTimer = new Timer(RezCallback, null, _deadTimeoutMsec, Timeout.Infinite);
					}
					else
					{
						_resurrectTimer.Change(_deadTimeoutMsec, Timeout.Infinite);
					}

					_isTimerActive = true;

					if (isDebug)
					{
						_log.Debug("Timer started.");
					}
				}
			}
		}

		#region [ IServerPool                  ]

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IMemcachedNode IServerPool.Locate(string key)
		{
			var node = _nodeLocator.Locate(key);

			return node;
		}

		/// <summary>
		///
		/// </summary>
		IOperationFactory IServerPool.OperationFactory
		{
			get { return _factory; }
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IEnumerable<IMemcachedNode> IServerPool.GetWorkingNodes()
		{
			return _nodeLocator.GetWorkingNodes();
		}

		void IServerPool.Start()
		{
			_allNodes = _configuration.Servers.
								Select(ip =>
								{
									var node = CreateNode(ip);
									node.Failed += NodeFail;

									return node;
								}).
								ToArray();

			// initialize the locator
			var locator = _configuration.CreateNodeLocator();
			locator.Initialize(_allNodes);

			_nodeLocator = locator;
		}

		/// <summary>
		///
		/// </summary>
		event Action<IMemcachedNode> IServerPool.NodeFailed
		{
			add { NodeFailed += value; }
			remove { NodeFailed -= value; }
		}

		#endregion

		#region [ IDisposable                  ]

		/// <summary>
		///
		/// </summary>
		void IDisposable.Dispose()
		{
			GC.SuppressFinalize(this);

			lock (_deadSync)
			{
				if (_isDisposed)
				{
					return;
				}

				_isDisposed = true;

				// dispose the locator first, maybe it wants to access
				// the nodes one last time
				if (_nodeLocator is IDisposable nd)
				{
					try { nd.Dispose(); }
					catch (Exception e)
					{
						if (_log.IsErrorEnabled)
						{
							_log.Error(e);
						}
					}
				}

				_nodeLocator = null;

				for (int i = 0; i < _allNodes.Length; i++)
				{
					try { _allNodes[i].Dispose(); }
					catch (Exception e)
					{
						if (_log.IsErrorEnabled)
						{
							_log.Error(e);
						}
					}
				}

				// stop the timer
				if (_resurrectTimer != null)
				{
					using (_resurrectTimer)
					{
						_resurrectTimer.Change(Timeout.Infinite, Timeout.Infinite);
					}
				}

				_allNodes = null;
				_resurrectTimer = null;
			}
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
