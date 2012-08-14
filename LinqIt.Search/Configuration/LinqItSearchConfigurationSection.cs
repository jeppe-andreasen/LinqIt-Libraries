using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public class LinqItSearchConfigurationSection : ConfigurationSection
    {
        [ConfigurationProperty("provider")]
        public string Provider
        {
            get { return (string)this["provider"]; }
        }


        [ConfigurationProperty("Filters")]
        public IFilterConfigurationCollection FilterConfigurations
        {
            get
            {
                return this["Filters"] as IFilterConfigurationCollection;
            }
        }

        [ConfigurationProperty("UrlReplacements")]
        public UrlReplacementConfigurationCollection UrlReplacements
        {
            get
            {
                return this["UrlReplacements"] as UrlReplacementConfigurationCollection;
            }
        }

        [ConfigurationProperty("IndexExpressions")]
        public RegularExpressionConfigurationCollection IndexExpressions
        {
            get
            {
                return this["IndexExpressions"] as RegularExpressionConfigurationCollection;
            }
        }

        [ConfigurationProperty("NoIndexExpressions")]
        public RegularExpressionConfigurationCollection NoIndexExpressions
        {
            get
            {
                return this["NoIndexExpressions"] as RegularExpressionConfigurationCollection;
            }
        }

        [ConfigurationProperty("FollowExpressions")]
        public RegularExpressionConfigurationCollection FollowExpressions
        {
            get
            {
                return this["FollowExpressions"] as RegularExpressionConfigurationCollection;
            }
        }

        [ConfigurationProperty("NoFollowExpressions")]
        public RegularExpressionConfigurationCollection NoFollowExpressions
        {
            get
            {
                return this["NoFollowExpressions"] as RegularExpressionConfigurationCollection;
            }
        }

        [ConfigurationProperty("StartUrls")]
        public SimpleConfigurationCollection StartUrls 
        {
            get { return this["StartUrls"] as SimpleConfigurationCollection; } 
        }
    }
}
