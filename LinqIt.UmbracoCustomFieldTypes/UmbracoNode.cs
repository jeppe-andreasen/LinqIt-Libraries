//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Web;
//using umbraco;
//using umbraco.cms.businesslogic.web;
//using umbraco.NodeFactory;
//using umbraco.presentation;

//namespace LinqIt.UmbracoCustomFieldTypes
//{
//    public abstract class UmbracoItem
//    {
//        public static UmbracoItem Get(string id)
//        {
//            var intId = Convert.ToInt32(id);
//            if (IsInCmsContext)
//                return new UmbracoDocument(intId);
//            else
//                return new UmbracoNode(intId);
//        }

//        public abstract string this[string key] { get; }

//        public static UmbracoItem GetCurrent()
//        {
//            if (IsInCmsContext)
//                return new UmbracoDocument();
//            else
//                return new UmbracoNode();
//        }

//        public static bool IsInCmsContext
//        {
//            get 
//            {
//                return (!GlobalSettings.RequestIsInUmbracoApplication(HttpContext.Current) && umbraco.presentation.UmbracoContext.Current.LiveEditingContext.Enabled);
//            }
//        }
//    }

//    public class UmbracoNode : UmbracoItem
//    {
//        private Node _node;

//        public UmbracoNode()
//        {
//            _node = Node.GetCurrent();
//        }

//        public UmbracoNode(int id)
//        {
//            _node = new Node(id);
//        }

//        public override string this[string key]
//        {
//            get { return _node.GetProperty(key).Value; }
//        }
//    }

//    public class UmbracoDocument : UmbracoItem
//    {
//        private Document _document;

//        public UmbracoDocument()
//        {
//            _document = new Document(Node.GetCurrent().Id);
//        }

//        public UmbracoDocument(int id)
//        {
//            _document = new Document(id);
//        }

//        public override string this[string key]
//        {
//            get { return _document.getProperty(key).Value.ToString(); }
//        }
//    }
//}
