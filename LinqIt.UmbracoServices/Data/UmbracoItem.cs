using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Cms;
using LinqIt.Utils.Extensions;
using umbraco.cms.businesslogic.web;
using umbraco.NodeFactory;

namespace LinqIt.UmbracoServices.Data
{
    public abstract class UmbracoItem
    {
        public abstract string this[string fieldname] { get; }

        public static UmbracoItem Get(object umbracoObject)
        {
            if (umbracoObject == null)
                return null;
            if (umbracoObject is Node)
                return new UmbracoNode((Node)umbracoObject);
            if (umbracoObject is Document)
                return new UmbracoDocument((Document) umbracoObject);
            return null;
        }

        internal static UmbracoItem Get(int id)
        {
            if (CmsContext.Current.StoreType == CmsStoreType.Published || id == -1)
            {
                var node = new Node(id);
                return node.Id == id ? new UmbracoNode(node) : null;
            }
            else if (CmsContext.Current.StoreType == CmsStoreType.Working && id != 0)
            {
                var document = new Document(id);
                if (document.IsTrashed)
                    return null;
                return document.Id == id ? new UmbracoDocument(document) : null;
            }
            return null;
        }

        internal static UmbracoItem Get(string path)
        {
            if (CmsContext.Current.StoreType == CmsStoreType.Working)
                return UmbracoDocument.FromPath(path);
            else
                return UmbracoNode.FromPath(path);            
        }

        internal static UmbracoItem Current
        {
            get
            {
                if (CmsContext.Current.StoreType == CmsStoreType.Working)
                {
                    var document = new Document(Node.GetCurrent().Id);
                    return new UmbracoDocument(document);
                }
                else
                {
                    var node = umbraco.NodeFactory.Node.GetCurrent();
                    return node != null ? new UmbracoNode(node) : null;
                }
            }
        }

        internal abstract IEnumerable<UmbracoItem> Children { get; }

        internal abstract UmbracoItem Parent { get; }

        internal abstract string Name { get;  }

        internal abstract string IdPath { get; }

        internal string Path
        {
            get
            {
                var parent = Parent;
                if (parent == null)
                    return "/Content/" + Name; 
                else
                    return parent.Path + "/" + Name;

                //return "/" + IdPath.Split(',').Select(n => Get(Convert.ToInt32(n)).Name ?? "Content").ToSeparatedString("/");
            }
        }

        internal abstract int Id { get; }

        internal abstract DocumentType DocumentType { get; }



        public abstract string Url { get; }
    }

    public class UmbracoNode : UmbracoItem
    {
        private readonly Node _node;

        internal UmbracoNode(Node node)
        {
            _node = node;
        }

        public override string this[string fieldname]
        {
            get
            {
                var property = _node.GetProperty(fieldname); 
                return property != null? property.Value : null;
            }
        }

        internal override IEnumerable<UmbracoItem> Children
        {
            get { return _node.Children.Cast<Node>().Select(n => new UmbracoNode(n)); }
        }

        internal override UmbracoItem Parent
        {
            get
            {
                var parent = (Node)_node.Parent;
                if (parent == null || parent.Id == -1)
                    return null;
                return new UmbracoNode(parent);
            }
        }

        internal override string Name
        {
            get { return _node.Name; }
        }

        internal override string IdPath
        {
            get { return _node.Path; }
        }

        internal override int Id
        {
            get { return _node.Id; }
        }

        internal override DocumentType DocumentType
        {
            get { return DocumentType.GetByAlias(_node.NodeTypeAlias); }
        }

        internal static UmbracoNode FromPath(string path)
        {
            var parts = path.Trim('/').Split('/');
            var result = new Node(-1);
            for (var i = 1; i < parts.Length; i++)
            {
                result = result.Children.Cast<Node>().Where(c => c.Name == parts[i]).FirstOrDefault();
                if (result == null)
                    return null;
            }
            return new UmbracoNode(result);
        }

        public override string Url
        {
            get { return _node.NiceUrl; }
        }
    }

    public class UmbracoDocument : UmbracoItem
    {
        private readonly Document _document;

        internal UmbracoDocument(Document document)
        {
            _document = document; 
        }

        public override string this[string fieldname]
        {
            get
            {
                if (fieldname == null)
                    return null;
                var property = _document.getProperty(fieldname);
                return property != null? Convert.ToString(property.Value) : null;
            }
        }

        internal override IEnumerable<UmbracoItem> Children
        {
            get { return _document.Children.Select(c => new UmbracoDocument(c)); }
        }

        internal override UmbracoItem Parent
        {
            get
            {
                if (_document.ParentId < 0)
                    return null;
                var parent = new Document(_document.ParentId);
                return parent != null ? new UmbracoDocument(parent) : null;
            }
        }

        internal override string Name
        {
            get { return _document.Text; }
        }

        internal override string IdPath
        {
            get { return _document.Path; }
        }

        internal override int Id
        {
            get { return _document.Id; }
        }

        internal override DocumentType DocumentType
        {
            get { return DocumentType.GetByAlias(_document.ContentType.Alias); }
        }

        internal static UmbracoDocument FromPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            var parts = path.Trim('/').Split('/');
            if (parts.Length < 2)
                return null;
            var result = Document.GetRootDocuments().Where(d => d.Text == parts[1]).FirstOrDefault();
            if (result == null)
                return null;
            for (var i = 2; i < parts.Length; i++)
            {
                result = result.Children.Where(d => d.Text == parts[i]).FirstOrDefault();
                if (result == null)
                    return null;
            }
            return new UmbracoDocument(result);
        }

        public override string Url
        {
            get { return string.Empty; }
        }
    }
}
