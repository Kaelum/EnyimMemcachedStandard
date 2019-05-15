using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Net;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public interface IVBucketConfiguration
	{
		/// <summary></summary>
		HashAlgorithm CreateHashAlgorithm();

		/// <summary></summary>
		IList<IPEndPoint> Servers { get; }

		/// <summary></summary>
		IList<VBucket> Buckets { get; }
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public struct VBucket
	{
		private readonly int _master;
		private readonly int[] _replicas;

		/// <summary>
		///
		/// </summary>
		/// <param name="master"></param>
		/// <param name="replicas"></param>
		public VBucket(int master, int[] replicas)
		{
			_master = master;
			_replicas = replicas;
		}

		/// <summary></summary>
		public int Master { get { return _master; } }

		/// <summary></summary>
		public int[] Replicas { get { return _replicas; } }
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
