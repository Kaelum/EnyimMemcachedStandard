using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching
{
	/// <summary>
	/// Interface for API methods that return detailed operation results
	/// </summary>
	public interface IMemcachedResultsClient
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IGetOperationResult ExecuteGet(string key);

		/// <summary>
		///
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="key"></param>
		/// <returns></returns>
		IGetOperationResult<T> ExecuteGet<T>(string key);

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		IDictionary<string, IGetOperationResult> ExecuteGet(IEnumerable<string> keys);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IGetOperationResult ExecuteTryGet(string key, out object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteStore(StoreMode mode, string key, object value, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IStoreOperationResult ExecuteCas(StoreMode mode, string key, object value, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteDecrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expiresAt"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, DateTime expiresAt, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="validFor"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IMutateOperationResult ExecuteIncrement(string key, ulong defaultValue, ulong delta, TimeSpan validFor, ulong cas);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperationResult ExecuteAppend(string key, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperationResult ExecuteAppend(string key, ulong cas, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperationResult ExecutePrepend(string key, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperationResult ExecutePrepend(string key, ulong cas, ArraySegment<byte> data);

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IRemoveOperationResult ExecuteRemove(string key);
	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
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