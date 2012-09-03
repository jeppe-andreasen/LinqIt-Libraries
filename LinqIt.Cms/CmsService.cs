using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Xml;
using LinqIt.Cms.Data;
using LinqIt.Cms.Data.DataInstallers;
using LinqIt.Cms.Data.DataIterators;
using LinqIt.Utils;
using LinqIt.Utils.Caching;
using LinqIt.Utils.Extensions;
using Page = LinqIt.Cms.Data.Page;

namespace LinqIt.Cms
{
    public abstract class CmsService
    {
        #region Fields

        //private static readonly CmsConfiguration _configuration = (CmsConfiguration)ConfigurationManager.GetSection("LinqIt.Cms");

        //private readonly Regex _TranslationKeyRegex = new Regex("[a-z]+([A-Z][a-z]*)+");

        //private static readonly object _translationLock = new object();

        //private LinkTranslations _linkTranslations;

        //private EntityTypeTable _EntityNameTable;

        #endregion Fields

        #region Properties

        public static CmsService Instance
        {
            get
            {
                
                return Cache.Get("CmsService", CacheScope.Request, () =>
                {
                    var type = Type.GetType("LinqIt.UmbracoServices.UmbracoCmsService, LinqIt.UmbracoServices");
                    return (CmsService)Activator.CreateInstance(type);
                });
            }
        }

        public abstract Uri CurrentUrl
        {
            get;
        }

        public abstract string CurrentLanguage { get; }

        public abstract string SitePath
        {
            get;
        }

        public string GetSitePath(string relativePath)
        {
            return PathUtil.Take(relativePath, 4);
        }

        public string ConfigurationPath
        {
            get { return PathUtil.Combine(SitePath, "Configuration"); }
        }

        public string SystemLinksPath
        {
            get { return PathUtil.Combine(ConfigurationPath, "SystemLinks"); }
        }

        public abstract string HomeItemPath { get; }

        #endregion Properties

        #region Methods

        public T CreateEntity<T>(string name, Entity parent) where T : Entity, new()
        {
            var tmp = new T();
            var obj = CreateEntity(name, parent.WrappedItem, tmp.TemplateId);
            return obj != null ? GetItemWrapper<T>(obj) : null;
        }

        protected abstract object CreateEntity(string name, object parent, Id templateId);

        public abstract Template CreateTemplate(string name, string path);

        #region Get Item

        public T GetItem<T>()
            where T : Entity, new()
        {
            return GetItemWrapper<T>(GetItem());
        }

        public T GetItem<T>(Id itemId)
            where T : Entity, new()
        {
            return GetItemWrapper<T>(GetItem(itemId));
        }

        public T GetItem<T>(string path)
            where T : Entity, new()
        {
            return GetItemWrapper<T>(GetItem(path));
        }

        public T GetItemOfType<T, TE>()
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return GetItemWrapper<T>(GetItem(), new TE());
        }

