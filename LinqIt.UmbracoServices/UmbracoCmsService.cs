using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Xml;
using System.Xml.Linq;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.UmbracoServices.Data;
using LinqIt.Utils;
using LinqIt.Utils.Extensions;
using umbraco.BusinessLogic;
using umbraco.cms.businesslogic.web;
using umbraco.interfaces;
using umbraco.NodeFactory;

namespace LinqIt.UmbracoServices
{
    public class UmbracoCmsService : CmsService
    {
        public const int SiteLevel = 4;
        public const int CompanyLevel = 3;
        public const int ContentLevel = 2;

        protected override T GetFieldValue<T>(Cms.Data.Entity entity, string fieldName)
        {
            var value = ((UmbracoItem)entity.WrappedItem)[fieldName];
            object result = null;

            var returnType = typeof(T);
            if (returnType == typeof(bool))
            {
                result = value == "1";
                return (T) result;
            }
            if (returnType == typeof(string))
                return (T)(object)Convert.ToString(value);
            if (returnType == typeof(Image))
            {
                if (string.IsNullOrEmpty(value))
                    return (T)(object)new Image();
                var mediaId = Convert.ToInt32(value);
                var media = new umbraco.cms.businesslogic.media.Media(mediaId);
                var mediaUrl = media.GenericProperties[0].Value.ToString();
                result = new Image(new Id(mediaId), mediaUrl, string.Empty);
                return (T)result;
            }
            if (returnType == typeof(Text))
            {
                result = new Text(Convert.ToString(value));
                return (T)result;
            }
            if (returnType == typeof(Html))
            {
                result = new Html(Convert.ToString(value));
                return (T)result;
            }
            if (returnType == typeof(Link))
            {
                result = ParseLink(value);
                return (T)result;
            }
            if (returnType == typeof(LinkList))
            {
                result = ParseLinkList(value);
                return (T)result;
            }
            if (returnType == typeof(DateTime?))
            {
                if (!string.IsNullOrEmpty(value))
                    result = DateTime.ParseExact(value, "yyyy-MM-ddTHH:mm:ss", System.Globalization.CultureInfo.InvariantCulture);
                return (T)result;
            }
            if (returnType == typeof(int?))
            {
                int n;
                if (int.TryParse(value, out n))
                    result = n;
                return (T) result;
            }

            var constructor = typeof(T).GetConstructor(new[] { typeof(string) });
            if (constructor != null)
            {
                result = constructor.Invoke(new[] { value });
                return (T)result;
            }


            return default(T);
        }



        public override Uri CurrentUrl
        {
            get { return HttpContext.Current.Request.Url; }
        }

        public override string CurrentLanguage
        {
            get { throw new NotImplementedException(); }
        }

        public override string SitePath
        {
            get
            {
                var current = (UmbracoItem)GetItem();
                if (current == null)
                    return null;
                var nodeId = Convert.ToInt32(current.IdPath.Split(',')[2]);
                return UmbracoItem.Get(nodeId).Path;
            }
        }

        public override string HomeItemPath
        {
            get
            {
                var current = (UmbracoItem)GetItem();
                if (current == null)
                    return null;
                var nodeId = Convert.ToInt32(current.Path.Split(',')[3]);
                return UmbracoItem.Get(nodeId).Path;
            }
        }

        protected override object CreateEntity(string name, object parent, Cms.Data.Id templateId)
        {
            var documentType = new DocumentType(templateId.IntValue);
            var author = User.GetUser(0);
            var document = Document.MakeNew(name, documentType, author, ((Node) parent).Id);
            document.Publish(author);
            umbraco.library.UpdateDocumentCache(document.Id);
            return new Node(document.Id);
        }

        public override Cms.Data.Template CreateTemplate(string name, string path)
        {
            throw new NotImplementedException();
        }

        protected override object GetItem(Cms.Data.Id itemId)
        {
            return UmbracoItem.Get(itemId.IntValue);
        }

        protected override object GetItem(string itemPath)
        {
            return UmbracoItem.Get(itemPath);
        }

        protected override object GetItem()
        {
            return !string.IsNullOrEmpty(CmsContext.Current.Path) ? GetItem(CmsContext.Current.Path) : UmbracoItem.Current;
        }

