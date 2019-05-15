using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public abstract class BinaryMultiItemOperation : MultiItemOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		public BinaryMultiItemOperation(IList<string> keys)
			: base(keys) { }

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		protected abstract BinaryRequest Build(string key);

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			var keys = Keys;
			List<ArraySegment<byte>> retval = new List<ArraySegment<byte>>(keys.Count * 2);

			foreach (string k in keys)
			{
				Build(k).CreateBuffer(retval);
			}

			return retval;
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
