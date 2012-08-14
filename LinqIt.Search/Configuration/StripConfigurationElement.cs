using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public enum StripTextConfigurationType { index, follow };

    public class StripConfigurationElement : ConfigurationElement
    {
        [ConfigurationProperty("type", IsRequired = true)]
        public StripTextConfigurationType Type
        {
            get
            {
                return (StripTextConfigurationType)this["type"];
            }
        }

        [ConfigurationProperty("starttag", IsRequired = true)]
        public string StartTag
        {
            get
            {
                return this["starttag"] as string;
            }
        }

        [ConfigurationProperty("endtag", IsRequired = true)]
        public string EndTag
        {
            get
            {
                return this["endtag"] as string;
            }
        }
    }
}
