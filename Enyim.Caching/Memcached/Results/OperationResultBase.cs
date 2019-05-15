using System;

namespace Enyim.Caching.Memcached.Results
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public abstract class OperationResultBase : IOperationResult
	{
		/// <summary></summary>
		public bool Success { get; set; }

		/// <summary></summary>
		public string Message { get; set; }

		/// <summary></summary>
		public Exception Exception { get; set; }

		/// <summary></summary>
		public int? StatusCode { get; set; }

		/// <summary></summary>
		public IOperationResult InnerResult { get; set; }

	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    @author Couchbase <info@couchbase.com>
 *    @copyright 2012 Couchbase, Inc.
 *    @copyright 2012 Attila Kiskó, enyim.com
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