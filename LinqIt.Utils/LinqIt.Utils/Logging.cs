using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Adapters;

namespace LinqIt.Utils
{
    public enum LogType
    {
        Debug,
        Info,
        Warning,
        Error
    };

    public static class Logging
    {
        private static readonly ILoggingAdapter _adapter = new Log4NetAdapter();

        public static void Log(LogType type, string message)
        {
            _adapter.Log(type, message);
        }

        public static void Log(LogType type, string message, Exception exc)
        {
            _adapter.Log(type, message, exc);
        }
    }
}
