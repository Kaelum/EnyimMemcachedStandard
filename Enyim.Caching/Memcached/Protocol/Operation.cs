using System;
using System.Collections.Generic;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached.Protocol
{
	/// <summary>
	/// Base class for implementing operations.
	/// </summary>
	public abstract class Operation : IOperation
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(Operation));

		/// <summary>
		///
		/// </summary>
		protected Operation() { }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		internal protected abstract IList<ArraySegment<byte>> GetBuffer();

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		internal protected abstract IOperationResult ReadResponse(PooledSocket socket);

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		internal protected abstract bool ReadResponseAsync(PooledSocket socket, Action<bool> next);

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IList<ArraySegment<byte>> IOperation.GetBuffer()
		{
			return GetBuffer();
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		IOperationResult IOperation.ReadResponse(PooledSocket socket)
		{
			return ReadResponse(socket);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		bool IOperation.ReadResponseAsync(PooledSocket socket, Action<bool> next)
		{
			return ReadResponseAsync(socket, next);
		}

		/// <summary>
		///
		/// </summary>
		int IOperation.StatusCode
		{
			get { return StatusCode; }
		}

		/// <summary></summary>
		public int StatusCode { get; protected set; }
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
