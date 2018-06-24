using System;
using Tiveria.Common.Logging;

namespace Tiveria.Home.Knx
{
    internal class NLogLogManager : ILogManager
    {
        ILogger ILogManager.GetLogger(string name)
        {
            return new NLogLogger(NLog.LogManager.GetLogger(name));
        }

        ILogger ILogManager.GetLogger(Type type)
        {
            return new NLogLogger(NLog.LogManager.GetLogger(type.Name));
        }
    }

    internal class NLogLogger : ILogger
    {
        private readonly NLog.ILogger _internalLogger;

        public bool IsTraceEnabled { get; }
        public bool IsDebugEnabled { get; }
        public bool IsInfoEnabled { get; }
        public bool IsWarnEnabled { get; }
        public bool IsErrorEnabled { get; }
        public bool IsFatalEnabled { get; }

        public NLogLogger(NLog.ILogger logger)
        {
            _internalLogger = logger;
            IsTraceEnabled = _internalLogger.IsTraceEnabled;
            IsDebugEnabled = _internalLogger.IsDebugEnabled;
            IsInfoEnabled = _internalLogger.IsInfoEnabled;
            IsWarnEnabled = _internalLogger.IsWarnEnabled;
            IsErrorEnabled = _internalLogger.IsErrorEnabled;
            IsFatalEnabled = _internalLogger.IsFatalEnabled;
    }

    public void Trace(object message)
        {
            #if DEBUG
            _internalLogger.Trace(message);
            #endif
        }

        public void Trace(object message, Exception exception)
        {
            #if DEBUG
            _internalLogger.Trace(message.ToString(), exception, null);
            #endif
        }

        public void Debug(object message)
        {
            #if DEBUG
            _internalLogger.Debug(message);
            #endif
        }

        public void Debug(object message, Exception exception)
        {
            #if DEBUG
            _internalLogger.Debug(message.ToString(), exception, null);
            #endif
        }

        public void Error(object message)
        {
            _internalLogger.Error(message);
        }

        public void Error(object message, Exception exception)
        {
            _internalLogger.Error(message.ToString(), exception, null);
        }

        public void Fatal(object message)
        {
            _internalLogger.Fatal(message);
        }

        public void Fatal(object message, Exception exception)
        {
            _internalLogger.Fatal(message.ToString(), exception, null);
        }

        public void Info(object message)
        {
            _internalLogger.Info(message);
        }

        public void Info(object message, Exception exception)
        {
            _internalLogger.Info(message.ToString(), exception, null);
        }

        public void Warn(object message)
        {
            _internalLogger.Warn(message);
        }

        public void Warn(object message, Exception exception)
        {
            _internalLogger.Warn(message.ToString(), exception, null);
        }
    }
}
