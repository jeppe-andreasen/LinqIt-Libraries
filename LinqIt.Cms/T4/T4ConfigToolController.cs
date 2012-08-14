using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using LinqIt.Cms.Configuration;
using LinqIt.Cms.Data;

namespace LinqIt.Cms.T4
{
    

    public class T4ConfigToolController
    {
        #region Fields

        private CodeGenerationConfiguration _generationConfiguration;

        #endregion Fields

        #region Properties

        public CodeGenerationConfiguration GenerationConfiguration
        {
            get
            {
                if (_generationConfiguration == null)
                    _generationConfiguration = CodeGenerationSettings.Instance.GetConfiguration(SelectedConfiguration);
                return _generationConfiguration;
            }
        }

        public IEnumerable<LinqIt.Cms.Configuration.CodeGenerationConfiguration.TemplateSetting> SearchTemplates
        {
            get
            {
                return GenerationConfiguration.GetTemplateSettings();
            }
        }

        public string SelectedConfiguration
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public IEnumerable<string> GetConfigurations()
        {
            return CodeGenerationSettings.Instance.Configurations;
        }

        public LinqIt.Cms.Configuration.CodeGenerationConfiguration.TemplateSetting GetTemplate(string templateId)
        {
            if (string.IsNullOrEmpty(templateId))
                return null;

            return GenerationConfiguration.GetTemplateSettings().Where(t => t.Id == new Id(templateId)).FirstOrDefault();
        }

        #endregion Methods
    }
}