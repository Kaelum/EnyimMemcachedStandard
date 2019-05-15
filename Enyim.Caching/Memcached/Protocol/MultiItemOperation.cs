using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached.Protocol
{
	/// <summary>
	/// Base class for implementing operations working with multiple items.
	/// </summary>
	public abstract class MultiItemOperation : Operation, IMultiItemOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		public MultiItemOperation(IList<string> keys)
		{
			Keys = keys;
		}

		//Input
		/// <summary></summary>
		public IList<string> Keys { get; private set; }

		// Output
		/// <summary></summary>
		public Dictionary<string, ulong> Cas { get; protected set; }

		/// <summary></summary>
		IList<string> IMultiItemOperation.Keys { get { return Keys; } }

		/// <summary></summary>
		Dictionary<string, ulong> IMultiItemOperation.Cas { get { return Cas; } }
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
