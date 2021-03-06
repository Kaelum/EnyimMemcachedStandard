using System;
using System.ComponentModel;
using System.Configuration;
using Enyim.Caching.Memcached;

namespace Enyim.Caching.Configuration
{
	/// <summary>
	/// Configures the socket pool settings for Memcached servers.
	/// </summary>
	public sealed class SocketPoolElement : ConfigurationElement, ISocketPoolConfiguration
	{
		/// <summary>
		/// Gets or sets a value indicating the minimum amount of sockets per server in the socket pool.
		/// </summary>
		/// <returns>The minimum amount of sockets per server in the socket pool.</returns>
		[ConfigurationProperty("minPoolSize", IsRequired = false, DefaultValue = 10), IntegerValidator(MinValue = 0)]
		public int MinPoolSize
		{
			get { return (int)base["minPoolSize"]; }
			set { base["minPoolSize"] = value; }
		}

		/// <summary>
		/// Gets or sets a value indicating the maximum amount of sockets per server in the socket pool.
		/// </summary>
		/// <returns>The maximum amount of sockets per server in the socket pool. The default is 20.</returns>
		/// <remarks>It should be 0.75 * (number of threads) for optimal performance.</remarks>
		[ConfigurationProperty("maxPoolSize", IsRequired = false, DefaultValue = 20), IntegerValidator(MinValue = 0)]
		public int MaxPoolSize
		{
			get { return (int)base["maxPoolSize"]; }
			set { base["maxPoolSize"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which the connection attempt will fail.
		/// </summary>
		/// <returns>The value of the connection timeout. The default is 10 seconds.</returns>
		[ConfigurationProperty("connectionTimeout", IsRequired = false, DefaultValue = "00:00:10"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan ConnectionTimeout
		{
			get { return (TimeSpan)base["connectionTimeout"]; }
			set { base["connectionTimeout"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which the getting a connection from the pool will fail. The default is 100 msec.
		/// </summary>
		/// <returns>The value of the queue timeout.</returns>
		[ConfigurationProperty("queueTimeout", IsRequired = false, DefaultValue = "00:00:00.100"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan QueueTimeout
		{
			get { return (TimeSpan)base["queueTimeout"]; }
			set { base["queueTimeout"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which receiving data from the socket fails.
		/// </summary>
		/// <returns>The value of the receive timeout. The default is 10 seconds.</returns>
		[ConfigurationProperty("receiveTimeout", IsRequired = false, DefaultValue = "00:00:10"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan ReceiveTimeout
		{
			get { return (TimeSpan)base["receiveTimeout"]; }
			set { base["receiveTimeout"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time interval would sent keepalive packet(ms).
		/// </summary>
		[ConfigurationProperty("keepAliveInterval", IsRequired = false, DefaultValue = "00:05:00"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan KeepAliveInterval
		{
			get { return (TimeSpan)base["keepAliveInterval"]; }
			set { base["keepAliveInterval"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time will client starts sent keepalive packet(ms).
		/// </summary>
		[ConfigurationProperty("keepAliveStartFrom", IsRequired = false, DefaultValue = "00:05:00"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan KeepAliveStartDelay
		{
			get { return (TimeSpan)base["keepAliveStartFrom"]; }
			set { base["keepAliveStartFrom"] = value; }
		}

		/// <summary>
		/// Gets or sets a value that specifies the amount of time after which an unresponsive (dead) server will be checked if it is working.
		/// </summary>
		/// <returns>The value of the dead timeout. The default is 10 secs.</returns>
		[ConfigurationProperty("deadTimeout", IsRequired = false, DefaultValue = "00:00:10"), PositiveTimeSpanValidator, TypeConverter(typeof(InfiniteTimeSpanConverter))]
		public TimeSpan DeadTimeout
		{
			get { return (TimeSpan)base["deadTimeout"]; }
			set { base["deadTimeout"] = value; }
		}

		/// <summary>
		/// Called after deserialization.
		/// </summary>
		protected override void PostDeserialize()
		{
			base.PostDeserialize();

			if (MinPoolSize > MaxPoolSize)
			{
				throw new ConfigurationErrorsException("maxPoolSize must be larger than minPoolSize.");
			}
		}

		/// <summary>
		///
		/// </summary>
		[ConfigurationProperty("failurePolicyFactory", IsRequired = false)]
		public ProviderElement<INodeFailurePolicyFactory> FailurePolicyFactory
		{
			get { return (ProviderElement<INodeFailurePolicyFactory>)base["failurePolicyFactory"]; }
			set { base["failurePolicyFactory"] = value; }
		}

		#region [ ISocketPoolConfiguration     ]

		int ISocketPoolConfiguration.MinPoolSize
		{
			get { return MinPoolSize; }
			set { MinPoolSize = value; }
		}

		int ISocketPoolConfiguration.MaxPoolSize
		{
			get { return MaxPoolSize; }
			set { MaxPoolSize = value; }
		}

		TimeSpan ISocketPoolConfiguration.ConnectionTimeout
		{
			get { return ConnectionTimeout; }
			set { ConnectionTimeout = value; }
		}

		TimeSpan ISocketPoolConfiguration.DeadTimeout
		{
			get { return DeadTimeout; }
			set { DeadTimeout = value; }
		}

		TimeSpan ISocketPoolConfiguration.QueueTimeout
		{
			get { return QueueTimeout; }
			set { QueueTimeout = value; }
		}

		TimeSpan ISocketPoolConfiguration.ReceiveTimeout
		{
			get { return ReceiveTimeout; }
			set { ReceiveTimeout = value; }
		}

		TimeSpan ISocketPoolConfiguration.KeepAliveStartDelay
		{
			get { return KeepAliveStartDelay; }
			set { KeepAliveStartDelay = value; }
		}

		TimeSpan ISocketPoolConfiguration.KeepAliveInterval
		{
			get { return KeepAliveInterval; }
			set { KeepAliveInterval = value; }
		}

		INodeFailurePolicyFactory ISocketPoolConfiguration.FailurePolicyFactory
		{
			get { return FailurePolicyFactory.CreateInstance() ?? new FailImmediatelyPolicyFactory(); }
			set { }
		}

		#endregion
	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    Copyright (c) 2010 Attila Kisk�, enyim.com
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
