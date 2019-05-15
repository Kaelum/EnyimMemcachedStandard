using System;
using System.Collections.Generic;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class TextOperationFactory : IOperationFactory
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <returns></returns>
		IGetOperation IOperationFactory.Get(string key)
		{
			return new GetOperation(key);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		/// <returns></returns>
		IMultiGetOperation IOperationFactory.MultiGet(IList<string> keys)
		{
			return new MultiGetOperation(keys);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="value"></param>
		/// <param name="expires"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IStoreOperation IOperationFactory.Store(StoreMode mode, string key, CacheItem value, uint expires, ulong cas)
		{
			if (cas == 0)
			{
				return new StoreOperation(mode, key, value, expires);
			}

			return new CasOperation(key, value, expires, (uint)cas);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <returns></returns>
		IDeleteOperation IOperationFactory.Delete(string key, ulong cas)
		{
			if (cas > 0)
			{
				throw new NotSupportedException("Text protocol does not support delete with cas.");
			}

			return new DeleteOperation(key);
		}

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
		IMutatorOperation IOperationFactory.Mutate(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires, ulong cas)
		{
			if (cas > 0)
			{
				throw new NotSupportedException("Text protocol does not support " + mode + " with cas.");
			}

			return new MutatorOperation(mode, key, delta);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="cas"></param>
		/// <param name="data"></param>
		/// <returns></returns>
		IConcatOperation IOperationFactory.Concat(ConcatenationMode mode, string key, ulong cas, ArraySegment<byte> data)
		{
			if (cas > 0)
			{
				throw new NotSupportedException("Text protocol does not support " + mode + " with cas.");
			}

			return new ConcateOperation(mode, key, data);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		IStatsOperation IOperationFactory.Stats(string type)
		{
			return new StatsOperation(type);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IFlushOperation IOperationFactory.Flush()
		{
			return new FlushOperation();
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
