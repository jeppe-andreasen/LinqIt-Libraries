using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class LocalizationAttribute : Attribute
    {
        #region Constructors

        public LocalizationAttribute(string path)
        {
            Path = path;
        }

        #endregion Constructors

        #region Properties

        public string ExcludeControls
        {
            get; set;
        }

        public string Path
        {
            get; private set;
        }

        #endregion Properties
    }
}