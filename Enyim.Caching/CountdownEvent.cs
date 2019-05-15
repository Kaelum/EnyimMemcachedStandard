using System;
using System.Linq;
using System.Configuration;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;
using System.Collections.Generic;
using System.Threading;
using System.Net;
using System.Diagnostics;

namespace Enyim.Caching
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class CountdownEvent : IDisposable
	{
		private int _count;
		private ManualResetEvent _mre;

		/// <summary>
		///
		/// </summary>
		/// <param name="count"></param>
		public CountdownEvent(int count)
		{
			_count = count;
			_mre = new ManualResetEvent(false);
		}

		/// <summary>
		///
		/// </summary>
		public void Signal()
		{
			if (_count == 0)
			{
				throw new InvalidOperationException("Counter underflow");
			}

			int tmp = Interlocked.Decrement(ref _count);

			if (tmp == 0)
			{ if (!_mre.Set())
				{
					throw new InvalidOperationException("couldn't signal");
				}
			}
			else if (tmp < 0)
			{
				throw new InvalidOperationException("Counter underflow");
			}
		}

		/// <summary>
		///
		/// </summary>
		public void Wait()
		{
			if (_count == 0)
			{
				return;
			}

			_mre.WaitOne();
		}

		/// <summary>
		///
		/// </summary>
		~CountdownEvent()
		{
			Dispose();
		}

		/// <summary>
		///
		/// </summary>
		void IDisposable.Dispose()
		{
			Dispose();
		}

		/// <summary>
		///
		/// </summary>
		public void Dispose()
		{
			GC.SuppressFinalize(this);

			if (_mre != null)
			{
				_mre.Close();
				_mre = null;
			}
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
