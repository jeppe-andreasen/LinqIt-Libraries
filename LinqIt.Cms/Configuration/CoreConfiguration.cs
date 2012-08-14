using System.Configuration;

namespace LinqIt.Cms.Configuration
{
    

    public class CmsConfiguration : ConfigurationSection
    {
        #region Properties

        [ConfigurationProperty("CmsServiceProvider", IsRequired=true)]
        public string CmsServiceProvider
        {
            get { return this["CmsServiceProvider"].ToString(); }
            set { this["CmsServiceProvider"] = value; }
        }

        [ConfigurationProperty("TypeTable", IsRequired = true)]
        public string TypeTable
        {
            get { return this["TypeTable"].ToString(); }
            set { this["TypeTable"] = value; }
        }

        #endregion Properties
    }
}