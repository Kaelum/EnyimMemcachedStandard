using System;
using System.Diagnostics;

namespace Enyim.Caching.Memcached.Protocol
{
	/// <summary>
	/// Base class for implementing operations working with keyed items.
	/// </summary>
	public abstract class SingleItemOperation : Operation, ISingleItemOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		protected SingleItemOperation(string key)
		{
			Key = key;
		}

		/// <summary></summary>
		public string Key { get; private set; }

		/// <summary></summary>
		public ulong Cas { get; set; }

		/// <summary>
		/// The item key of the current operation.
		/// </summary>
		string ISingleItemOperation.Key
		{
			get { return Key; }
		}

		/// <summary>
		///
		/// </summary>
		ulong ISingleItemOperation.CasValue
		{
			get { return Cas; }
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
