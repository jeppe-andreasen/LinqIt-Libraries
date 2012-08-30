//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Web;
//using LinqIt.Utils;

//namespace LinqIt.Cms.Data.DataIterators
//{
//    public class TemplateIterator : DataIterator
//    {
//        private int _index = -1;
//        private readonly Template[] _templates;
//        public TemplateIterator(DateTime from, DateTime to) : base(from, to)
//        {
//            _templates = CmsService.Instance.GetTemplates().Where(t => ShouldDeploy(t, from, to)).ToArray();
//        }        
        
//        private static bool ShouldDeploy(Template template, DateTime from, DateTime to)
//        {
//            var modifiedOn = template.ModifiedOn; 
//            if (!modifiedOn.HasValue)                
//                return true; 
//            return modifiedOn.Value >= from && modifiedOn.Value <= to;
//        }

//        protected internal override void RenderCurrent(System.Xml.XmlWriter writer)
//        {
//            var template = _templates[_index]; writer.WriteStartElement("template"); 
//            writer.WriteAttributeString("path", template.Path); 
//            foreach (var parent in template.BaseTemplates) 
//                writer.WriteElementString("parent", parent.Path); 
//            writer.WriteStartElement("fields"); 
//            foreach (var field in template.OwnFields)
//            {
//                writer.WriteStartElement(("field")); 
//                writer.WriteAttributeString("name", field.Name); 
//                if (!string.IsNullOrEmpty(field.DisplayName) && field.DisplayName != field.Name)                    
//                    writer.WriteAttributeString("displayName", field.DisplayName); 
//                writer.WriteAttributeString("type", field.Type); 
//                writer.WriteEndElement();
//            } 
//            writer.WriteEndElement(); // fields           
//            writer.WriteEndElement(); // template        
//        }

//        protected internal override bool ReadNext()
//        {
//            if (_index < _templates.Length - 1)
//            {
//                _index++; return true;
//            } 
//            return false;
//        }

//        protected internal override string ItemType { get { return "templates"; } }
//    }
//}