using System;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;
using Enyim.Caching.Memcached.Results.Helpers;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class StoreOperation : BinarySingleItemOperation, IStoreOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(StoreOperation));

		private readonly StoreMode _mode;
		private CacheItem _value;
		private readonly uint _expires;

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		public StoreOperation(StoreMode mode, string key, CacheItem value, uint expires) :
			base(key)
		{
			_mode = mode;
			_value = value;
			_expires = expires;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected override BinaryRequest Build()
		{
			OpCode op;
			switch (_mode)
			{
				case StoreMode.Add: op = OpCode.Add; break;
				case StoreMode.Set: op = OpCode.Set; break;
				case StoreMode.Replace: op = OpCode.Replace; break;
				default: throw new ArgumentOutOfRangeException("mode", _mode + " is not supported");
			}

			byte[] extra = new byte[8];

			BinaryConverter.EncodeUInt32((uint)_value.Flags, extra, 0);
			BinaryConverter.EncodeUInt32(_expires, extra, 4);

			BinaryRequest request = new BinaryRequest(op)
			{
				Key = Key,
				Cas = Cas,
				Extra = new ArraySegment<byte>(extra),
				Data = _value.Data
			};

			return request;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override IOperationResult ProcessResponse(BinaryResponse response)
		{
			BinaryOperationResult result = new BinaryOperationResult();

#if EVEN_MORE_LOGGING
			if (log.IsDebugEnabled)
				if (response.StatusCode == 0)
					log.DebugFormat("Store succeeded for key '{0}'.", this.Key);
				else
				{
					log.DebugFormat("Store failed for key '{0}'. Reason: {1}", this.Key, Encoding.ASCII.GetString(response.Data.Array, response.Data.Offset, response.Data.Count));
				}
#endif
			StatusCode = response.StatusCode;
			if (response.StatusCode == 0)
			{
				return result.Pass();
			}
			else
			{
				string message = ResultHelper.ProcessResponseData(response.Data);
				return result.Fail(message);
			}
		}

		/// <summary>
		///
		/// </summary>
		StoreMode IStoreOperation.Mode
		{
			get { return _mode; }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		protected internal override bool ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
		{
			throw new System.NotSupportedException();
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
