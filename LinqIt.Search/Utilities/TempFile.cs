using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Diagnostics;

namespace LinqIt.Search.Utilities
{
    /// <summary>
    /// TempFile creates a temp file, that deleted itself when garbage collected
    /// </summary>
    public class TempFile : DisposableBase
    {
        #region Fields

        private string m_Filename = string.Empty;

        #endregion

        #region Constructors

        public TempFile()
        {
            m_Filename = GetTempFileName();
        }

        public TempFile(string fileName)
        {
            m_Filename = fileName;
        }

        #endregion

        #region Properties

        public string FileName
        {
            get { return m_Filename; }
            set { m_Filename = value; }
        }

        public string Tag { get; set; }

        #endregion

        #region Override Methods

        /// <summary>
        /// Do cleanup here
        /// </summary>
        protected override void Cleanup()
        {
            try
            {
                FileInfo fileInfo = new FileInfo(m_Filename);
                if (fileInfo.Exists)
                {
                    fileInfo.Attributes = FileAttributes.Normal;
                    fileInfo.Delete();
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        #endregion

        #region Static Methods

        public static string GetTempFileName()
        {
            return Path.GetTempFileName();
        }

        #endregion
    }
}