        protected override IEnumerable<object> GetChildItems(object item)
        {
            return ((UmbracoItem) item).Children;
        }

        protected override IEnumerable<object> SelectItems(string query)
        {
            throw new NotImplementedException();
        }

        public override Cms.Data.Device CurrentDevice
        {
            get { throw new NotImplementedException(); }
        }

        public override Cms.Data.Device[] Devices
        {
            get { throw new NotImplementedException(); }
        }

        protected override object GetParent(object item)
        {
            return ((UmbracoItem) item).Parent;
        }

        public override Cms.Data.File GetFile(Cms.Data.Id id)
        {
            throw new NotImplementedException();
        }

        public override string GetHostName()
        {
            var item = (UmbracoItem)GetItem();
            var homeId = Convert.ToInt32(item.IdPath.Split(',')[3]);
            return umbraco.library.GetCurrentDomains(homeId).Select(n => n.Name).FirstOrDefault();
        }

        public override string GetSystemPath(string systemLinkKey)
        {
            return GetSystemPath(systemLinkKey, SitePath);
        }

        public override List<Cms.Data.Id> GetSelectedMenuIds()
        {
            return umbraco.NodeFactory.Node.GetCurrent().Path.Split(',').Skip(3).Select(s => new Id(Convert.ToInt32(s))).ToList();
        }

        public override IEnumerable<Cms.Data.Template> GetTemplates()
        {
            return umbraco.cms.businesslogic.web.DocumentType.GetAllAsList().Select(d => new UmbracoTemplate(d));
        }

        protected override void AddBaseTemplate(Cms.Data.Template template, Cms.Data.Template baseTemplate)
        {
            throw new NotImplementedException();
        }

        protected override void AddTemplateField(Cms.Data.Template template, string section, string name, string type, bool isMultilingual, string settings, string standardValue)
        {
            throw new NotImplementedException();
        }

        protected override string ConvertToString(object value)
        {
            throw new NotImplementedException();
        }

        protected override bool GetBool(Cms.Data.Entity entity, string fieldName)
        {
            throw new NotImplementedException();
        }

        protected override string GetDisplayName(Cms.Data.Entity entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            return ((UmbracoItem) entity.WrappedItem).Name;
        }

        protected override System.Collections.Specialized.NameValueCollection GetFields(Cms.Data.Entity entity)
        {
            throw new NotImplementedException();
        }

        

        protected override string GetHostName(Cms.Data.Page page)
        {
            throw new NotImplementedException();
        }

        protected override Cms.Data.Id GetId(Cms.Data.Entity entity)
        {
            return new Id(((UmbracoItem) entity.WrappedItem).Id);
        }

        protected override string GetImageUrl(Cms.Data.Image image, Cms.Data.ImageSize imageSize)
        {
            throw new NotImplementedException();
        }

        protected override string GetName(Cms.Data.Entity entity)
        {
            return ((UmbracoItem) entity.WrappedItem).Name;
        }

        public override string GetPath(Id itemId)
        {
            var item = UmbracoItem.Get(itemId.IntValue);
            return item.Path;
        }

        protected override string GetPath(Cms.Data.Entity entity)
        {
            return ((UmbracoItem) entity.WrappedItem).Path;
        }

        protected override Cms.Data.Template GetTemplate(Cms.Data.Entity entity)
        {
            var item = ((UmbracoItem) entity.WrappedItem);
            var documentType = item.DocumentType;
            return new UmbracoTemplate(documentType);
        }

        protected override string GetUrl(Cms.Data.Entity page)
        {
            if (page == null)
                return null;

            //var homeItem = GetHomeItem(page);
            //var result = new StringBuilder();
            return ((UmbracoItem) page.WrappedItem).Url;
        }

        protected override bool HasVersion(Cms.Data.Entity entity)
        {
            throw new NotImplementedException();
        }

        protected override bool SaveItem(Cms.Data.Entity entity)
        {
            Document doc = new Document(entity.Id.IntValue);
            doc.Save();
            return true;
        }

