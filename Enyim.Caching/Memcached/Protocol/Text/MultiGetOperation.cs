using System;
using System.Collections.Generic;
using System.Linq;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class MultiGetOperation : MultiItemOperation, IMultiGetOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(MultiGetOperation));

		private Dictionary<string, CacheItem> _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="keys"></param>
		public MultiGetOperation(IList<string> keys)
			: base(keys) { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			// gets key1 key2 key3 ... keyN\r\n

			string command = "gets " + string.Join(" ", Keys.ToArray()) + TextSocketHelper.CommandTerminator;

			return TextSocketHelper.GetCommandBuffer(command);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			Dictionary<string, CacheItem> retval = new Dictionary<string, CacheItem>();
			Dictionary<string, ulong> cas = new Dictionary<string, ulong>();

			try
			{
				GetResponse r;

				while ((r = GetHelper.ReadItem(socket)) != null)
				{
					string key = r.Key;

					retval[key] = r.Item;
					cas[key] = r.CasValue;
				}
			}
			catch (NotSupportedException)
			{
				throw;
			}
			catch (Exception e)
			{
				_log.Error(e);
			}

			_result = retval;
			Cas = cas;

			return new TextOperationResult().Pass();
		}

		/// <summary>
		///
		/// </summary>
		Dictionary<string, CacheItem> IMultiGetOperation.Result
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
