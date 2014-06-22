using System;
using log4net;
using MasterServer.ServiceInterface;

namespace MasterServer.Logger
{
    /// <summary>
    /// Log4Net Wrapper
    /// </summary>
    public class Log4NetLogger : ILogHandler
    {
        private readonly ILog log4NetLog;

        public Log4NetLogger()
        {
            log4NetLog = LogManager.GetLogger(typeof(Log4NetLogger));
        }

        public bool IsDebugEnabled
        {
            get { return log4NetLog.IsDebugEnabled; }
        }

        public bool IsErrorEnabled
        {
            get { return log4NetLog.IsErrorEnabled; }
        }

        public bool IsFatalEnabled
        {
            get { return log4NetLog.IsFatalEnabled; }
        }

        public bool IsInfoEnabled
        {
            get { return log4NetLog.IsInfoEnabled; }
        }

        public bool IsWarnEnabled
        {
            get { return log4NetLog.IsWarnEnabled; }
        }

        public void Debug(object message)
        {
            log4NetLog.Debug(message);
        }

        public void Debug(object message, Exception exception)
        {
            log4NetLog.Debug(message, exception);
        }

        public void Error(object message)
        {
            log4NetLog.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            log4NetLog.Error(message, exception);
        }

        public void Fatal(object message)
        {
            log4NetLog.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            log4NetLog.Fatal(message, exception);
        }

        public void Info(object message)
        {
            log4NetLog.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            log4NetLog.Info(message, exception);
        }

        public void Warn(object message)
        {
            log4NetLog.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            log4NetLog.Warn(message, exception);
        }
    }
}
