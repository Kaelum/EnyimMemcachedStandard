using System;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;
using Enyim.Caching.Memcached.Results.Helpers;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///
	/// </summary>
	public class GetOperation : BinarySingleItemOperation, IGetOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(GetOperation));
		private CacheItem _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		public GetOperation(string key)
			: base(key) { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected override BinaryRequest Build()
		{
			BinaryRequest request = new BinaryRequest(OpCode.Get)
			{
				Key = Key,
				Cas = Cas
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
			int status = response.StatusCode;
			BinaryOperationResult result = new BinaryOperationResult();

			StatusCode = status;

			if (status == 0)
			{
				int flags = BinaryConverter.DecodeInt32(response.Extra, 0);
				_result = new CacheItem((ushort)flags, response.Data);
				Cas = response.CAS;

#if EVEN_MORE_LOGGING
				if (log.IsDebugEnabled)
					log.DebugFormat("Get succeeded for key '{0}'.", this.Key);
#endif

				return result.Pass();
			}

			Cas = 0;

#if EVEN_MORE_LOGGING
			if (log.IsDebugEnabled)
				log.DebugFormat("Get failed for key '{0}'. Reason: {1}", this.Key, Encoding.ASCII.GetString(response.Data.Array, response.Data.Offset, response.Data.Count));
#endif

			string message = ResultHelper.ProcessResponseData(response.Data);
			return result.Fail(message);
		}

		/// <summary>
		///
		/// </summary>
		CacheItem IGetOperation.Result
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
