using System;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class ConcateOperation : StoreOperationBase, IConcatOperation
	{
		private readonly ConcatenationMode _mode;

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="data"></param>
		internal ConcateOperation(ConcatenationMode mode, string key, ArraySegment<byte> data)
			: base(mode == ConcatenationMode.Append
					? StoreCommand.Append
					: StoreCommand.Prepend, key, new CacheItem() { Data = data, Flags = 0 }, 0, 0)
		{
			_mode = mode;
		}

		/// <summary>
		///
		/// </summary>
		ConcatenationMode IConcatOperation.Mode
		{
			get { return _mode; }
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
