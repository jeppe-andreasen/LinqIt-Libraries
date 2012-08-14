//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Diagnostics;

//namespace LinqIt.Search.Utilities
//{
//    public class Logger
//    {
//        private static Logger m_logger;

//        public static Logger Current
//        {
//            get
//            {
//                if (m_logger == null)
//                {
//                    m_logger = new Logger();
//                }
//                return m_logger;
//            }
//        }

//        #region ILogger Members

//        public void AddBatch(LogBatch batch)
//        {
//            Info("----------------------------");
//            foreach (LogBatch.LogEntry entry in batch.Entries)
//            {
//                switch (entry.Type)
//                {
//                    case Utilities.LogBatch.LogType.Debug:
//                        Debug(entry.Message);
//                        break;
//                    case Utilities.LogBatch.LogType.Error:
//                        Error(entry.Message);
//                        break;
//                    case Utilities.LogBatch.LogType.Info:
//                        Info(entry.Message);
//                        break;
//                    case Utilities.LogBatch.LogType.Warn:
//                        Warning(entry.Message);
//                        break;
//                }
//            }
//        }

//        public void Verbose(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Verbose, message, parameters);
//        }

//        public void Info(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Information, message, parameters);
//        }

//        public void Warning(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Warning, message, parameters);
//        }

//        public void Error(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Error, message, parameters);
//        }

//        public void FatalError(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Critical, message, parameters);
//        }

//        #endregion

//        #region Class Methods

//        [Conditional("Debug")]
//        internal static void WriteDebug(string message, params string[] parameters)
//        {
//            string s = string.Format("{0}: {1}", DateTime.Now, string.Format(message, parameters));
//            Trace.WriteLine(s);
//        }

//        #endregion

//        public static void Log(TraceEventType traceEventType, string message, params string[] parameters)
//        {
//            if (parameters.Length > 0)
//                message = string.Format(message, parameters);

//            Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Write(message, "CrawlerService", 0, 0, traceEventType);
//            WriteDebug(message, parameters);
//        }

//        public static void PerformRollOfFlatFile()
//        {
//            foreach (LogSource traceSource in Microsoft.Practices.EnterpriseLibrary.Logging.Logger.Writer.TraceSources.Values)
//            {
//                foreach (TraceListener listener in traceSource.Listeners)
//                {
//                    if (listener is RollingFlatFileTraceListener)
//                    {
//                        var c = ((RollingFlatFileTraceListener)listener);
//                        c.RollingHelper.PerformRoll(DateTime.Now);
//                    }
//                }
//            }

//        }

//        public void Debug(string message, params string[] parameters)
//        {
//            Log(TraceEventType.Information, message, parameters);
//        }

//        internal void Break()
//        {
//            Info("--------------------------------------");
//        }
//    }
//}
