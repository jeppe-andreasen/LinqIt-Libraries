using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;

namespace LinqIt.Cms.Data.DataInstallers
{
    public abstract class DataInstaller
    {
        protected internal abstract void Install(XmlDocument data, StringBuilder log);
    }
}
