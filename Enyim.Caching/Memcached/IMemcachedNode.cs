using System;
using System.Net;

using Enyim.Caching.Memcached.Results;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IMemcachedNode : IDisposable
	{
		/// <summary></summary>
		IPEndPoint EndPoint { get; }

		/// <summary></summary>
		bool IsAlive { get; }

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		bool Ping();

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <returns></returns>
		IOperationResult Execute(IOperation op);

		/// <summary>
		///
		/// </summary>
		/// <param name="op"></param>
		/// <param name="next"></param>
		/// <returns></returns>
		bool ExecuteAsync(IOperation op, Action<bool> next);

		//PooledSocket CreateSocket(TimeSpan connectionTimeout, TimeSpan receiveTimeout);

		/// <summary></summary>
		event Action<IMemcachedNode> Failed;

		//IAsyncResult BeginExecute(IOperation op, AsyncCallback callback, object state);
		//bool EndExecute(IAsyncResult result);
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
