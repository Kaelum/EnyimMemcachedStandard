using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;

using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

using log4net.Config;
using log4net.Repository;

using NUnit.Framework;

namespace MemcachedTest
{
	[TestFixture]
	public class ConfigTest
	{
		[OneTimeSetUp]
		public void Setup()
		{
			ILoggerRepository loggerRepository = log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
			FileInfo configFileInfo = new FileInfo("App.config");
			XmlConfigurator.Configure(loggerRepository, configFileInfo);

			TestSetup.Run();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			TestSetup.Cleanup();
		}

		[TestCase]
		public void NewProvidersConfigurationTest()
		{
			ValidateConfig(ConfigurationManager.GetSection("test/newProviders") as IMemcachedClientConfiguration);
		}

		[TestCase]
		public void NewProvidersWithFactoryConfigurationTest()
		{
			ValidateConfig(ConfigurationManager.GetSection("test/newProvidersWithFactory") as IMemcachedClientConfiguration);
		}

		private static void ValidateConfig(IMemcachedClientConfiguration config)
		{
			Assert.IsNotNull(config);

			Assert.IsInstanceOf(typeof(TestKeyTransformer), config.CreateKeyTransformer());
			Assert.IsInstanceOf(typeof(TestLocator), config.CreateNodeLocator());
			Assert.IsInstanceOf(typeof(TestTranscoder), config.CreateTranscoder());
		}

		[TestCase]
		public void TestVBucketConfig()
		{
			IMemcachedClientConfiguration config = ConfigurationManager.GetSection("test/vbucket") as IMemcachedClientConfiguration;
			var loc = config.CreateNodeLocator();
		}

		/// <summary>
		/// Tests if the client can initialize itself from enyim.com/memcached
		/// </summary>
		[TestCase]
		public void DefaultConfigurationTest()
		{
			using (new MemcachedClient())
			{
				;
			}
		}

		/// <summary>
		/// Tests if the client can initialize itself from a specific config
		/// </summary>
		[TestCase]
		public void NamedConfigurationTestInConstructor()
		{
			Assert.DoesNotThrow(() =>
			{
				using (new MemcachedClient("test/validConfig"))
				{
				};
			});
		}

		[TestCase]
		public void TestLoadingNamedConfig()
		{
			IMemcachedClientConfiguration config = ConfigurationManager.GetSection("test/validConfig") as IMemcachedClientConfiguration;
			Assert.NotNull(config);

			var spc = config.SocketPool;
			Assert.NotNull(spc);

			TimeSpan expected = new TimeSpan(0, 12, 34);

			Assert.AreEqual(expected, spc.ConnectionTimeout);
			Assert.AreEqual(expected, spc.DeadTimeout);
			Assert.AreEqual(expected, spc.KeepAliveInterval);
			Assert.AreEqual(expected, spc.KeepAliveStartDelay);
			Assert.AreEqual(expected, spc.QueueTimeout);
			Assert.AreEqual(expected, spc.ReceiveTimeout);
			Assert.AreEqual(12, spc.MinPoolSize);
			Assert.AreEqual(48, spc.MaxPoolSize);
		}

		/// <summary>
		/// Tests if the client can handle an invalid configuration
		/// </summary>
		[TestCase]
		public void InvalidSectionTest()
		{
			Assert.Throws<ConfigurationErrorsException>(() =>
			{
				using (MemcachedClient client = new MemcachedClient("test/invalidConfig"))
				{
					Assert.IsFalse(false, ".ctor should have failed.");
				}
			});
		}

		[TestCase]
		public void NullConfigurationTest()
		{
			Assert.Throws<ArgumentNullException>(() =>
			{
				using (MemcachedClient client = new MemcachedClient((IMemcachedClientConfiguration)null))
				{
					Assert.IsFalse(false, ".ctor should have failed.");
				}
			});
		}

