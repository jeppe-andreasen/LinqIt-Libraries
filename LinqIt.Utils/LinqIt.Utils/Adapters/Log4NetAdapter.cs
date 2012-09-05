using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using log4net;

namespace LinqIt.Utils.Adapters
{
    public class Log4NetAdapter : ILoggingAdapter
    {
        public void Log(LogType type, string message)
        {
            var stackTrace = new StackTrace();
            log4net.ILog log = log4net.LogManager.GetLogger(stackTrace.GetFrame(1).GetMethod().DeclaringType);
            switch (type)
            {
                case LogType.Info:
                    log.Info(message);
                    break;
                case LogType.Warning:
                    log.Warn(message);
                    break;
                case LogType.Debug:
                    log.Debug(message);
                    break;
                case LogType.Error:
                    log.Error(message);
                    break;
            }
        }

        public void Log(LogType type, string message, Exception exc)
        {
            var stackTrace = new StackTrace();
            log4net.ILog log = log4net.LogManager.GetLogger(stackTrace.GetFrame(1).GetMethod().DeclaringType);
            switch (type)
            {
                case LogType.Info:
                    log.Info(message, exc);
                    break;
                case LogType.Warning:
                    log.Warn(message, exc);
                    break;
                case LogType.Debug:
                    log.Debug(message, exc);
                    break;
                case LogType.Error:
                    log.Error(message, exc);
                    break;
            }
        }
    }
}
