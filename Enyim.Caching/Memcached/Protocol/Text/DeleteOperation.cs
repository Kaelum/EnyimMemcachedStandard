using System;

using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class DeleteOperation : SingleItemOperation, IDeleteOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		internal DeleteOperation(string key)
			: base(key) { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override System.Collections.Generic.IList<ArraySegment<byte>> GetBuffer()
		{
			string command = "delete " + Key + TextSocketHelper.CommandTerminator;

			return TextSocketHelper.GetCommandBuffer(command);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			return new TextOperationResult
			{
				Success = string.Compare(TextSocketHelper.ReadResponse(socket), "DELETED", StringComparison.Ordinal) == 0
			};
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
 *    Copyright (c) 2010 Attila Kisk�, enyim.com
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
