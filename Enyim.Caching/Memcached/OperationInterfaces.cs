using System;
using System.Net;
using System.Collections.Generic;
using Enyim.Caching.Memcached.Protocol;
using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IOperation
	{
		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		IList<ArraySegment<byte>> GetBuffer();

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		IOperationResult ReadResponse(PooledSocket socket);

		/// <summary></summary>
		int StatusCode { get; }

		/// <summary>
		/// 'next' is called when the operation completes. The bool parameter indicates the success of the operation.
		/// </summary>
		/// <param name="socket"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		bool ReadResponseAsync(PooledSocket socket, Action<bool> next);
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface ISingleItemOperation : IOperation
	{
		/// <summary></summary>
		string Key { get; }

		/// <summary>
		/// The CAS value returned by the server after executing the command.
		/// </summary>
		ulong CasValue { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IMultiItemOperation : IOperation
	{
		/// <summary></summary>
		IList<string> Keys { get; }

		/// <summary></summary>
		Dictionary<string, ulong> Cas { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IGetOperation : ISingleItemOperation
	{
		/// <summary></summary>
		CacheItem Result { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IMultiGetOperation : IMultiItemOperation
	{
		/// <summary></summary>
		Dictionary<string, CacheItem> Result { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IStoreOperation : ISingleItemOperation
	{
		/// <summary></summary>
		StoreMode Mode { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IDeleteOperation : ISingleItemOperation { }

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IConcatOperation : ISingleItemOperation
	{
		/// <summary></summary>
		ConcatenationMode Mode { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IMutatorOperation : ISingleItemOperation
	{
		/// <summary></summary>
		MutationMode Mode { get; }

		/// <summary></summary>
		ulong Result { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IStatsOperation : IOperation
	{
		/// <summary></summary>
		Dictionary<string, string> Result { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IFlushOperation : IOperation { }

	/// <summary>
	///		Summary description for
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public struct CasResult<T>
	{
		/// <summary></summary>
		public T Result { get; set; }

		/// <summary></summary>
		public ulong Cas { get; set; }

		/// <summary></summary>
		public int StatusCode { get; set; }
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
