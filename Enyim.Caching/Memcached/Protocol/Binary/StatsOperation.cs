using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class StatsOperation : BinaryOperation, IStatsOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(StatsOperation));

		private readonly string _type;
		private Dictionary<string, string> _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		public StatsOperation(string type)
		{
			_type = type;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected override BinaryRequest Build()
		{
			BinaryRequest request = new BinaryRequest(OpCode.Stat);
			if (!string.IsNullOrEmpty(_type))
			{
				request.Key = _type;
			}

			return request;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			BinaryResponse response = new BinaryResponse();
			Dictionary<string, string> serverData = new Dictionary<string, string>();
			bool retval = false;

			while (response.Read(socket) && response.KeyLength > 0)
			{
				retval = true;

				var data = response.Data;
				string key = BinaryConverter.DecodeKey(data.Array, data.Offset, response.KeyLength);
				string value = BinaryConverter.DecodeKey(data.Array, data.Offset + response.KeyLength, data.Count - response.KeyLength);

				serverData[key] = value;
			}

			_result = serverData;
			StatusCode = response.StatusCode;

			BinaryOperationResult result = new BinaryOperationResult()
			{
				StatusCode = StatusCode
			};

			result.PassOrFail(retval, "Failed to read response");
			return result;
		}

		/// <summary>
		///
		/// </summary>
		Dictionary<string, string> IStatsOperation.Result
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