        protected override bool TemplateIs(Cms.Data.Entity entity, string templateName)
        {
            throw new NotImplementedException();
        }

        protected override void SetEntityFieldValue(Cms.Data.Entity entity, string fieldName, string value)
        {
            throw new NotImplementedException();
        }

        public override void Log(string message, Cms.Logging.LogType logType)
        {
            throw new NotImplementedException();
        }

        protected override Cms.Data.Link ParseLink(string value)
        {
            int id;
            if (int.TryParse(value, out id))
            {
                var page = GetItem<Page>(new Id(id));
                return new Link(page);
            }
            return new Link();
        }

        protected override Cms.Data.LinkList ParseLinkList(string value)
        {
            var result = new LinkList();
            if (string.IsNullOrEmpty(value))
                return result;
            XDocument doc;
            try
            {
                doc = XDocument.Parse(value);
                foreach (XElement linkElement in doc.Root.Elements("link"))
                {
                    var link = new Link();
                    link.Title = (string)linkElement.Attribute("title");

                    var type = ((string)linkElement.Attribute("type"));
                    if (type == "internal")
                    {
                        link.LinkType = LinkType.Internal;
                        link.LinkedItemId = new Id(Convert.ToInt32((string)linkElement.Attribute("link")));
                        link.Href = GetUrl(GetItem<Entity>(link.LinkedItemId));
                    }
                    else if (type == "external")
                    {
                        link.LinkType = LinkType.External;
                        link.Href = (string)linkElement.Attribute("link");
                    }
                    if (((string)linkElement.Attribute("newwindow")) == "1")
                        link.Target = "_blank";
                    result.Add(link);
                }
                
            }
            catch
            {
            }
            return result;
            
        }

        protected override bool HasValue(Cms.Data.Entity entity, string key)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<T> GetFieldValues<T>(Cms.Data.Entity entity, string fieldName)
        {
            throw new NotImplementedException();
        }

        protected override string GetTemplatePath(object item)
        {
            throw new NotImplementedException();
        }

        protected override object GetAnchestorOfType(object item, string templatePath)
        {
            throw new NotImplementedException();
        }

        public override void ResolveItem(string url, CmsContext context)
        {
            throw new NotImplementedException();
        }

        public override string ResolveSitePath(string url, out string language)
        {
            throw new NotImplementedException();
        }

        public override string ResolveImagePath(string imageUrl)
        {
            throw new NotImplementedException();
        }

        protected override void PublishItem(Cms.Data.Entity entity)
        {
            throw new NotImplementedException();
        }

        public override Cms.Data.UserAccount GetUser(string username)
        {
            throw new NotImplementedException();
        }

        public override Cms.Data.UserAccount GetUserByEmail(string domain, string email)
        {
            throw new NotImplementedException();
        }

        public override Cms.Data.UserAccount GetCurrentUser()
        {
            return new UmbracoUserAccount(User.GetUser(0));
        }

        public override Cms.Data.UserAccount CreateUser(string domain, string email, string password)
        {
            throw new NotImplementedException();
        }

        protected override Stack<CmsContext> GetCmsContextStack()
        {
            var result = (Stack<CmsContext>)HttpContext.Current.Items["CmsContextStack"];
            if (result == null)
            {
                result = new Stack<CmsContext>();
                HttpContext.Current.Items["CmsContextStack"] = result;
            }
            return result;
        }

        public override string GetSystemPath(string systemLinkKey, string relativePath)
        {
            var lookup = GetSiteSystemPaths(relativePath);
            if (lookup.ContainsKey(systemLinkKey))
                return lookup[systemLinkKey];
            return null;
        }

        private Dictionary<string, string> GetSiteSystemPaths(string relativePath)
        {
            var result = new Dictionary<string, string>();

            for (var i = SiteLevel; i >= ContentLevel; i--)
            {
                var siteRootPath = PathUtil.Take(relativePath, i);
                var systemLinkFolderPath = PathUtil.Combine(siteRootPath, "Configuration", "SystemLinks");
                var systemLinkFolder = (UmbracoItem)GetItem(systemLinkFolderPath);
                if (systemLinkFolder == null)
                    continue;

                foreach (var systemLink in systemLinkFolder.Children)
                {
                    if (result.ContainsKey(systemLink.Name))
                        continue;
                    var link = systemLink["link"];
                    if (string.IsNullOrEmpty(link))
                        continue;

                    var linkedItem = UmbracoItem.Get(Convert.ToInt32(link));
                    if (linkedItem != null)
                        result.Add(systemLink.Name, linkedItem.Path);
                }
            }
            return result;
        }

