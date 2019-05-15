using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class SocketPoolConfiguration : ISocketPoolConfiguration
	{
		private int _minPoolSize = 10;
		private int _maxPoolSize = 20;
		private TimeSpan _connectionTimeout = new TimeSpan(0, 0, 10);
		private TimeSpan _receiveTimeout = new TimeSpan(0, 0, 10);
		private TimeSpan _deadTimeout = new TimeSpan(0, 0, 10);
		private TimeSpan _queueTimeout = new TimeSpan(0, 0, 0, 0, 100);
		private INodeFailurePolicyFactory _policyFactory = new FailImmediatelyPolicyFactory();
		private TimeSpan _keepAliveInterval;
		private TimeSpan _keepAliveStartDelay;

		/// <summary>
		///
		/// </summary>
		int ISocketPoolConfiguration.MinPoolSize
		{
			get { return _minPoolSize; }
			set
			{
				if (value < 0)
				{
					throw new ArgumentOutOfRangeException("value", "MinPoolSize must be >= 0!");
				}

				if (value > _maxPoolSize)
				{
					throw new ArgumentOutOfRangeException("value", "MinPoolSize must be <= MaxPoolSize!");
				}

				_minPoolSize = value;
			}
		}

		/// <summary>
		/// Gets or sets a value indicating the maximum amount of sockets per server in the socket pool.
		/// </summary>
		/// <returns>The maximum amount of sockets per server in the socket pool. The default is 20.</returns>
		/// <remarks>It should be 0.75 * (number of threads) for optimal performance.</remarks>
		int ISocketPoolConfiguration.MaxPoolSize
		{
			get { return _maxPoolSize; }
			set
			{
				if (value < _minPoolSize)
				{
					throw new ArgumentOutOfRangeException("value", "MaxPoolSize must be >= MinPoolSize!");
				}

				_maxPoolSize = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.ConnectionTimeout
		{
			get { return _connectionTimeout; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_connectionTimeout = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.ReceiveTimeout
		{
			get { return _receiveTimeout; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_receiveTimeout = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.QueueTimeout
		{
			get { return _queueTimeout; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_queueTimeout = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.DeadTimeout
		{
			get { return _deadTimeout; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_deadTimeout = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		INodeFailurePolicyFactory ISocketPoolConfiguration.FailurePolicyFactory
		{
			get
			{
				return _policyFactory;
			}

			set
			{
				_policyFactory = value ?? throw new ArgumentNullException("value");
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.KeepAliveInterval
		{
			get { return _keepAliveInterval; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_keepAliveInterval = value;
			}
		}

		/// <summary>
		///
		/// </summary>
		TimeSpan ISocketPoolConfiguration.KeepAliveStartDelay
		{
			get { return _keepAliveStartDelay; }
			set
			{
				if (value < TimeSpan.Zero)
				{
					throw new ArgumentOutOfRangeException("value", "value must be positive");
				}

				_keepAliveStartDelay = value;
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
