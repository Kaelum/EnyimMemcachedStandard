using System;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class GetOperation : SingleItemOperation, IGetOperation
	{
		private CacheItem _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		internal GetOperation(string key)
			: base(key) { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override System.Collections.Generic.IList<System.ArraySegment<byte>> GetBuffer()
		{
			string command = "gets " + Key + TextSocketHelper.CommandTerminator;

			return TextSocketHelper.GetCommandBuffer(command);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			GetResponse r = GetHelper.ReadItem(socket);
			TextOperationResult result = new TextOperationResult();

			if (r == null)
			{
				return result.Fail("Failed to read response");
			}

			_result = r.Item;
			Cas = r.CasValue;

			GetHelper.FinishCurrent(socket);

			return result.Pass();
		}

		/// <summary>
		///
		/// </summary>
		CacheItem IGetOperation.Result
		{
			get { return _result; }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		protected internal override bool ReadResponseAsync(PooledSocket socket, System.Action<bool> next)
		{
			throw new NotSupportedException();
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
