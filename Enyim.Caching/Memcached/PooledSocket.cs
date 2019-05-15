//#define DEBUG_IO
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;

namespace Enyim.Caching.Memcached
{
	[DebuggerDisplay("[ Address: {endpoint}, IsAlive = {IsAlive} ]")]
	public partial class PooledSocket : IDisposable
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(PooledSocket));

		private bool _isAlive;
		private Socket _socket;
		private IPEndPoint _endpoint;

		private BufferedStream _inputStream;
		private AsyncSocketHelper _helper;

		/// <summary>
		///
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="connectionTimeout"></param>
		/// <param name="receiveTimeout"></param>
		/// <param name="keepAliveStartDelay"></param>
		/// <param name="keepAliveInterval"></param>
		public PooledSocket(IPEndPoint endpoint, TimeSpan connectionTimeout, TimeSpan receiveTimeout, TimeSpan keepAliveStartDelay, TimeSpan keepAliveInterval)
		{
			_isAlive = true;

			Socket socket = new Socket(endpoint.AddressFamily, SocketType.Stream, ProtocolType.Tcp)
			{
				// TODO test if we're better off using nagle
				NoDelay = true
			};

			int timeout = connectionTimeout == TimeSpan.MaxValue
							? Timeout.Infinite
							: (int)connectionTimeout.TotalMilliseconds;

			int rcv = receiveTimeout == TimeSpan.MaxValue
				? Timeout.Infinite
				: (int)receiveTimeout.TotalMilliseconds;

			socket.ReceiveTimeout = rcv;
			socket.SendTimeout = rcv;

			ConfigureKeepAlive(socket, keepAliveStartDelay, keepAliveInterval);
			ConnectWithTimeout(socket, endpoint, timeout);

			_socket = socket;
			_endpoint = endpoint;

			_inputStream = new BufferedStream(new BasicNetworkStream(socket));
		}

		private static void ConfigureKeepAlive(Socket socket, TimeSpan keepAliveStartFrom, TimeSpan keepAliveInterval)
		{
			int SizeOfUint = Marshal.SizeOf((uint)0);
			byte[] inOptionValues = new byte[SizeOfUint * 3];
			bool isEnabled = keepAliveStartFrom > TimeSpan.Zero || keepAliveInterval > TimeSpan.Zero;

			BitConverter.GetBytes((uint)(isEnabled ? 1 : 0)).CopyTo(inOptionValues, 0);
			BitConverter.GetBytes((uint)keepAliveInterval.TotalMilliseconds).CopyTo(inOptionValues, SizeOfUint);
			BitConverter.GetBytes((uint)keepAliveStartFrom.TotalMilliseconds).CopyTo(inOptionValues, SizeOfUint * 2);

			socket.IOControl(IOControlCode.KeepAliveValues, inOptionValues, null);
		}

		private static void ConnectWithTimeout(Socket socket, IPEndPoint endpoint, int timeout)
		{
			ManualResetEvent mre = new ManualResetEvent(false);

			socket.BeginConnect(endpoint, iar =>
			{
				try { using (iar.AsyncWaitHandle)
					{
						socket.EndConnect(iar);
					}
				}
				catch { }

				mre.Set();
			}, null);

			if (!mre.WaitOne(timeout) || !socket.Connected)
			{
				using (socket)
				{
					throw new TimeoutException("Could not connect to " + endpoint);
				}
			}
		}

		/// <summary></summary>
		public Action<PooledSocket> CleanupCallback { get; set; }

		/// <summary>
		///
		/// </summary>
		public int Available
		{
			get { return _socket.Available; }
		}

		/// <summary>
		///
		/// </summary>
		public void Reset()
		{
			// discard any buffered data
			_inputStream.Flush();

			if (_helper != null)
			{
				_helper.DiscardBuffer();
			}

			int available = _socket.Available;

			if (available > 0)
			{
				if (_log.IsWarnEnabled)
				{
					_log.WarnFormat("Socket bound to {0} has {1} unread data! This is probably a bug in the code. InstanceID was {2}.", _socket.RemoteEndPoint, available, InstanceId);
				}

				byte[] data = new byte[available];

				Read(data, 0, available);

				if (_log.IsWarnEnabled)
				{
					_log.Warn(Encoding.ASCII.GetString(data));
				}
			}

			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("Socket {0} was reset", InstanceId);
			}
		}

		/// <summary>
		/// The ID of this instance. Used by the <see cref="T:MemcachedServer"/> to identify the instance in its inner lists.
		/// </summary>
		public readonly Guid InstanceId = Guid.NewGuid();

		/// <summary>
		///
		/// </summary>
		public bool IsAlive
		{
			get { return _isAlive; }
		}

		/// <summary>
		/// Releases all resources used by this instance and shuts down the inner <see cref="T:Socket"/>. This instance will not be usable anymore.
		/// </summary>
		/// <remarks>Use the IDisposable.Dispose method if you want to release this instance back into the pool.</remarks>
		public void Destroy()
		{
			Dispose(true);
		}

		/// <summary>
		///
		/// </summary>
		~PooledSocket()
		{
			try { Dispose(true); }
			catch { }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="disposing"></param>
		protected void Dispose(bool disposing)
		{
			if (disposing)
			{
				GC.SuppressFinalize(this);

				try
				{
					if (_socket != null)
					{
						try { _socket.Close(); }
						catch { }
					}

					if (_inputStream != null)
					{
						_inputStream.Dispose();
					}

					_inputStream = null;
					_socket = null;
					CleanupCallback = null;
				}
				catch (Exception e)
				{
					_log.Error(e);
				}
			}
			else
			{
				Action<PooledSocket> cc = CleanupCallback;

				if (cc != null)
				{
					try
					{
						cc(this);
					}
					catch (Exception e)
					{
						_log.Info("CleanupCallback failed during Dispose. Ignoring the exception.", e);
					}
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose(false);
		}

		/// <summary>
		///
		/// </summary>
		private void CheckDisposed()
		{
			if (_socket == null)
			{
				throw new ObjectDisposedException("PooledSocket");
			}
		}

		/// <summary>
		/// Reads the next byte from the server's response.
		/// </summary>
		/// <remarks>This method blocks and will not return until the value is read.</remarks>
		public int ReadByte()
		{
			CheckDisposed();

			try
			{
				return _inputStream.ReadByte();
			}
			catch (IOException)
			{
				_isAlive = false;

				throw;
			}
		}

		/// <summary>
		/// Reads data from the server into the specified buffer.
		/// </summary>
		/// <param name="buffer">An array of <see cref="T:System.Byte"/> that is the storage location for the received data.</param>
		/// <param name="offset">The location in buffer to store the received data.</param>
		/// <param name="count">The number of bytes to read.</param>
		/// <remarks>This method blocks and will not return until the specified amount of bytes are read.</remarks>
		public void Read(byte[] buffer, int offset, int count)
		{
			CheckDisposed();

			int read = 0;
			int shouldRead = count;

			while (read < count)
			{
				try
				{
					int currentRead = _inputStream.Read(buffer, offset, shouldRead);
					if (currentRead < 1)
					{
						continue;
					}

					read += currentRead;
					offset += currentRead;
					shouldRead -= currentRead;
				}
				catch (IOException)
				{
					_isAlive = false;
					throw;
				}
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <param name="offset"></param>
		/// <param name="length"></param>
		public void Write(byte[] data, int offset, int length)
		{
			CheckDisposed();

			_socket.Send(data, offset, length, SocketFlags.None, out var status);

			if (status != SocketError.Success)
			{
				_isAlive = false;

				ThrowHelper.ThrowSocketWriteError(_endpoint, status);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="buffers"></param>
		public void Write(IList<ArraySegment<byte>> buffers)
		{
			CheckDisposed();

#if DEBUG
			int total = 0;
			for (int i = 0, C = buffers.Count; i < C; i++)
			{
				total += buffers[i].Count;
			}

			if (_socket.Send(buffers, SocketFlags.None, out SocketError status) != total)
			{
				Debugger.Break();
			}
#else
			_socket.Send(buffers, SocketFlags.None, out SocketError status);
#endif

			if (status != SocketError.Success)
			{
				_isAlive = false;

				ThrowHelper.ThrowSocketWriteError(_endpoint, status);
			}
		}

		/// <summary>
		/// Receives data asynchronously. Returns true if the IO is pending. Returns false if the socket already failed or the data was available in the buffer.
		/// p.Next will only be called if the call completes asynchronously.
		/// </summary>
		public bool ReceiveAsync(AsyncIOArgs p)
		{
			CheckDisposed();

			if (!IsAlive)
			{
				p.Fail = true;
				p.Result = null;

				return false;
			}

			if (_helper == null)
			{
				_helper = new AsyncSocketHelper(this);
			}

			return _helper.Read(p);
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
