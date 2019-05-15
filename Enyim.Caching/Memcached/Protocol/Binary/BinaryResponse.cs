using System;
using System.Text;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class BinaryResponse
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(BinaryResponse));

		private const byte _magic_value = 0x81;
		private const int _headerLength = 24;

		private const int _header_opcode = 1;
		private const int _header_key = 2; // 2-3
		private const int _header_extra = 4;
		private const int _header_datatype = 5;
		private const int _header_status = 6; // 6-7
		private const int _header_body = 8; // 8-11
		private const int _header_opaque = 12; // 12-15
		private const int _header_cas = 16; // 16-23

		/// <summary></summary>
		public byte Opcode;
		/// <summary></summary>
		public int KeyLength;
		/// <summary></summary>
		public byte DataType;
		/// <summary></summary>
		public int StatusCode;

		/// <summary></summary>
		public int CorrelationId;
		/// <summary></summary>
		public ulong CAS;

		/// <summary></summary>
		public ArraySegment<byte> Extra;
		/// <summary></summary>
		public ArraySegment<byte> Data;

		private string _responseMessage;

		/// <summary>
		///
		/// </summary>
		public BinaryResponse()
		{
			StatusCode = -1;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		public string GetStatusMessage()
		{
			if (Data.Array == null) { return null; }

			return _responseMessage ?? (_responseMessage = Encoding.ASCII.GetString(Data.Array, Data.Offset, Data.Count));
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		public unsafe bool Read(PooledSocket socket)
		{
			StatusCode = -1;

			if (!socket.IsAlive)
			{
				return false;
			}

			byte[] header = new byte[_headerLength];
			socket.Read(header, 0, header.Length);

			DeserializeHeader(header, out int dataLength, out int extraLength);

			if (dataLength > 0)
			{
				byte[] data = new byte[dataLength];
				socket.Read(data, 0, dataLength);

				Extra = new ArraySegment<byte>(data, 0, extraLength);
				Data = new ArraySegment<byte>(data, extraLength, data.Length - extraLength);
			}

			return StatusCode == 0;
		}

		/// <summary>
		/// Reads the response from the socket asynchronously.
		/// </summary>
		/// <param name="socket">The socket to read from.</param>
		/// <param name="next">The delegate whihc will continue processing the response. This is only called if the read completes asynchronoulsy.</param>
		/// <param name="ioPending">Set totrue if the read is still pending when ReadASync returns. In this case 'next' will be called when the read is finished.</param>
		/// <returns>
		/// If the socket is already dead, ReadAsync returns false, next is not called, ioPending = false
		/// If the read completes synchronously (e.g. data is received from the buffer), it returns true/false depending on the StatusCode, and ioPending is set to true, 'next' will not be called.
		/// It returns true if it has to read from the socket, so the operation will complate asynchronously at a later time. ioPending will be true, and 'next' will be called to handle the data
		/// </returns>
		public bool ReadAsync(PooledSocket socket, Action<bool> next, out bool ioPending)
		{
			StatusCode = -1;
			_currentSocket = socket;
			_next = next;

			AsyncIOArgs asyncEvent = new AsyncIOArgs
			{
				Count = _headerLength,
				Next = DoDecodeHeaderAsync
			};

			_shouldCallNext = true;

			if (socket.ReceiveAsync(asyncEvent))
			{
				ioPending = true;
				return true;
			}

			ioPending = false;
			_shouldCallNext = false;

			return (
				asyncEvent.Fail
				? false
				: DoDecodeHeader(asyncEvent, out ioPending)
			);
		}

		private PooledSocket _currentSocket;
		private int _dataLength;
		private int _extraLength;
		private bool _shouldCallNext;
		private Action<bool> _next;

		/// <summary></summary>
		public static byte Magic_Value => _magic_value;

		/// <summary></summary>
		public static int Header_Opcode => _header_opcode;

		/// <summary></summary>
		public static int Header_Datatype => _header_datatype;

		/// <summary></summary>
		public static int Header_Body => _header_body;

		/// <summary>
		///
		/// </summary>
		/// <param name="asyncEvent"></param>
		private void DoDecodeHeaderAsync(AsyncIOArgs asyncEvent)
		{
			_shouldCallNext = true;

			DoDecodeHeader(asyncEvent, out bool tmp);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="asyncEvent"></param>
		/// <param name="pendingIO"></param>
		/// <returns></returns>
		private bool DoDecodeHeader(AsyncIOArgs asyncEvent, out bool pendingIO)
		{
			pendingIO = false;

			if (asyncEvent.Fail)
			{
				if (_shouldCallNext)
				{
					_next(false);
				}

				return false;
			}

			DeserializeHeader(asyncEvent.Result, out _dataLength, out _extraLength);
			bool retval = StatusCode == 0;

			if (_dataLength == 0)
			{
				if (_shouldCallNext)
				{
					_next(retval);
				}
			}
			else
			{
				asyncEvent.Count = _dataLength;
				asyncEvent.Next = DoDecodeBodyAsync;

				if (_currentSocket.ReceiveAsync(asyncEvent))
				{
					pendingIO = true;
				}
				else
				{
					if (asyncEvent.Fail)
					{
						return false;
					}

					DoDecodeBody(asyncEvent);
				}
			}

			return retval;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="asyncEvent"></param>
		private void DoDecodeBodyAsync(AsyncIOArgs asyncEvent)
		{
			_shouldCallNext = true;
			DoDecodeBody(asyncEvent);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="asyncEvent"></param>
		private void DoDecodeBody(AsyncIOArgs asyncEvent)
		{
			if (asyncEvent.Fail)
			{
				if (_shouldCallNext)
				{
					_next(false);
				}

				return;
			}

			Extra = new ArraySegment<byte>(asyncEvent.Result, 0, _extraLength);
			Data = new ArraySegment<byte>(asyncEvent.Result, _extraLength, _dataLength - _extraLength);

			if (_shouldCallNext)
			{
				_next(true);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="header"></param>
		/// <param name="dataLength"></param>
		/// <param name="extraLength"></param>
		private unsafe void DeserializeHeader(byte[] header, out int dataLength, out int extraLength)
		{
			fixed (byte* buffer = header)
			{
				if (buffer[0] != _magic_value)
				{
					throw new InvalidOperationException("Expected magic value " + _magic_value + ", received: " + buffer[0]);
				}

				DataType = buffer[_header_datatype];
				Opcode = buffer[_header_opcode];
				StatusCode = BinaryConverter.DecodeUInt16(buffer, _header_status);

				KeyLength = BinaryConverter.DecodeUInt16(buffer, _header_key);
				CorrelationId = BinaryConverter.DecodeInt32(buffer, _header_opaque);
				CAS = BinaryConverter.DecodeUInt64(buffer, _header_cas);

				dataLength = BinaryConverter.DecodeInt32(buffer, _header_body);
				extraLength = buffer[_header_extra];
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
