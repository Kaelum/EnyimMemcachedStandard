using System;
using System.Net;
using System.Security;

using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached.Protocol.Binary
{
	/// <summary>
	/// A node which is used by the BinaryPool. It implements the binary protocol's SASL auth. mechanism.
	/// </summary>
	public class BinaryNode : MemcachedNode
	{
		private static readonly Enyim.Caching.ILog _log = LogManager.GetLogger(typeof(BinaryNode));

		private readonly ISaslAuthenticationProvider _authenticationProvider;

		/// <summary>
		///
		/// </summary>
		/// <param name="endpoint"></param>
		/// <param name="config"></param>
		/// <param name="authenticationProvider"></param>
		public BinaryNode(IPEndPoint endpoint, ISocketPoolConfiguration config, ISaslAuthenticationProvider authenticationProvider)
			: base(endpoint, config)
		{
			_authenticationProvider = authenticationProvider;
		}

		/// <summary>
		/// Authenticates the new socket before it is put into the pool.
		/// </summary>
		protected internal override PooledSocket CreateSocket()
		{
			var retval = base.CreateSocket();

			if (_authenticationProvider != null && !Auth(retval))
			{
				if (_log.IsErrorEnabled)
				{
					_log.Error("Authentication failed: " + EndPoint);
				}

				throw new SecurityException("auth failed: " + EndPoint);
			}

			return retval;
		}

		/// <summary>
		/// Implements memcached's SASL auth sequence. (See the protocol docs for more details.)
		/// </summary>
		/// <param name="socket"></param>
		/// <returns></returns>
		private bool Auth(PooledSocket socket)
		{
			SaslStep currentStep = new SaslStart(_authenticationProvider);

			socket.Write(currentStep.GetBuffer());

			while (!currentStep.ReadResponse(socket).Success)
			{
				// challenge-response authentication
				if (currentStep.StatusCode == 0x21)
				{
					currentStep = new SaslContinue(_authenticationProvider, currentStep.Data);
					socket.Write(currentStep.GetBuffer());
				}
				else
				{
					if (_log.IsWarnEnabled)
					{
						_log.WarnFormat("Authentication failed, return code: 0x{0:x}", currentStep.StatusCode);
					}

					// invalid credentials or other error
					return false;
				}
			}

			return true;
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
