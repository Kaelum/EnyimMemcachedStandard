using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	///		Summary descritpion for ConfigurationHelper.
	/// </summary>
	public static class ConfigurationHelper
	{
		internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out int value, bool required)
		{
			string tmp;
			if (TryGetAndRemove(dict, name, out tmp, required)
				&& int.TryParse(tmp, out value))
			{
				return true;
			}

			if (required)
			{
				throw new System.Configuration.ConfigurationErrorsException("Missing or invalid parameter: " + (string.IsNullOrEmpty(name) ? "element content" : name));
			}

			value = 0;

			return false;
		}

		internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out TimeSpan value, bool required)
		{
			string tmp;
			if (TryGetAndRemove(dict, name, out tmp, required)
				&& TimeSpan.TryParse(tmp, out value))
			{
				return true;
			}

			if (required)
			{
				throw new System.Configuration.ConfigurationErrorsException("Missing or invalid parameter: " + (string.IsNullOrEmpty(name) ? "element content" : name));
			}

			value = TimeSpan.Zero;

			return false;
		}

		internal static bool TryGetAndRemove(Dictionary<string, string> dict, string name, out string value, bool required)
		{
			if (dict.TryGetValue(name, out value))
			{
				dict.Remove(name);

				if (!string.IsNullOrEmpty(value))
				{
					return true;
				}
			}

			if (required)
			{
				throw new System.Configuration.ConfigurationErrorsException("Missing parameter: " + (string.IsNullOrEmpty(name) ? "element content" : name));
			}

			return false;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="dict"></param>
		internal static void CheckForUnknownAttributes(Dictionary<string, string> dict)
		{
			if (dict.Count > 0)
			{
				throw new System.Configuration.ConfigurationErrorsException("Unrecognized parameter: " + dict.Keys.First());
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <param name="interfaceType"></param>
		public static void CheckForInterface(Type type, Type interfaceType)
		{
			if (type == null || interfaceType == null)
			{
				return;
			}

			if (Array.IndexOf<Type>(type.GetInterfaces(), interfaceType) == -1)
			{
				throw new System.Configuration.ConfigurationErrorsException("The type " + type.AssemblyQualifiedName + " must implement " + interfaceType.AssemblyQualifiedName);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static IPEndPoint ResolveToEndPoint(string value)
		{
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentNullException("value");
			}

			string[] parts = value.Split(':');
			if (parts.Length != 2)
			{
				throw new ArgumentException("host:port is expected", "value");
			}

			int port;
			if (!int.TryParse(parts[1], out port))
			{
				throw new ArgumentException("Cannot parse port: " + parts[1], "value");
			}

			return ResolveToEndPoint(parts[0], port);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="host"></param>
		/// <param name="port"></param>
		/// <returns></returns>
		public static IPEndPoint ResolveToEndPoint(string host, int port)
		{
			if (string.IsNullOrEmpty(host))
			{
				throw new ArgumentNullException("host");
			}

			IPAddress address;

			// parse as an IP address
			if (!IPAddress.TryParse(host, out address))
			{
				// not an ip, resolve from dns
				// TODO we need to find a way to specify whihc ip should be used when the host has several
				var entry = Dns.GetHostEntry(host);
				address = entry.AddressList.FirstOrDefault(ip => ip.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);

				if (address == null)
				{
					throw new ArgumentException(string.Format("Could not resolve host '{0}'.", host));
				}
			}

			return new IPEndPoint(address, port);
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
