using System;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public enum MutationMode : byte
	{
		/// <summary></summary>
		Increment = 0x05,

		/// <summary></summary>
		Decrement = 0x06,
	};

	/// <summary>
	///		Summary description for
	/// </summary>
	public enum ConcatenationMode : byte
	{
		/// <summary></summary>
		Append = 0x0E,

		/// <summary></summary>
		Prepend = 0x0F,
	};

	/// <summary>
	///		Summary description for
	/// </summary>
	public enum MemcachedProtocol
	{
		/// <summary></summary>
		Binary,

		/// <summary></summary>
		Text,
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
