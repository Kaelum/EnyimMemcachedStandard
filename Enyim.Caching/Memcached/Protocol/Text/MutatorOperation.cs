using System;
using System.Collections.Generic;
using System.Globalization;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class MutatorOperation : SingleItemOperation, IMutatorOperation
	{
		private readonly MutationMode _mode;
		private ulong _delta;
		private ulong _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="delta"></param>
		internal MutatorOperation(MutationMode mode, string key, ulong delta)
			: base(key)
		{
			_delta = delta;
			_mode = mode;
		}

		/// <summary>
		///
		/// </summary>
		public ulong Result
		{
			get { return _result; }
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			string command = $"{(_mode == MutationMode.Increment ? "incr " : "decr ")}{Key} {_delta.ToString(CultureInfo.InvariantCulture)}{TextSocketHelper.CommandTerminator}";

			return TextSocketHelper.GetCommandBuffer(command);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			string response = TextSocketHelper.ReadResponse(socket);
			TextOperationResult result = new TextOperationResult();

			//maybe we should throw an exception when the item is not found?
			if (string.Compare(response, "NOT_FOUND", StringComparison.Ordinal) == 0)
			{
				return result.Fail("Failed to read response.  Item not found");
			}

			result.Success = ulong.TryParse(response, NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite, CultureInfo.InvariantCulture, out _result);

			return result;
		}

		/// <summary>
		///
		/// </summary>
		MutationMode IMutatorOperation.Mode
		{
			get { return _mode; }
		}

		/// <summary>
		///
		/// </summary>
		ulong IMutatorOperation.Result
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
			throw new System.NotSupportedException();
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
