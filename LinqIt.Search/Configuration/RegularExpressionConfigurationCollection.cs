using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public class RegularExpressionConfigurationCollection : ConfigurationElementCollection
    {
        public RegularExpressionConfiguration this[int index]
        {
            get
            {
                return base.BaseGet(index) as RegularExpressionConfiguration;
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
            return new RegularExpressionConfiguration();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return (element as RegularExpressionConfiguration).Value;
        }
    }
}
