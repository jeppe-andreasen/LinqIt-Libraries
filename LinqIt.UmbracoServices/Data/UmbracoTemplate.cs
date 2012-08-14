using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using LinqIt.Cms.Data;
using umbraco.cms.businesslogic.web;

namespace LinqIt.UmbracoServices.Data
{
    public class UmbracoTemplate : Template
    {
        private readonly DocumentType _documentType;

        internal UmbracoTemplate(DocumentType documentType)
        {
            if (documentType == null)
                throw new ArgumentNullException("documentType");
            _documentType = documentType;
        }

        public override TemplateField[] AllFields
        {
            get 
            {
                var result = new List<TemplateField>();
                result.AddRange(BaseTemplates.SelectMany(t => t.AllFields));
                result.AddRange(OwnFields);
                return result.ToArray();
            }
        }

        public override Template[] BaseTemplates
        {
            get 
            { 
                if (_documentType.MasterContentType == 0)
                    return new Template[0];

                return new Template[]{ new UmbracoTemplate(new DocumentType(_documentType.MasterContentType))};
                
            }
        }

        public override string Description
        {
            get { return _documentType.Description; }
        }

        public override string DisplayName
        {
            get { return _documentType.Alias; }
        }

        public override Id Id
        {
            get { return new Id(_documentType.Id); }
        }

        public override string Name
        {
            get { return _documentType.Alias; }
        }

        public override TemplateField[] OwnFields
        {
            get
            {
                var result = new List<TemplateField>();
                foreach (var propertyType in _documentType.PropertyTypes)
                {
                    var field = new TemplateField();
                    field.Description = propertyType.Description;
                    field.DisplayName = propertyType.Name;
                    field.IsRequired = propertyType.Mandatory;
                    field.Name = propertyType.Alias;
                    var tab = _documentType.getVirtualTabs.Where(t => t.Id == propertyType.TabId).FirstOrDefault();
                    field.Section = tab != null ? tab.Caption : string.Empty;
                    field.Type = propertyType.DataTypeDefinition.DataType.DataTypeName;
                    result.Add(field);
                }
                return result.ToArray();
            }
        }

        public override string Path
        {
            get
            {
                var result = string.Empty;
                var parent = BaseTemplates.FirstOrDefault();
                if (parent != null)
                    result += parent.Path;
                result += "/" + Name;
                return result;
            }
        }

        public override void AddField<T>(string section, string name, bool isMultilingual, string settings)
        {
            throw new NotImplementedException();
        }

        public override void AddField<T>(string section, string name, bool isMultilingual, string settings, T standardValue)
        {
            throw new NotImplementedException();
        }

        public override void AddField(string section, string name, string type, bool isMultilingual, string settings, string standardValue)
        {
            throw new NotImplementedException();
        }

        public override string IconUrl
        {
            get { return "/umbraco/images/umbraco/" + _documentType.IconUrl.Replace(".sprTree", ""); }
        }

        public override DateTime? CreatedOn
        {
            get { return _documentType.CreateDateTime; }
        }

        public override DateTime? ModifiedOn
        {
            get { return null; }
        }
    }
}
