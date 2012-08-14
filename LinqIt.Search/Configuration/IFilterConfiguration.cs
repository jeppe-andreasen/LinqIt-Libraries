using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public class IFilterConfiguration : ConfigurationElementCollection
    {
        [ConfigurationProperty("mimetype", IsRequired = true)]
        public string MimeType
        {
            get
            {
                return this["mimetype"] as string;
            }
        }

        [ConfigurationProperty("ext", IsRequired = false)]
        public string Extension
        {
            get
            {
                return this["ext"] as string;
            }
        }

        [ConfigurationProperty("type", IsRequired = true)]
        public string Type
        {
            get
            {
                return this["type"] as string;
            }
        }

        public StripConfigurationElement this[int index]
        {
            get
            {
                return base.BaseGet(index) as StripConfigurationElement;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new StripConfigurationElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as StripConfigurationElement).Type;
        }
    
    }
}
