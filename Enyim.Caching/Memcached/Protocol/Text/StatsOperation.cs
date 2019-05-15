using System;
using System.Collections.Generic;

using Enyim.Caching.Memcached.Results;
using Enyim.Caching.Memcached.Results.Extensions;

namespace Enyim.Caching.Memcached.Protocol.Text
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class StatsOperation : Operation, IStatsOperation
	{
		private static Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(StatsOperation));

		private readonly string _type;
		private Dictionary<string, string> _result;

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		public StatsOperation(string type)
		{
			_type = type;
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		protected internal override IList<ArraySegment<byte>> GetBuffer()
		{
			string command = (
				string.IsNullOrEmpty(_type)
				? "stats" + TextSocketHelper.CommandTerminator
				: "stats " + _type + TextSocketHelper.CommandTerminator
			);

			return TextSocketHelper.GetCommandBuffer(command);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		protected internal override IOperationResult ReadResponse(PooledSocket socket)
		{
			Dictionary<string, string> serverData = new Dictionary<string, string>();

			while (true)
			{
				string line = TextSocketHelper.ReadResponse(socket);

				// stat values are terminated by END
				if (string.Compare(line, "END", StringComparison.Ordinal) == 0)
				{
					break;
				}

				// expected response is STAT item_name item_value
				if (line.Length < 6 || string.Compare(line, 0, "STAT ", 0, 5, StringComparison.Ordinal) != 0)
				{
					if (_log.IsWarnEnabled)
					{
						_log.Warn("Unknow response: " + line);
					}

					continue;
				}

				// get the key&value
				string[] parts = line.Remove(0, 5).Split(' ');
				if (parts.Length != 2)
				{
					if (_log.IsWarnEnabled)
					{
						_log.Warn("Unknow response: " + line);
					}

					continue;
				}

				// store the stat item
				serverData[parts[0]] = parts[1];
			}

			_result = serverData;

			return new TextOperationResult().Pass();
		}

		/// <summary>
		///
		/// </summary>
		Dictionary<string, string> IStatsOperation.Result
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
