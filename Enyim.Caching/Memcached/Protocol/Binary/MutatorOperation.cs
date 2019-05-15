using System;

using Enyim.Caching.Memcached.Results.Extensions;
using Enyim.Caching.Memcached.Results.Helpers;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class MutatorOperation : BinarySingleItemOperation, IMutatorOperation
	{
		private readonly ulong _defaultValue;
		private readonly ulong _delta;
		private readonly uint _expires;
		private readonly MutationMode _mode;
		private ulong _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="mode"></param>
		/// <param name="key"></param>
		/// <param name="defaultValue"></param>
		/// <param name="delta"></param>
		/// <param name="expires"></param>
		public MutatorOperation(MutationMode mode, string key, ulong defaultValue, ulong delta, uint expires)
			: base(key)
		{
			if (delta < 0)
			{
				throw new ArgumentOutOfRangeException("delta", "delta must be >= 0");
			}

			_defaultValue = defaultValue;
			_delta = delta;
			_expires = expires;
			_mode = mode;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="request"></param>
		protected unsafe void UpdateExtra(BinaryRequest request)
		{
			byte[] extra = new byte[20];

			fixed (byte* buffer = extra)
			{
				BinaryConverter.EncodeUInt64(_delta, buffer, 0);

				BinaryConverter.EncodeUInt64(_defaultValue, buffer, 8);
				BinaryConverter.EncodeUInt32(_expires, buffer, 16);
			}

			request.Extra = new ArraySegment<byte>(extra);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected override BinaryRequest Build()
		{
			BinaryRequest request = new BinaryRequest((OpCode)_mode)
			{
				Key = Key,
				Cas = Cas
			};

			UpdateExtra(request);

			return request;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="response"></param>
		/// <returns></returns>
		protected override IOperationResult ProcessResponse(BinaryResponse response)
		{
			BinaryOperationResult result = new BinaryOperationResult();
			int status = response.StatusCode;
			StatusCode = status;

			if (status == 0)
			{
				var data = response.Data;
				if (data.Count != 8)
				{
					return result.Fail("Result must be 8 bytes long, received: " + data.Count, new InvalidOperationException());
				}

				_result = BinaryConverter.DecodeUInt64(data.Array, data.Offset);

				return result.Pass();
			}

			string message = ResultHelper.ProcessResponseData(response.Data);
			return result.Fail(message);
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
