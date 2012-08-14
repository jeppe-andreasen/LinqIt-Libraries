using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search.Configuration
{
    public class UrlReplacementConfigurationCollection : GenericElementCollection<RegularExpressionReplamentConfiguration>
    {
        protected override object GetKey(RegularExpressionReplamentConfiguration element)
        {
            return element.Pattern;
        }
    }
}
