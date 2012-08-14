using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.Specialized;

namespace LinqIt.Search.Utilities
{
    public class LogBatch
    {
        internal enum LogType { Info, Warn, Error, Debug };
        private List<LogEntry> m_Entries = new List<LogEntry>();

        public void Info(string message)
        {
            Add(LogType.Info, message);
        }

        public void Warn(string message)
        {
            Add(LogType.Warn, message);
        }

        public void Error(string message)
        {
            Add(LogType.Error, message);
        }

        public void Debug(string message)
        {
            Add(LogType.Debug, message);
        }

        private void Add(LogType type, string message)
        {
            m_Entries.Add(new LogEntry(type, message));
        }

        internal IEnumerable<LogEntry> Entries
        {
            get { return m_Entries; }
        }

        internal class LogEntry
        {
            internal LogEntry(LogType type, string message)
            {
                Type = type;
                Message = message;
            }

            internal LogType Type { get; set; }

            internal string Message { get; set; }
        }
    }
}
