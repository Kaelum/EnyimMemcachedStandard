using System;
using System.Collections.Generic;
using System.Text;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Implements the default plain text ("PLAIN") Memcached authentication.
	/// </summary>
	/// <remarks>Either use the parametrized constructor, or pass the "userName" and "password" parameters during initalization.</remarks>
	public sealed class PlainTextAuthenticator : ISaslAuthenticationProvider
	{
		private byte[] _authData;

		/// <summary>
		///
		/// </summary>
		public PlainTextAuthenticator() { }

		/// <summary>
		///
		/// </summary>
		/// <param name="zone"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		public PlainTextAuthenticator(string zone, string userName, string password)
		{
			_authData = CreateAuthData(zone, userName, password);
		}

		/// <summary>
		///
		/// </summary>
		string ISaslAuthenticationProvider.Type
		{
			get { return "PLAIN"; }
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="parameters"></param>
		void ISaslAuthenticationProvider.Initialize(Dictionary<string, object> parameters)
		{
			if (parameters != null)
			{
				string zone = (string)parameters["zone"];
				string userName = (string)parameters["userName"];
				string password = (string)parameters["password"];

				_authData = CreateAuthData(zone, userName, password);
			}
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		byte[] ISaslAuthenticationProvider.Authenticate()
		{
			return _authData;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="data"></param>
		/// <returns></returns>
		byte[] ISaslAuthenticationProvider.Continue(byte[] data)
		{
			return null;
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="zone"></param>
		/// <param name="userName"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		private static byte[] CreateAuthData(string zone, string userName, string password)
		{
			//message   = [authzid] UTF8NUL authcid UTF8NUL passwd
			//authcid   = 1*SAFE ; MUST accept up to 255 octets
			//authzid   = 1*SAFE ; MUST accept up to 255 octets
			//passwd    = 1*SAFE ; MUST accept up to 255 octets
			//UTF8NUL   = %x00 ; UTF-8 encoded NUL character
			return Encoding.UTF8.GetBytes(zone + "\0" + userName + "\0" + password);
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
