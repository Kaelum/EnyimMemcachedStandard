using System;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///
	/// </summary>
	public class FlushOperation : BinaryOperation, IFlushOperation
	{
		/// <summary>
		///
		/// </summary>
		public FlushOperation() { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected override BinaryRequest Build()
		{
			BinaryRequest request = new BinaryRequest(OpCode.Flush);

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
			bool retval = response.Read(socket);

			StatusCode = StatusCode;
			BinaryOperationResult result = new BinaryOperationResult()
			{
				Success = retval,
				StatusCode = StatusCode
			};

			result.PassOrFail(retval, "Failed to read response");
			return result;
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
