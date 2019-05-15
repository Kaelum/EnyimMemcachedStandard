using System;

namespace Enyim.Caching
{
	internal class NLogWrapper : Enyim.Caching.ILog
	{
		private NLog.Logger _log;

		public NLogWrapper(NLog.Logger log)
		{
			_log = log;
		}

		#region [ ILog                         ]

		bool ILog.IsDebugEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		bool ILog.IsInfoEnabled
		{
			get { return _log.IsDebugEnabled; }
		}

		bool ILog.IsWarnEnabled
		{
			get { return _log.IsWarnEnabled; }
		}

		bool ILog.IsErrorEnabled
		{
			get { return _log.IsErrorEnabled; }
		}

		bool ILog.IsFatalEnabled
		{
			get { return _log.IsFatalEnabled; }
		}

		void ILog.Debug(object message)
		{
			_log.Debug(message);
		}

		void ILog.Debug(object message, Exception exception)
		{
			_log.Debug(exception, (message ?? string.Empty).ToString());
		}

		void ILog.DebugFormat(string format, object arg0)
		{
			_log.Debug(format, arg0);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1)
		{
			_log.Debug(format, arg0, arg1);
		}

		void ILog.DebugFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.Debug(format, arg0, arg1, arg2);
		}

		void ILog.DebugFormat(string format, params object[] args)
		{
			_log.Debug(format, args);
		}

		void ILog.DebugFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.Debug(provider, format, args);
		}

		void ILog.Info(object message)
		{
			_log.Info(message);
		}

		void ILog.Info(object message, Exception exception)
		{
			_log.Info(exception, (message ?? string.Empty).ToString());
		}

		void ILog.InfoFormat(string format, object arg0)
		{
			_log.Info(format, arg0);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1)
		{
			_log.Info(format, arg0, arg1);
		}

		void ILog.InfoFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.Info(format, arg0, arg1, arg2);
		}

		void ILog.InfoFormat(string format, params object[] args)
		{
			_log.Info(format, args);
		}

		void ILog.InfoFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.Info(provider, format, args);
		}

		void ILog.Warn(object message)
		{
			_log.Warn(message);
		}

		void ILog.Warn(object message, Exception exception)
		{
			_log.Warn(exception, (message ?? string.Empty).ToString());
		}

		void ILog.WarnFormat(string format, object arg0)
		{
			_log.Warn(format, arg0);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1)
		{
			_log.Warn(format, arg0, arg1);
		}

		void ILog.WarnFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.Warn(format, arg0, arg1, arg2);
		}

		void ILog.WarnFormat(string format, params object[] args)
		{
			_log.Warn(format, args);
		}

		void ILog.WarnFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.Warn(provider, format, args);
		}

		void ILog.Error(object message)
		{
			_log.Error(message);
		}

		void ILog.Error(object message, Exception exception)
		{
			_log.Error(exception, (message ?? string.Empty).ToString());
		}

		void ILog.ErrorFormat(string format, object arg0)
		{
			_log.Error(format, arg0);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1)
		{
			_log.Error(format, arg0, arg1);
		}

		void ILog.ErrorFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.Error(format, arg0, arg1, arg2);
		}

		void ILog.ErrorFormat(string format, params object[] args)
		{
			_log.Error(format, args);
		}

		void ILog.ErrorFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.Error(provider, format, args);
		}

		void ILog.Fatal(object message)
		{
			_log.Fatal(message);
		}

		void ILog.Fatal(object message, Exception exception)
		{
			_log.Fatal(exception, (message ?? string.Empty).ToString());
		}

		void ILog.FatalFormat(string format, object arg0)
		{
			_log.Fatal(format, arg0);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1)
		{
			_log.Fatal(format, arg0, arg1);
		}

		void ILog.FatalFormat(string format, object arg0, object arg1, object arg2)
		{
			_log.Fatal(format, arg0, arg1, arg2);
		}

		void ILog.FatalFormat(string format, params object[] args)
		{
			_log.Fatal(format, args);
		}

		void ILog.FatalFormat(IFormatProvider provider, string format, params object[] args)
		{
			_log.Fatal(provider, format, args);
		}

		#endregion
	}
}
