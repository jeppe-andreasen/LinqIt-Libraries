using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public class RegularExpressionReplamentConfiguration : ConfigurationElement
    {
        [ConfigurationProperty("pattern", IsRequired = true)]
        public string Pattern
        {
            get
            {
                return this["pattern"] as string;
            }
        }

        [ConfigurationProperty("replacement", IsRequired = true)]
        public string Replacement
        {
            get
            {
                return this["replacement"] as string;
            }
        }



    }
}
