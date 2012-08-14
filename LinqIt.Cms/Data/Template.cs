using System;

namespace LinqIt.Cms.Data
{
    using System.Linq;

    public abstract class Template
    {
        

        #region Properties

        public abstract TemplateField[] AllFields
        {
            get;
        }

        public abstract Template[] BaseTemplates
        {
            get;
        }

        public abstract string Description
        {
            get;
        }

        public abstract string DisplayName
        {
            get;
        }

        public abstract Id Id
        {
            get;
        }

        public abstract string Name
        {
            get;
        }

        public abstract TemplateField[] OwnFields
        {
            get;
        }

        public abstract string Path
        {
            get;
        }

        public abstract DateTime? CreatedOn { get; }

        public abstract DateTime? ModifiedOn { get; }

        #endregion Properties

        #region Methods

        public abstract void AddField<T>(string section, string name, bool isMultilingual, string settings);

        public abstract void AddField<T>(string section, string name, bool isMultilingual, string settings, T standardValue);

        public abstract void AddField(string section, string name, string type, bool isMultilingual, string settings, string standardValue);

        public bool Is(string templateName)
        {
            if (string.Compare(Name, templateName, true) == 0)
                return true;

            return BaseTemplates.Any(template => template.Is(templateName));
        }

        #endregion Methods

        public abstract string IconUrl { get; }
    }



    //public class Template
    //{
    //    #region Properties

    //    public TemplateField[] AllFields
    //    {
    //        get; set;
    //    }

    //    public Template[] BaseTemplates
    //    {
    //        get; set;
    //    }

    //    public string Description
    //    {
    //        get; set;
    //    }

    //    public string DisplayName
    //    {
    //        get; set;
    //    }

    //    public Id Id
    //    {
    //        get; set;
    //    }

    //    public string Name
    //    {
    //        get; set;
    //    }

    //    public TemplateField[] OwnFields
    //    {
    //        get; set;
    //    }

    //    public string Path
    //    {
    //        get; set;
    //    }

    //    #endregion Properties

    //    #region Methods

    //    public void AddBaseTemplate(Template baseTemplate)
    //    {
    //        CmsService.Instance.AddBaseTemplate(this, baseTemplate);
    //    }

    //    public void AddField<T>(string section, string name, bool isMultilingual, string settings)
    //    {
    //        string type = CodeGenerationSettings.Instance.GetCsTypeConversion<T>();
    //        CmsService.Instance.AddTemplateField(this, section, name, type, isMultilingual, settings, null);
    //    }

    //    public void AddField<T>(string section, string name, bool isMultilingual, string settings, T standardValue)
    //    {
    //        string type = CodeGenerationSettings.Instance.GetCsTypeConversion<T>();
    //        string value = CmsService.Instance.ConvertToString(standardValue);
    //        CmsService.Instance.AddTemplateField(this, section, name, type, isMultilingual, settings, value);
    //    }

    //    public void AddField(string section, string name, string type, bool isMultilingual, string settings, string standardValue)
    //    {
    //        CmsService.Instance.AddTemplateField(this, section, name, type, isMultilingual, settings, standardValue);
    //    }

    //    public bool Is(string templateName)
    //    {
    //        if (string.Compare(Name, templateName, true) == 0)
    //            return true;

    //        foreach (var template in BaseTemplates)
    //        {
    //            if (template.Is(templateName))
    //                return true;
    //        }
    //        return false;
    //    }

    //    #endregion Methods
    //}
}