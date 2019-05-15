using System;
using System.IO;
using System.Reflection;
using System.Threading;

using Enyim.Caching;
using Enyim.Caching.Configuration;
using Enyim.Caching.Memcached;

using log4net.Config;
using log4net.Repository;

using NUnit.Framework;

namespace MemcachedTest
{
	[TestFixture]
	public class FailurePolicyTest
	{
		[OneTimeSetUp]
		public void Setup()
		{
			ILoggerRepository loggerRepository = log4net.LogManager.GetRepository(Assembly.GetExecutingAssembly());
			FileInfo configFileInfo = new FileInfo("App.config");
			XmlConfigurator.Configure(loggerRepository, configFileInfo);
		}

		[TestCase]
		public void TestIfCalled()
		{
			MemcachedClientConfiguration config = new MemcachedClientConfiguration();
			config.AddServer("localhost", 12345);

			config.SocketPool.FailurePolicyFactory = new FakePolicy();
			config.SocketPool.ConnectionTimeout = TimeSpan.FromSeconds(4);
			config.SocketPool.ReceiveTimeout = TimeSpan.FromSeconds(6);

			MemcachedClient client = new MemcachedClient(config);

			Assert.IsNull(client.Get("a"), "Get should have failed.");
		}

		class FakePolicy : INodeFailurePolicy, INodeFailurePolicyFactory
		{
			bool INodeFailurePolicy.ShouldFail()
			{
				Assert.IsTrue(true);

				return true;
			}

			INodeFailurePolicy INodeFailurePolicyFactory.Create(IMemcachedNode node)
			{
				return new FakePolicy();
			}
		}

		[TestCase]
		public void TestThrottlingFailurePolicy()
		{
			MemcachedClientConfiguration config = new MemcachedClientConfiguration();
			config.AddServer("localhost", 12345);

			config.SocketPool.FailurePolicyFactory = new ThrottlingFailurePolicyFactory(4, TimeSpan.FromMilliseconds(2000));
			config.SocketPool.ConnectionTimeout = TimeSpan.FromMilliseconds(10);
			config.SocketPool.ReceiveTimeout = TimeSpan.FromMilliseconds(10);
			config.SocketPool.MinPoolSize = 1;
			config.SocketPool.MaxPoolSize = 1;

			MemcachedClient client = new MemcachedClient(config);
			bool canFail = false;
			bool didFail = false;

			client.NodeFailed += node =>
			{
				Assert.IsTrue(canFail, "canfail");

				didFail = true;
			};

			Assert.IsNull(client.Get("a"), "Get should have failed. 1");
			Assert.IsNull(client.Get("a"), "Get should have failed. 2");

			canFail = true;
			Thread.Sleep(2000);

			Assert.IsNull(client.Get("a"), "Get should have failed. 3");
			Assert.IsNull(client.Get("a"), "Get should have failed. 4");
			Assert.IsNull(client.Get("a"), "Get should have failed. 5");
			Assert.IsNull(client.Get("a"), "Get should have failed. 6");

			Assert.IsTrue(didFail, "didfail");
		}
	}
}