		/// <summary>
		/// Tests if the client can be decleratively initialized
		/// </summary>
		[TestCase]
		public void ProgrammaticConfigurationTest()
		{
			// try to hit all lines in the config classes
			MemcachedClientConfiguration mcc = new MemcachedClientConfiguration();

			mcc.Servers.Add(new System.Net.IPEndPoint(IPAddress.Loopback, 20000));
			mcc.Servers.Add(new System.Net.IPEndPoint(IPAddress.Loopback, 20002));

			mcc.NodeLocator = typeof(DefaultNodeLocator);
			mcc.KeyTransformer = new SHA1KeyTransformer();
			mcc.Transcoder = new DefaultTranscoder();

			mcc.SocketPool.MinPoolSize = 10;
			mcc.SocketPool.MaxPoolSize = 100;
			mcc.SocketPool.ConnectionTimeout = new TimeSpan(0, 0, 10);
			mcc.SocketPool.DeadTimeout = new TimeSpan(0, 0, 30);

			using (new MemcachedClient(mcc))
			{
				;
			}
		}

		[TestCase]
		public void ProgrammaticConfigurationTestWithDefaults()
		{
			MemcachedClientConfiguration mcc = new MemcachedClientConfiguration();

			// only add servers
			mcc.Servers.Add(new System.Net.IPEndPoint(IPAddress.Loopback, 20000));
			mcc.Servers.Add(new System.Net.IPEndPoint(IPAddress.Loopback, 20002));

			using (new MemcachedClient(mcc))
			{
				;
			}
		}

		[TestCase]
		public void TestThrottlingFailurePolicy()
		{
			IMemcachedClientConfiguration config = ConfigurationManager.GetSection("test/throttlingFailurePolicy") as IMemcachedClientConfiguration;

			var policyFactory = config.SocketPool.FailurePolicyFactory;

			Assert.IsNotNull(policyFactory);
			Assert.IsInstanceOf<ThrottlingFailurePolicyFactory>(policyFactory);

			ThrottlingFailurePolicyFactory tfp = (ThrottlingFailurePolicyFactory)policyFactory;

			Assert.IsTrue(tfp.FailureThreshold == 10, "failureThreshold must be 10");
			Assert.IsTrue(tfp.ResetAfter == 100, "resetAfter must be 100 msec");
		}
	}

	class TestTranscoderFactory : IProviderFactory<ITranscoder>
	{
		void IProvider.Initialize(Dictionary<string, string> parameters)
		{
			Assert.IsTrue(parameters.ContainsKey("test"));
		}

		ITranscoder IProviderFactory<ITranscoder>.Create()
		{
			return new TestTranscoder();
		}
	}

	class TestLocatorFactory : IProviderFactory<IMemcachedNodeLocator>
	{
		void IProvider.Initialize(Dictionary<string, string> parameters)
		{
			Assert.IsTrue(parameters.ContainsKey("test"));
		}

		IMemcachedNodeLocator IProviderFactory<IMemcachedNodeLocator>.Create()
		{
			return new TestLocator();
		}
	}

	class TestKeyTransformerFactory : IProviderFactory<IMemcachedKeyTransformer>
	{
		void IProvider.Initialize(Dictionary<string, string> parameters)
		{
			Assert.IsTrue(parameters.ContainsKey("test"));
		}

		IMemcachedKeyTransformer IProviderFactory<IMemcachedKeyTransformer>.Create()
		{
			return new TestKeyTransformer();
		}
	}

	class TestTranscoder : ITranscoder
	{
		CacheItem ITranscoder.Serialize(object o)
		{
			return new CacheItem();
		}

		object ITranscoder.Deserialize(CacheItem item)
		{
			return null;
		}
	}

	class TestLocator : IMemcachedNodeLocator
	{
		private IList<IMemcachedNode> _nodes;

		void IMemcachedNodeLocator.Initialize(IList<IMemcachedNode> nodes)
		{
			_nodes = nodes;
		}

		IMemcachedNode IMemcachedNodeLocator.Locate(string key)
		{
			return null;
		}

		IEnumerable<IMemcachedNode> IMemcachedNodeLocator.GetWorkingNodes()
		{
			return _nodes.ToArray();
		}
	}

	class TestKeyTransformer : IMemcachedKeyTransformer
	{
		string IMemcachedKeyTransformer.Transform(string key)
		{
			return null;
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
