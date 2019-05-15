using System;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public abstract class SaslStep : BinaryOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		protected SaslStep(ISaslAuthenticationProvider provider)
		{
			Provider = provider;
		}

		/// <summary></summary>
		protected ISaslAuthenticationProvider Provider { get; private set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			BinaryResponse response = new BinaryResponse();

			bool retval = response.Read(socket);

			StatusCode = response.StatusCode;
			Data = response.Data.Array;

			BinaryOperationResult result = new BinaryOperationResult
			{
				StatusCode = StatusCode
			};

			result.PassOrFail(retval, "Failed to read response");
			return result;
		}

		/// <summary></summary>
		public byte[] Data { get; private set; }
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
