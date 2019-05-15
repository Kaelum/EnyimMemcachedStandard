using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class MultiGetOperation : BinaryMultiItemOperation, IMultiGetOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(MultiGetOperation));

		private Dictionary<string, CacheItem> _result;
		private Dictionary<int, string> _idToKey;
		private int _noopId;

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		public MultiGetOperation(IList<string> keys)
			: base(keys) { }

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected override BinaryRequest Build(string key)
		{
			BinaryRequest request = new BinaryRequest(OpCode.GetQ)
			{
				Key = key
			};

			return request;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			var keys = Keys;

			if (keys == null || keys.Count == 0)
			{
				if (_log.IsWarnEnabled)
				{
					_log.Warn("Empty multiget!");
				}

				return new ArraySegment<byte>[0];
			}

			if (_log.IsDebugEnabled)
			{
				_log.DebugFormat("Building multi-get for {0} keys", keys.Count);
			}

			// map the command's correlationId to the item key,
			// so we can use GetQ (which only returns the item data)
			_idToKey = new Dictionary<int, string>();

			// get ops have 2 segments, header + key
			List<ArraySegment<byte>> buffers = new List<ArraySegment<byte>>(keys.Count * 2);

			foreach (string key in keys)
			{
				var request = Build(key);

				request.CreateBuffer(buffers);

				// we use this to map the responses to the keys
				_idToKey[request.CorrelationId] = key;
			}

			// uncork the server
			BinaryRequest noop = new BinaryRequest(OpCode.NoOp);
			_noopId = noop.CorrelationId;

			noop.CreateBuffer(buffers);

			return buffers;
		}


		private PooledSocket _currentSocket;
		private BinaryResponse _asyncReader;
		private bool? _asyncLoopState;
		private Action<bool> _afterAsyncRead;

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		protected internal override bool ReadResponseAsync(PooledSocket socket, Action<bool> next)
		{
			_result = new Dictionary<string, CacheItem>();
			Cas = new Dictionary<string, ulong>();

			_currentSocket = socket;
			_asyncReader = new BinaryResponse();
			_asyncLoopState = null;
			_afterAsyncRead = next;

			return DoReadAsync();
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		private bool DoReadAsync()
		{
			var reader = _asyncReader;

			while (_asyncLoopState == null)
			{
				bool readSuccess = reader.ReadAsync(_currentSocket, EndReadAsync, out bool ioPending);
				StatusCode = reader.StatusCode;

				if (ioPending)
				{
					return readSuccess;
				}

				if (!readSuccess)
				{
					_asyncLoopState = false;
				}
				else if (reader.CorrelationId == _noopId)
				{
					_asyncLoopState = true;
				}
				else
				{
					StoreResult(reader);
				}
			}

			_afterAsyncRead((bool)_asyncLoopState);

			return true;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="readSuccess"></param>
		private void EndReadAsync(bool readSuccess)
		{
			if (!readSuccess)
			{
				_asyncLoopState = false;
			}
			else if (_asyncReader.CorrelationId == _noopId)
			{
				_asyncLoopState = true;
			}
			else
			{
				StoreResult(_asyncReader);
			}

			DoReadAsync();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="reader"></param>
		private void StoreResult(BinaryResponse reader)
		{
			// find the key to the response
			if (!_idToKey.TryGetValue(reader.CorrelationId, out string key))
			{
				// we're not supposed to get here tho
				_log.WarnFormat("Found response with CorrelationId {0}, but no key is matching it.", reader.CorrelationId);
			}
			else
			{
				if (_log.IsDebugEnabled)
				{
					_log.DebugFormat("Reading item {0}", key);
				}

				// deserialize the response
				ushort flags = (ushort)BinaryConverter.DecodeInt32(reader.Extra, 0);

				_result[key] = new CacheItem(flags, reader.Data);
				Cas[key] = reader.CAS;
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			_result = new Dictionary<string, CacheItem>();
			Cas = new Dictionary<string, ulong>();
			TextOperationResult result = new TextOperationResult();

			BinaryResponse response = new BinaryResponse();

			while (response.Read(socket))
			{
				StatusCode = response.StatusCode;

				// found the noop, quit
				if (response.CorrelationId == _noopId)
				{
					return result.Pass();
				}

				// find the key to the response
				if (!_idToKey.TryGetValue(response.CorrelationId, out string key))
				{
					// we're not supposed to get here tho
					_log.WarnFormat("Found response with CorrelationId {0}, but no key is matching it.", response.CorrelationId);
					continue;
				}

				if (_log.IsDebugEnabled)
				{
					_log.DebugFormat("Reading item {0}", key);
				}

				// deserialize the response
				int flags = BinaryConverter.DecodeInt32(response.Extra, 0);

				_result[key] = new CacheItem((ushort)flags, response.Data);
				Cas[key] = response.CAS;
			}

			// finished reading but we did not find the NOOP
			return result.Fail("Found response with CorrelationId {0}, but no key is matching it.");
		}

		/// <summary>
		///
		/// </summary>
		public Dictionary<string, CacheItem> Result
		{
			get { return _result; }
		}

		/// <summary>
		///
		/// </summary>
		Dictionary<string, CacheItem> IMultiGetOperation.Result
		{
			get { return _result; }
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
