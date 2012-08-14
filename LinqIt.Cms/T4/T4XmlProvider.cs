using System.Web.Services;

namespace LinqIt.Cms.T4
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    
    using System.Xml;

    using LinqIt.Cms.Configuration;
    
    /// <summary>
    /// Summary description for $codebehindclassname$
    /// </summary>
    [WebService(Namespace = "http://tempuri.org/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class T4XmlProvider : IHttpHandler
    {
        #region Fields

        private CodeGenerationConfiguration _configuration;
        private Dictionary<string, CodeGenerationConfiguration.TemplateSetting> _templates;

        #endregion Fields

        #region Properties

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        #endregion Properties

        #region Methods

        public void ProcessRequest(HttpContext context)
        {
            string configName = context.Request.QueryString["configuration"];

            _configuration = CodeGenerationSettings.Instance.GetConfiguration(configName);
            context.Response.ContentType = "text/plain";
            using (XmlTextWriter writer = new XmlTextWriter(context.Response.OutputStream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
                writer.WriteStartElement("namespaces");
                _templates = _configuration.GetTemplateSettings().ToDictionary(t => t.Path);

                foreach (var nameSpace in _templates.Values.GroupBy(c => c.NameSpace))
                {
                    writer.WriteStartElement("namespace");
                    writer.WriteAttributeString("name", nameSpace.Key);

                    foreach (var template in nameSpace)
                        WriteTemplate(template, writer);

                    writer.WriteEndElement();
                }
                writer.WriteEndElement();
            }
        }

        private void WriteTemplate(CodeGenerationConfiguration.TemplateSetting template, XmlTextWriter writer)
        {
            writer.WriteStartElement("template");
            writer.WriteAttributeString("name", template.Name);
            writer.WriteAttributeString("id", template.Id.ToString());
            writer.WriteAttributeString("base", template.BaseClass);
            writer.WriteAttributeString("csname", template.ClassName);
            writer.WriteAttributeString("access", template.AccessModifier);
            writer.WriteAttributeString("path", template.Path);

            writer.WriteStartElement("sections");

            foreach (var section in template.FieldSettings.Values.GroupBy(f => f.Section))
            {
                writer.WriteStartElement("section");
                writer.WriteAttributeString("name", section.Key);

                foreach (var field in section)
                {
                    writer.WriteStartElement("field");
                    writer.WriteAttributeString("name", field.Name);
                    writer.WriteAttributeString("csname", field.PropertyName);
                    writer.WriteAttributeString("cstype", field.CsType);
                    writer.WriteAttributeString("method", field.Method);
                    writer.WriteAttributeString("access", field.AccessModifier);
                    writer.WriteAttributeString("typeTable", field.TypeTable);
                    writer.WriteEndElement(); // field
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement(); // fields
            writer.WriteEndElement(); // template
        }

        #endregion Methods
    }
}