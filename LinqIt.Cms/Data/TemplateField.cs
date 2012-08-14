namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TemplateField
    {
        #region Properties

        public TemplateFieldCodeGenerationSettings CodeGenerationSettings
        {
            get; set;
        }

        public string Description
        {
            get; set;
        }

        public string DisplayName
        {
            get; set;
        }

        public bool IsRequired
        {
            get; set;
        }

        public string Name
        {
            get; set;
        }

        public string Section
        {
            get; set;
        }

        public string ToolTip
        {
            get; set;
        }

        public string Type
        {
            get; set;
        }

        #endregion Properties
    }
}