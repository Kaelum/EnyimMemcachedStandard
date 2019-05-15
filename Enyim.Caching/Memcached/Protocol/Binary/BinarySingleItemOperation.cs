using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public abstract class BinarySingleItemOperation : SingleItemOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		protected BinarySingleItemOperation(string key) : base(key) { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected abstract BinaryRequest Build();

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			return Build().CreateBuffer();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected abstract IOperationResult ProcessResponse(BinaryResponse response);

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			BinaryResponse response = new BinaryResponse();
			bool retval = response.Read(socket);

			Cas = response.CAS;
			StatusCode = response.StatusCode;

			BinaryOperationResult result = new BinaryOperationResult()
			{
				Success = retval,
				Cas = Cas,
				StatusCode = StatusCode
			};

			IOperationResult responseResult;
			if (! (responseResult = ProcessResponse(response)).Success)
			{
				result.InnerResult = responseResult;
				responseResult.Combine(result);
			}

			return result;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		protected internal override bool ReadResponseAsync(PooledSocket socket, Action<bool> next)
		{
			throw new NotImplementedException();
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
