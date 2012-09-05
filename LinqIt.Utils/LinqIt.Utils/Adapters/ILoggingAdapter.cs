using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Adapters
{
    public interface ILoggingAdapter
    {
        void Log(LogType type, string message, Exception exc);

        void Log(LogType type, string message);
    }
}
