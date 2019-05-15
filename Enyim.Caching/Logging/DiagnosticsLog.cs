using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.IO;
using System.Configuration;

namespace Enyim.Caching
{
	/// <summary>
	///		Summary description for
	/// </summary>
	public class DiagnosticsLogFactory : ILogFactory
	{
		private readonly TextWriter _writer;

		/// <summary>
		///
		/// </summary>
		public DiagnosticsLogFactory()
			: this(ConfigurationManager.AppSettings["Enyim.Caching.Diagnostics.LogPath"]) { }

		/// <summary>
		///
		/// </summary>
		/// <param name="logPath"></param>
		public DiagnosticsLogFactory(string logPath)
		{
			if (string.IsNullOrEmpty(logPath))
			{
				throw new ArgumentNullException(
					"Log path must be defined.  Add the following to configuration/appSettings: <add key=\"Enyim.Caching.Diagnostics.LogPath\" "
					+ "value=\"path to the log file\" /> or specify a valid path in in the constructor.");
			}

			_writer = new StreamWriter(logPath, true);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ILog ILogFactory.GetLogger(string name)
		{
			return new TextWriterLog(name, _writer);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ILog ILogFactory.GetLogger(Type type)
		{
			return new TextWriterLog(type.FullName, _writer);
		}
	}

	/// <summary>
	///		Summary description for
	/// </summary>
	public class ConsoleLogFactory : ILogFactory
	{
		/// <summary>
		///
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		ILog ILogFactory.GetLogger(string name)
		{
			return new TextWriterLog(name, Console.Out);
		}

		/// <summary>
		///
		/// </summary>
		/// <param name="type"></param>
		/// <returns></returns>
		ILog ILogFactory.GetLogger(Type type)
		{
			return new TextWriterLog(type.FullName, Console.Out);
		}
	}

	#region [ ILog implementation          ]

	internal class TextWriterLog : ILog
	{
		private const string _prefixDebug = "DEBUG";
		private const string _prefixInfo = "INFO";
		private const string _prefixWarn = "WARN";
		private const string _prefixError = "ERROR";
		private const string _prefixFatal = "FATAL";

		private TextWriter _writer;
		private readonly string _name;

		public TextWriterLog(string name, TextWriter writer)
		{
			_name = name;
			_writer = writer;
		}

		private void Dump(string prefix, string message, params object[] args)
		{
			string line = string.Format("{0:yyyy-MM-dd' 'HH:mm:ss} [{1}] {2} {3} - ", DateTime.Now, prefix, Thread.CurrentThread.ManagedThreadId, _name) + string.Format(message, args);

			lock (_writer)
			{
				_writer.WriteLine(line);
				_writer.Flush();
			}
		}

		private void Dump(string prefix, object message)
		{
			string line = string.Format("{0:yyyy-MM-dd' 'HH:mm:ss} [{1}] {2} {3} - {4}", DateTime.Now, prefix, Thread.CurrentThread.ManagedThreadId, _name, message);

			lock (_writer)
			{
				_writer.WriteLine(line);
				_writer.Flush();
			}
		}

		bool ILog.IsDebugEnabled
		{
			get { return true; }
		}

		bool ILog.IsInfoEnabled
		{
			get { return true; }
		}

		bool ILog.IsWarnEnabled
		{
			get { return true; }
		}

		bool ILog.IsErrorEnabled
		{
			get { return true; }
		}

		bool ILog.IsFatalEnabled
		{
			get { return true; }
		}

		void ILog.Debug(object message)
		{
			Dump(_prefixDebug, message);
		}

		void ILog.Debug(object message, Exception exception)
		{
			Dump(_prefixDebug, message + " - " + exception);
		}

		void ILog.DebugFormat(string format, object arg0)
		{
			Dump(_prefixDebug, format, arg0);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1)
		{
			Dump(_prefixDebug, format, arg0, arg1);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			Dump(_prefixDebug, format, arg0, arg1, arg2);
		}

		void ILog.DebugFormat(string format, params object[] args)
		{
			Dump(_prefixDebug, format, args);
		}

		void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			Dump(_prefixDebug, string.Format(provider, format, args));
		}

		void ILog.Info(object message)
		{
			Dump(_prefixInfo, message);
		}

		void ILog.Info(object message, Exception exception)
		{
			Dump(_prefixInfo, message + " - " + exception);
		}

		void ILog.InfoFormat(string format, object arg0)
		{
			Dump(_prefixInfo, format, arg0);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1)
		{
			Dump(_prefixInfo, format, arg0, arg1);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			Dump(_prefixInfo, format, arg0, arg1, arg2);
		}

		void ILog.InfoFormat(string format, params object[] args)
		{
			Dump(_prefixInfo, format, args);
		}

		void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			Dump(_prefixInfo, string.Format(provider, format, args));
		}

		void ILog.Warn(object message)
		{
			Dump(_prefixWarn, message);
		}

		void ILog.Warn(object message, Exception exception)
		{
			Dump(_prefixWarn, message + " - " + exception);
		}

		void ILog.WarnFormat(string format, object arg0)
		{
			Dump(_prefixWarn, format, arg0);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1)
		{
			Dump(_prefixWarn, format, arg0, arg1);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			Dump(_prefixWarn, format, arg0, arg1, arg2);
		}

		void ILog.WarnFormat(string format, params object[] args)
		{
			Dump(_prefixWarn, format, args);
		}

		void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			Dump(_prefixWarn, string.Format(provider, format, args));
		}

		void ILog.Error(object message)
		{
			Dump(_prefixError, message);
		}

		void ILog.Error(object message, Exception exception)
		{
			Dump(_prefixError, message + " - " + exception);
		}

		void ILog.ErrorFormat(string format, object arg0)
		{
			Dump(_prefixError, format, arg0);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1)
		{
			Dump(_prefixError, format, arg0, arg1);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			Dump(_prefixError, format, arg0, arg1, arg2);
		}

		void ILog.ErrorFormat(string format, params object[] args)
		{
			Dump(_prefixError, format, args);
		}

		void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			Dump(_prefixError, string.Format(provider, format, args));
		}

		void ILog.Fatal(object message)
		{
			Dump(_prefixFatal, message);
		}

		void ILog.Fatal(object message, Exception exception)
		{
			Dump(_prefixFatal, message + " - " + exception);
		}

		void ILog.FatalFormat(string format, object arg0)
		{
			Dump(_prefixFatal, format, arg0);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1)
		{
			Dump(_prefixFatal, format, arg0, arg1);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			Dump(_prefixFatal, format, arg0, arg1, arg2);
		}

		void ILog.FatalFormat(string format, params object[] args)
		{
			Dump(_prefixFatal, format, args);
		}

		void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			Dump(_prefixFatal, string.Format(provider, format, args));
		}
	}

	#endregion
}
