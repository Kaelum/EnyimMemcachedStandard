using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Provides custom server pool implementations
	/// </summary>
	public interface IServerPool : IDisposable
	{
		/// <summary></summary>
		event Action<IMemcachedNode> NodeFailed;

		/// <summary></summary>
		IMemcachedNode Locate(string key);

		/// <summary></summary>
		IOperationFactory OperationFactory { get; }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IEnumerable<IMemcachedNode> GetWorkingNodes();

		/// <summary>
		///
		/// </summary>
		void Start();
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
