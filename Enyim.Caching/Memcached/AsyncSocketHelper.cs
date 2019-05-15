//#define DEBUG_IO
using System;
using System.Net.Sockets;
using System.Threading;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public partial class PooledSocket
	{
		/// <summary>
		/// Supports exactly one reader and writer, but they can do IO concurrently
		/// </summary>
		private class AsyncSocketHelper
		{
			private const int _chunkSize = 65536;

			private PooledSocket _socket;
			private SlidingBuffer _asyncBuffer;

			private SocketAsyncEventArgs _readEvent;
#if DEBUG_IO
			private int doingIO;
#endif
			private int _remainingRead;
			private int _expectedToRead;
			private AsyncIOArgs _pendingArgs;

			private int _isAborted;
			private ManualResetEvent _readInProgressEvent;

			/// <summary>
			///
			/// </summary>
			/// <param name="socket"></param>
			public AsyncSocketHelper(PooledSocket socket)
			{
				_socket = socket;
				_asyncBuffer = new SlidingBuffer(_chunkSize);

				_readEvent = new SocketAsyncEventArgs();
				_readEvent.Completed += new EventHandler<SocketAsyncEventArgs>(AsyncReadCompleted);
				_readEvent.SetBuffer(new byte[_chunkSize], 0, _chunkSize);

				_readInProgressEvent = new ManualResetEvent(false);
			}

			/// <summary>
			/// returns true if io is pending
			/// </summary>
			/// <param name="p"></param>
			/// <returns></returns>
			public bool Read(AsyncIOArgs p)
			{
				int count = p.Count;
				if (count < 1)
				{
					throw new ArgumentOutOfRangeException("count", "count must be > 0");
				}
#if DEBUG_IO
				if (Interlocked.CompareExchange(ref this.doingIO, 1, 0) != 0)
					throw new InvalidOperationException("Receive is already in progress");
#endif
				_expectedToRead = p.Count;
				_pendingArgs = p;

				p.Fail = false;
				p.Result = null;

				if (_asyncBuffer.Available >= count)
				{
					PublishResult(false);

					return false;
				}
				else
				{
					_remainingRead = count - _asyncBuffer.Available;
					_isAborted = 0;

					BeginReceive();

					return true;
				}
			}

			public void DiscardBuffer()
			{
				_asyncBuffer.UnsafeClear();
			}

			private void BeginReceive()
			{
				while (_remainingRead > 0)
				{
					_readInProgressEvent.Reset();

					if (_socket._socket.ReceiveAsync(_readEvent))
					{
						// wait until the timeout elapses, then abort this reading process
						// EndREceive will be triggered sooner or later but its timeout
						// may be higher than our read timeout, so it's not reliable
						if (!_readInProgressEvent.WaitOne(_socket._socket.ReceiveTimeout))
						{
							AbortReadAndTryPublishError(false);
						}

						return;
					}

					EndReceive();
				}
			}

			void AsyncReadCompleted(object sender, SocketAsyncEventArgs e)
			{
				if (EndReceive())
				{
					BeginReceive();
				}
			}

			private void AbortReadAndTryPublishError(bool markAsDead)
			{
				if (markAsDead)
				{
					_socket._isAlive = false;
				}

				// we've been already aborted, so quit
				// both the EndReceive and the wait on the event can abort the read
				// but only one should of them should continue the async call chain
				if (Interlocked.CompareExchange(ref _isAborted, 1, 0) != 0)
				{
					return;
				}

				_remainingRead = 0;
				var p = _pendingArgs;
#if DEBUG_IO
				Thread.MemoryBarrier();

				this.doingIO = 0;
#endif

				p.Fail = true;
				p.Result = null;

				_pendingArgs.Next(p);
			}

			/// <summary>
			/// returns true when io is pending
			/// </summary>
			/// <returns></returns>
			private bool EndReceive()
			{
				_readInProgressEvent.Set();

				int read = _readEvent.BytesTransferred;
				if (_readEvent.SocketError != SocketError.Success
					|| read == 0)
				{
					AbortReadAndTryPublishError(true);//new IOException("Remote end has been closed"));

					return false;
				}

				_remainingRead -= read;
				_asyncBuffer.Append(_readEvent.Buffer, 0, read);

				if (_remainingRead <= 0)
				{
					PublishResult(true);

					return false;
				}

				return true;
			}

			private void PublishResult(bool isAsync)
			{
				var retval = _pendingArgs;

				byte[] data = new byte[_expectedToRead];
				_asyncBuffer.Read(data, 0, retval.Count);
				_pendingArgs.Result = data;
#if DEBUG_IO
				Thread.MemoryBarrier();
				this.doingIO = 0;
#endif

				if (isAsync)
				{
					_pendingArgs.Next(_pendingArgs);
				}
			}
		}
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