        public T GetItemOfType<T, TE>(Id itemId)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return GetItemWrapper<T>(GetItem(itemId), new TE());
        }

        public T GetItemOfType<T, TE>(string path)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return GetItemWrapper<T>(GetItem(path), new TE());
        }

        protected abstract object GetItem(Id itemId);

        protected abstract object GetItem(string itemPath);

        protected abstract object GetItem();

        #endregion

        #region Get Child Items

        #region With Id
        public T[] GetChildren<T>(Id itemId)
            where T : Entity, new()
        {
            return WrapItems<T>(GetChildItems(GetItem(itemId)));
        }

        public T[] GetChildrenOfType<T>(Id itemId)
            where T : Entity, new()
        {
            return WrapItemsOfType<T>(GetChildItems(GetItem(itemId)));
        }

        public T[] GetChildrenOfType<T, TE>(Id itemId)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return WrapItemsOfType<T, TE>(GetChildItems(GetItem(itemId)));
        }
        #endregion

        #region With Path

        public T[] GetChildren<T>(string itemPath)
            where T : Entity, new()
        {
            return WrapItems<T>(GetChildItems(GetItem(itemPath)));
        }

        public T[] GetChildrenOfType<T>(string itemPath)
            where T : Entity, new()
        {
            return WrapItemsOfType<T>(GetChildItems(GetItem(itemPath)));
        }

        public T[] GetChildrenOfType<T, TE>(string itemPath)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return WrapItemsOfType<T, TE>(GetChildItems(GetItem(itemPath)));
        }

        #endregion

        #region With Entity

        internal T[] GetChildren<T>(Entity parent)
            where T : Entity, new()
        {
            return WrapItems<T>(GetChildItems(parent.WrappedItem));
        }

        internal T[] GetChildrenOfType<T>(Entity entity)
            where T : Entity, new()
        {
            return WrapItemsOfType<T>(GetChildItems(entity.WrappedItem));
        }

        internal T[] GetChildrenOfType<T, TE>(Entity entity)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return WrapItemsOfType<T, TE>(GetChildItems(entity.WrappedItem));
        }

        #endregion

        internal T[] GetDescendantsOfType<T>(Entity parent) where T : Entity, new()
        {
            var tmp = new T();
            var templatePath = tmp.TemplatePath;
            var items = IterationUtil.FindAllBFS(parent.WrappedItem, GetChildItems, i => GetTemplatePath(i) == templatePath);
            return WrapItems<T>(items);
        }

        internal T[] GetDescendantsOfType<T, TE>(Entity entity)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            var tmp = entity.WrappedItem;
            return WrapItemsOfType<T, TE>(IterationUtil.FindAllBFS(tmp, GetChildItems, i => true));
        }

        protected abstract IEnumerable<object> GetChildItems(object item);

        #endregion

        #region Get Items With Query

        public T SelectSingleItem<T>(string query) where T : Entity, new()
        {
            return GetItemWrapper<T>(SelectSingleItem(query));
        }

        public T SelectSingleItemOfType<T>(string query) where T : Entity, new()
        {
            var item = SelectSingleItem(query);
            if (item == null)
                return null;
            return GetTemplatePath(item) == new T().TemplatePath? GetItemWrapper<T>(item) : null;
        }

        public T SelectSingleItemOfType<T, TE>(string query)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            var typeTable = new TE();
            return GetItemWrapper<T>(SelectSingleItem(query), typeTable);
        }

        protected abstract object SelectSingleItem(string query);

        public T[] SelectItems<T>(string query) where T : Entity, new()
        {
            return WrapItems<T>(SelectItems(query));
        }

        public T[] SelectItemsOfType<T>(string query) where T : Entity, new()
        {
            return WrapItemsOfType<T>(SelectItems(query));
        }

        public T[] SelectItemsOfType<T, TE>(string query)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            return WrapItemsOfType<T, TE>(SelectItems(query));
        }

        protected abstract IEnumerable<object> SelectItems(string query);

        #endregion

        #region Get Items With Id List

        public T[] GetItems<T>(IEnumerable<Id> ids) where T : Entity, new()
        {
            if (ids == null || !ids.Any())
                return new T[0];
            return ids.Select(GetItem<T>).ToArray();
        }

        public abstract Device CurrentDevice { get; }

        public abstract Device[] Devices { get; }

        public T[] GetItemsOfType<T, TE>(IEnumerable<Id> ids)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            if (ids == null || !ids.Any())
                return new T[0];

            var result = WrapItemsOfType<T, TE>(ids.Select(GetItem));
            return result;
        }

        public T[] GetItemsOfType<T>(IEnumerable<Id> ids) where T : Entity, new()
        {
            if (ids == null || !ids.Any())
                return new T[0];

            var result = WrapItemsOfType<T>(ids.Select(GetItem));
            return result;
        }

        #endregion

        #region GetParent

        internal T GetParent<T>(Entity entity) where T : Entity, new()
        {
            return GetItemWrapper<T>(GetParent(entity.WrappedItem));
        }

        public T GetParentOfType<T>(Entity entity) where T : Entity, new()
        {
            var templatePath = new T().TemplatePath;
            var tmp = GetParent(entity.WrappedItem);
            while (tmp != null && GetTemplatePath(tmp) != templatePath)
                tmp = GetParent(tmp);
            return tmp != null ? GetItemWrapper<T>(tmp) : null;
        }

        protected abstract object GetParent(object item);

        #endregion

        public abstract File GetFile(Id id);

        public abstract string GetHostName();

        public virtual NameValueCollection GetLayoutParameters(UserControl userControl)
        {
            return null;
        }

        public abstract string GetSystemPath(string systemLinkKey);

        public abstract List<Id> GetSelectedMenuIds();

        public Template GetTemplate(string path)
        {
            return GetTemplates().Where(t => t.Path == path).FirstOrDefault();
        }

        public abstract IEnumerable<Template> GetTemplates();

        //public string GetTranslation(string folderName, string translationKey, string defaultText)
        //{
        //    var language = CmsContext.Current.Language;

        //    string cacheKey = language + ":" + folderName + "/" + translationKey;
        //    Dictionary<string, string> translations = Cache.Get("Translations", CacheScope.Request, () => new Dictionary<string, string>());
        //    if (!translations.ContainsKey(cacheKey))
        //    {
        //        string translation = this.FetchTranslation(folderName, translationKey, defaultText, language);
        //        lock (_translationLock)
        //        {
        //            if (!translations.ContainsKey(cacheKey))
        //                translations.Add(cacheKey, translation);
        //        }
        //        return translation;
        //    }
        //    return translations[cacheKey];
        //}

        //public abstract string FetchTranslation(string folderName, string translationKey, string defaultText, string language);



        //public void TranslateControl(Control control, string folder, List<string> excludedControls)
        //{
        //    foreach (Control child in control.Controls)
        //    {
        //        if (!IsSelfTranslated(child) && !IsTranslationCancelled(child))
        //            LocalizeControl(child, folder, excludedControls);
        //    }
        //}



        protected internal abstract void AddBaseTemplate(Template template, Template baseTemplate);

        protected internal abstract void AddTemplateField(Template template, string section, string name, string type, bool isMultilingual, string settings, string standardValue);

        protected internal abstract string ConvertToString(object value);

        protected internal abstract bool GetBool(Entity entity, string fieldName);

        //protected internal abstract XPathNodeIterator GetChildNodeIterator(Entity entity);

        //internal System.Xml.XmlNode GetConfigNode()
        //{
        //    XmlDocument document = new XmlDocument();
        //    document.Load(ApplicationSettings.MapPath("~/App_Config/Core.config"));
        //    return document.DocumentElement["SolutionSettings"];
        //}

        protected internal abstract string GetDisplayName(Entity entity);

        protected internal abstract NameValueCollection GetFields(Entity entity);

        //protected internal abstract string GetFieldValue(XPathNodeIterator node, string fieldName);

        protected internal abstract T GetFieldValue<T>(Entity entity, string fieldName);

        protected internal abstract string GetHostName(Page page);

        protected internal abstract Id GetId(Entity entity);

        protected internal abstract string GetImageUrl(Image image, ImageSize imageSize);

        protected internal abstract string GetName(Entity entity);

        //protected internal abstract XPathNodeIterator GetNodeIterator(Entity entity);

        protected internal abstract string GetPath(Entity entity);

        protected internal abstract Template GetTemplate(Entity entity);

        protected internal abstract string GetUrl(Entity page);

        protected internal abstract bool HasVersion(Entity entity);

        protected internal abstract bool SaveItem(Entity entity);

        internal void SetFieldValue(Entity entity, string fieldName, object value)
        {
            string valueAsString = ConvertToString(value);
            SetEntityFieldValue(entity, fieldName, valueAsString);
        }

        protected internal abstract bool TemplateIs(Entity entity, string templateName);

        protected abstract void SetEntityFieldValue(Entity entity, string fieldName, string value);

        //private static string GetKeyPostFix(string controltype)
        //{
        //    switch (controltype)
        //    {
        //        case "RequiredFieldValidator": return "Required Error";
        //        case "CompareValidator": return "Compare Error";
        //        case "RangeValidator": return "Range Error";
        //        case "RegularExpressionValidator": return "Format Error";
        //        case "CustomValidator": return "Error";
        //        case "LocalizedLiteral": return "Text";
        //        case "Localize": return "Text";
        //        default: return controltype;
        //    }
        //}

        //private static CmsService InstantiateCmsService()
        //{
        //    var assemblyName = new AssemblyQualifiedName(_configuration.CmsServiceProvider);
        //    var result = assemblyName.ActivateObject<CmsService>();
        //    return result;
        //}

        //private static bool IsSelfTranslated(Control control)
        //{
        //    if (!(control is UserControl))
        //        return false;

        //    return control.GetType().GetCustomAttributes(typeof(LocalizationAttribute), true).Cast<LocalizationAttribute>().Any();
        //}

        //private bool IsTranslationCancelled(Control child)
        //{
        //    if (child is SelfTranslatingUserControl)
        //        return ((SelfTranslatingUserControl)child).CancelTranslation;
        //    return false;
        //}

        //private void LocalizeControl(Control control, string folder, List<string> excludedControls)
        //{
        //    if (control.ID != null && !excludedControls.Contains(control.ID) && (control is Label || control is Localize || control is Button || control is LinkButton || control is LocalizedLink || control is CheckBox || control is LocalizedLiteral))
        //    {
        //        // Remove prefix from control id ( all preceding lowercase chars eg. btn or lbl ).
        //        // If the item name is invalid, use the parent item name.

        //        Match match = _TranslationKeyRegex.Match(control.ID);
        //        if (!match.Success)
        //        {
        //            Control parent = control.Parent;
        //            while (parent != null && !string.IsNullOrEmpty(parent.ID) && !match.Success)
        //            {
        //                match = _TranslationKeyRegex.Match(parent.ID);
        //                if (!match.Success)
        //                    parent = parent.Parent;
        //            }
        //        }

        //        if (!match.Success)
        //            return;

        //        //throw new ApplicationException("Control could not be translated: " + control.ID);

        //        string itemName = match.Groups[1].Captures.ToSeparatedString(" ");
        //        string postFix = GetKeyPostFix(control.GetType().Name);
        //        if (!string.IsNullOrEmpty(postFix))
        //            itemName += " " + postFix;

        //        if (control is BaseValidator)
        //        {
        //            BaseValidator validator = control as BaseValidator;
        //            validator.ErrorMessage = GetTranslation(folder, itemName, validator.ErrorMessage);
        //        }
        //        else if (control is LocalizedLink)
        //        {
        //            LocalizedLink localizedLink = control as LocalizedLink;
        //            localizedLink.SetTranslation(GetTranslation(folder, itemName, localizedLink.Text));
        //        }
        //        else if (control is Localize)
        //        {
        //            Localize localize = control as Localize;
        //            localize.Text = GetTranslation(folder, itemName, localize.Text);
        //        }
        //        else if (control is Button)
        //        {
        //            Button button = control as Button;
        //            button.Text = GetTranslation(folder, itemName, button.Text);
        //        }
        //        else if (control is LinkButton)
        //        {
        //            LinkButton linkButton = control as LinkButton;
        //            linkButton.Text = GetTranslation(folder, itemName, linkButton.Text);
        //        }
        //        else if (control is CheckBox)
        //        {
        //            CheckBox checkBox = control as CheckBox;
        //            checkBox.Text = GetTranslation(folder, itemName, checkBox.Text);
        //        }
        //        else if (control is Label)
        //        {
        //            Label label = control as Label;
        //            label.Text = GetTranslation(folder, itemName, label.Text);
        //        }
        //        else if (control is LocalizedLiteral)
        //        {
        //            LocalizedLiteral localizedLiteral = (LocalizedLiteral)control;
        //            localizedLiteral.SetTranslation(GetTranslation(folder, itemName, localizedLiteral.Text));
        //        }
        //    }
        //    else
        //    {
        //        foreach (Control child in control.Controls)
        //        {
        //            if (!IsSelfTranslated(child))
        //                LocalizeControl(child, folder, excludedControls);
        //        }
        //    }
        //}

        #endregion Methods

        protected internal abstract Link ParseLink(string value);

        protected internal abstract LinkList ParseLinkList(string value);

        protected internal abstract bool HasValue(Entity entity, string key);

        protected internal abstract IEnumerable<T> GetFieldValues<T>(Entity entity, string fieldName);

        #region Wrapper Methods

        private static T[] WrapItems<T>(IEnumerable<object> items) where T : Entity, new()
        {
            return items.Select(GetItemWrapper<T>).ToArray();
        }

        private T[] WrapItemsOfType<T>(IEnumerable<object> items) where T : Entity, new()
        {
            var templatePath = new T().TemplatePath;
            return items.Where(i => GetTemplatePath(i) == templatePath).Select(GetItemWrapper<T>).ToArray();
        }

        private T[] WrapItemsOfType<T, TE>(IEnumerable<object> items)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            var typeTable = new TE();
            return items.Select(i => GetItemWrapper<T>(i, typeTable)).Where(i => i != null).ToArray();
        }

        public static T GetItemWrapper<T>(object innerItem) where T : Entity, new()
        {
            if (innerItem == null)
                return null;
            object result = new T();
            ((Entity)result).Initialize(innerItem);
            return (T)result;
        }

        private T GetItemWrapper<T>(object innerItem, EntityTypeTable typeTable) where T : Entity, new()
        {
            var paramType = typeof(T);
            var type = typeTable.GetType(GetTemplatePath(innerItem));
            if (paramType.IsAssignableFrom(type))
            {
                var result = (T)Activator.CreateInstance(type);
                result.Initialize(innerItem);
                return result;
            }
            return null;
        }

        #endregion

        protected abstract string GetTemplatePath(object item);

        internal T GetAnchestorOfType<T>(Entity entity) where T : Entity, new()
        {
            var templatePath = new T().TemplatePath;
            var parent = GetAnchestorOfType(entity.WrappedItem, templatePath);
            return parent == null ? default(T) : GetItemWrapper<T>(parent);
        }

        internal T GetAnchestorOfType<T, TE>(Entity entity)
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            var typeTable = new TE();
            var parent = GetParent(entity.WrappedItem);
            while (parent != null)
            {
                var tmp = GetItemWrapper<T>(parent, typeTable);
                if (tmp != null)
                    return tmp;
                parent = GetParent(parent);
            }
            return null;
        }

        protected abstract object GetAnchestorOfType(object item, string templatePath);

        public abstract void ResolveItem(string url, CmsContext context);

        public abstract string ResolveSitePath(string url, out string language);

        public abstract string ResolveImagePath(string imageUrl);

        protected internal abstract void PublishItem(Entity entity);

        public abstract UserAccount GetUser(string username);

        public abstract UserAccount GetUserByEmail(string domain, string email);

        public abstract UserAccount GetCurrentUser();

        public abstract UserAccount CreateUser(string domain, string email, string password);

        protected internal abstract Stack<CmsContext> GetCmsContextStack();

        public abstract string GetSystemPath(string p, string relativePath);

        public abstract Image GetImage(Id imageId);

        //public LinkTranslations Links
        //{
        //    get
        //    {
        //        if (_linkTranslations != null)
        //            return _linkTranslations;

        //        string sitePath = null;
        //        try
        //        {
        //            sitePath = this.SitePath;
        //        }
        //        catch
        //        {
        //        }
        //        if (string.IsNullOrEmpty(sitePath))
        //            sitePath = CmsContext.Current.SitePath;

        //        if (string.IsNullOrEmpty(sitePath))
        //            return new LinkTranslations();

        //        var collectionsPage = GetItem(GetSystemPath("CollectionsPage", HomeItemPath));
        //        if (collectionsPage == null)
        //            return new LinkTranslations();

        //        var baseUrl = GetInternalLink(collectionsPage);
        //        if (string.IsNullOrEmpty(baseUrl))
        //            return new LinkTranslations();

        //        _linkTranslations = new LinkTranslations(sitePath, CmsContext.Current.Language, baseUrl);
        //        return _linkTranslations;
        //    }
        //}

        protected abstract string GetInternalLink(object item);

        //public ILinkService Links
        //{
        //    get
        //    {
        //        try
        //        {
        //            if (this.HomeItemPath == null)
        //                return null;
        //            return Cache.Get("LinkService", CacheScope.Site, GetLinkManager);
        //        }
        //        catch(InvalidOperationException)
        //        {
        //            return null;
        //        }
        //    }
        //}

        //protected abstract ILinkService GetLinkManager();

        public T GetConfigurationItem<T>(string itemName) where T : Entity, new()
        {
            var item = GetConfigurationObject(itemName, SitePath);
            return item == null ? null : GetItemWrapper<T>(item);
        }

        public T GetConfigurationItem<T>(string itemName, string relativePath) where T : Entity, new()
        {
            var item = GetConfigurationObject(itemName, relativePath);
            return item == null ? null : GetItemWrapper<T>(item);
        }

        protected abstract object GetConfigurationObject(string objectName, string relativePath);

        public abstract Template GetTemplate(Id id);

        public abstract string GetPath(Id itemId);

        protected internal abstract string GetIconUrl(Entity entity);

        public abstract void ChangeTemplate(Entity entity, Template template);

        public abstract Page GetHomeItem();

        public void BuildSnapShot(SnapShotOptions options, XmlWriter writer)
        {
            writer.WriteRaw("<?xml version=\"1.0\" encoding=\"utf-8\"?>\r\n");
            writer.WriteStartElement("snapshot");
            
            var iterators = GetDataIterators(options);
            foreach (var iterator in iterators)
            {
                writer.WriteStartElement(iterator.ItemType);
                while (iterator.ReadNext())
                {
                    iterator.RenderCurrent(writer);
                }
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
        }

        public void InstallSnapShot(XmlDocument document, StringBuilder log)
        {
            var installers = GetDataInstallers();
            if (installers == null)
                return;
            foreach (var installer in installers)
                installer.Install(document, log);
        }


        protected virtual List<DataIterator> GetDataIterators(SnapShotOptions options)
        {
            var iterators = new List<DataIterator>();
            if (options.InvalidPaths == null || !options.InvalidPaths.Contains("files"))
                iterators.Add(new FileIterator(options));
            return iterators;
        }

        protected virtual List<DataInstaller> GetDataInstallers()
        {
            var iterators = new List<DataInstaller>();
            return iterators;
        }
    }
}