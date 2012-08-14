namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class File
    {
        #region Constructors

        public File()
        {
            Url = null;
        }

        public File(string url)
        {
            Url = url;
        }

        #endregion Constructors

        #region Properties

        public bool Exists
        {
            get { return !string.IsNullOrEmpty(Url); }
        }

        public string Url
        {
            get; private set;
        }

        #endregion Properties
    }
}