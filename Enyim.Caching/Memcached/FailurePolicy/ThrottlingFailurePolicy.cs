using System;
using System.Collections.Generic;

using Enyim.Caching.Configuration;

namespace Enyim.Caching.Memcached
{
	/// <summary>
	/// Fails a node when the specified number of failures happen in a specified time window.
	/// </summary>
	public class ThrottlingFailurePolicy : INodeFailurePolicy
	{
		private static readonly ILog _log = LogManager.GetLogger(typeof(ThrottlingFailurePolicy));
		private static readonly bool _logIsDebugEnabled = _log.IsDebugEnabled;

		private readonly int _resetAfter;
		private readonly int _failureThreshold;
		private DateTime _lastFailed;
		private int _failCounter;

		/// <summary>
		/// Creates a new instance of <see cref="T:ThrottlingFailurePolicy"/>.
		/// </summary>
		/// <param name="resetAfter">Specifies the time in milliseconds how long a node should function properly to reset its failure counter.</param>
		/// <param name="failureThreshold">Specifies the number of failures that must occur in the specified time window to fail a node.</param>
		public ThrottlingFailurePolicy(int resetAfter, int failureThreshold)
		{
			_resetAfter = resetAfter;
			_failureThreshold = failureThreshold;
		}

		bool INodeFailurePolicy.ShouldFail()
		{
			var now = DateTime.UtcNow;

			if (_lastFailed == DateTime.MinValue)
			{
				if (_logIsDebugEnabled)
				{
					_log.Debug("Setting fail counter to 1.");
				}

				_failCounter = 1;
			}
			else
			{
				int diff = (int)(now - _lastFailed).TotalMilliseconds;
				if (_logIsDebugEnabled)
				{
					_log.DebugFormat("Last fail was {0} msec ago with counter {1}.", diff, _failCounter);
				}

				if (diff <= _resetAfter)
				{
					_failCounter++;
				}
				else
				{
					_failCounter = 1;
				}
			}

			_lastFailed = now;

			if (_failCounter == _failureThreshold)
			{
				if (_logIsDebugEnabled)
				{
					_log.DebugFormat("Threshold reached, node will fail.");
				}

				_lastFailed = DateTime.MinValue;
				_failCounter = 0;

				return true;
			}

			if (_logIsDebugEnabled)
			{
				_log.DebugFormat("Current counter is {0}, threshold not reached.", _failCounter);
			}

			return false;
		}
	}

	/// <summary>
	/// Creates instances of <see cref="T:ThrottlingFailurePolicy"/>.
	/// </summary>
	public class ThrottlingFailurePolicyFactory : INodeFailurePolicyFactory, IProviderFactory<INodeFailurePolicyFactory>
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="failureThreshold"></param>
		/// <param name="resetAfter"></param>
		public ThrottlingFailurePolicyFactory(int failureThreshold, TimeSpan resetAfter)
			: this(failureThreshold, (int)resetAfter.TotalMilliseconds) { }

		/// <summary>
		///
		/// </summary>
		/// <param name="failureThreshold"></param>
		/// <param name="resetAfter"></param>
		public ThrottlingFailurePolicyFactory(int failureThreshold, int resetAfter)
		{
			ResetAfter = resetAfter;
			FailureThreshold = failureThreshold;
		}

		/// <summary>
		///		used by the config files
		/// </summary>
		internal ThrottlingFailurePolicyFactory() { }

		/// <summary>
		/// Gets or sets the amount of time of in milliseconds after the failure counter is reset.
		/// </summary>
		public int ResetAfter { get; private set; }

		/// <summary>
		/// Gets or sets the number of failures that must happen in a time window to make a node marked as failed.
		/// </summary>
		public int FailureThreshold { get; private set; }

		/// <summary>
		///
		/// </summary>
		/// <param name="node"></param>
		/// <returns></returns>
		INodeFailurePolicy INodeFailurePolicyFactory.Create(IMemcachedNode node)
		{
			return new ThrottlingFailurePolicy(ResetAfter, FailureThreshold);
		}

		/// <summary>
		///
		/// </summary>
		/// <returns></returns>
		INodeFailurePolicyFactory IProviderFactory<INodeFailurePolicyFactory>.Create()
		{
			return new ThrottlingFailurePolicyFactory(FailureThreshold, ResetAfter);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="parameters"></param>
		void IProvider.Initialize(Dictionary<string, string> parameters)
		{
			ConfigurationHelper.TryGetAndRemove(parameters, "failureThreshold", out int failureThreshold, true);

			if (failureThreshold < 1)
			{
				throw new InvalidOperationException("failureThreshold must be > 0");
			}

			FailureThreshold = failureThreshold;

			ConfigurationHelper.TryGetAndRemove(parameters, "resetAfter", out TimeSpan reset, true);

			if (reset <= TimeSpan.Zero)
			{
				throw new InvalidOperationException("resetAfter must be > 0msec");
			}

			ResetAfter = (int)reset.TotalMilliseconds;
		}
	}
}

#region [ License information          ]
/* ************************************************************
 *
 *    Copyright (c) 2011 Attila Kiskó, enyim.com
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
