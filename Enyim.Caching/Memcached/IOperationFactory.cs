using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IOperationFactory
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IGetOperation Get(string key);

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		IMultiGetOperation MultiGet(IList<string> keys);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IStoreOperation Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IDeleteOperation Delete(string key, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expires"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutatorOperation Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperation Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IStatsOperation Stats(string type);

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IFlushOperation Flush();
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