        public override Cms.Data.Image GetImage(Cms.Data.Id imageId)
        {
            throw new NotImplementedException();
        }

        protected override string GetInternalLink(object item)
        {
            throw new NotImplementedException();
        }

        protected override object GetConfigurationObject(string objectName, string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath))
                return null;
            for (var i = SiteLevel; i >= ContentLevel; i--)
            {
                var siteRootPath = PathUtil.Take(relativePath, i);
                var configurationItemPath = PathUtil.Combine(siteRootPath, "Configuration", objectName);
                var configurationItem = GetItem(configurationItemPath);
                if (configurationItem != null)
                    return configurationItem;
            }
            return null;
        }

        public override Cms.Data.Template GetTemplate(Cms.Data.Id id)
        {
            return new UmbracoTemplate(new DocumentType(id.IntValue));
        }

        protected override string GetIconUrl(Entity entity)
        {
            var docType = ((UmbracoItem) entity.WrappedItem).DocumentType;
            return "/umbraco/images/umbraco/" + docType.IconUrl.Replace(".sprTree", "");
        }

        private static UmbracoDataContext GetDbContext()
        {
            return new UmbracoDataContext(ConfigurationManager.AppSettings["umbracoDbDSN"]);   
        }

        public override void ChangeTemplate(Entity entity, Template template)
        {
            int documentId = entity.Id.IntValue;


            var doc = new Document(entity.Id.IntValue);
            var type = DocumentType.GetByAlias(doc.ContentType.Alias);

            var storedValues = new Dictionary<string, object>();
            foreach (var propertyType in type.PropertyTypes)
                storedValues[propertyType.Alias] = doc.getProperty(propertyType).Value;

            var context = GetDbContext();

            // Remove Field Data
            foreach (var data in context.cmsPropertyDatas.Where(d => d.contentNodeId == documentId))
                context.cmsPropertyDatas.DeleteOnSubmit(data);

            // Update Type
            var cmsContent = context.cmsContents.Where(c => c.nodeId == doc.Id).FirstOrDefault();
            cmsContent.contentType = template.Id.IntValue;

            // Update Type on xml content
            var xmlContent = context.cmsContentXmls.Where(c => c.nodeId == doc.Id).FirstOrDefault();
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(xmlContent.xml);
            xml.DocumentElement.Attributes["nodeType"].Value = template.Id.ToString();
            xmlContent.xml = xml.OuterXml;

            // Submit Data
            context.SubmitChanges();

            umbraco.library.UpdateDocumentCache(documentId);

            // Populate Stored Field Values
            doc = new Document(documentId);
            type = DocumentType.GetByAlias(doc.ContentType.Alias);
            foreach (string key in storedValues.Keys)
            {
                var propertyType = type.getPropertyType(key);
                if (propertyType == null)
                    continue;

                var property = doc.getProperty(propertyType) ?? doc.addProperty(propertyType, Guid.NewGuid());
                property.Value = storedValues[key];
            }
            doc.Save();
            umbraco.library.UpdateDocumentCache(documentId);
        }

        public override Page GetHomeItem()
        {
            var item = (UmbracoItem)GetItem();
            var homeId = Convert.ToInt32(item.IdPath.Split(',')[3]);
            return GetItem<Page>(new Id(homeId));
        }

        public Page GetHomeItem(string idPath)
        {
            var homeId = Convert.ToInt32(idPath.Split(',')[3]);
            return GetItem<Page>(new Id(homeId));
        }

        internal static XDocument SelectContentXml(int nodeId)
        {
            var db = GetDbContext();
            var item = db.cmsContentXmls.Where(n => n.nodeId == nodeId).FirstOrDefault();
            return item == null ? null : XDocument.Parse(item.xml);
        }
    }
}
