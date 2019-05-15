using System;

namespace Enyim.Caching
{
	/// <summary>
	/// The ILog interface is used by the client to log messages.
	/// </summary>
	/// <remarks>Use the <see cref="T:Enyim.Caching.LogManager" /> class to programmatically assign logger implementations.</remarks>
	public interface ILog
	{
		/// <summary></summary>
		bool IsDebugEnabled { get; }

		/// <summary></summary>
		bool IsInfoEnabled { get; }

		/// <summary></summary>
		bool IsWarnEnabled { get; }

		/// <summary></summary>
		bool IsErrorEnabled { get; }

		/// <summary></summary>
		bool IsFatalEnabled { get; }

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		void Debug(object message);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Debug(object message, Exception exception);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void DebugFormat(string format, object arg0);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void DebugFormat(string format, object arg0, object arg1);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void DebugFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void DebugFormat(string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void DebugFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		void Info(object message);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Info(object message, Exception exception);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void InfoFormat(string format, object arg0);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void InfoFormat(string format, object arg0, object arg1);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void InfoFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFormat(string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void InfoFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		void Warn(object message);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Warn(object message, Exception exception);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void WarnFormat(string format, object arg0);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void WarnFormat(string format, object arg0, object arg1);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void WarnFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void WarnFormat(string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void WarnFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		void Error(object message);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Error(object message, Exception exception);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void ErrorFormat(string format, object arg0);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void ErrorFormat(string format, object arg0, object arg1);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void ErrorFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void ErrorFormat(string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void ErrorFormat(IFormatProvider provider, string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		void Fatal(object message);

		/// <summary>
		///
		/// </summary>
		/// <param name="message"></param>
		/// <param name="exception"></param>
		void Fatal(object message, Exception exception);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		void FatalFormat(string format, object arg0);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		void FatalFormat(string format, object arg0, object arg1);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="arg0"></param>
		/// <param name="arg1"></param>
		/// <param name="arg2"></param>
		void FatalFormat(string format, object arg0, object arg1, object arg2);

		/// <summary>
		///
		/// </summary>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void FatalFormat(string format, params object[] args);

		/// <summary>
		///
		/// </summary>
		/// <param name="provider"></param>
		/// <param name="format"></param>
		/// <param name="args"></param>
		void FatalFormat(IFormatProvider provider, string format, params object[] args);
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
