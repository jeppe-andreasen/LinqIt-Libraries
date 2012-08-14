namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Xml.XPath;

    using LinqIt.Cms.Utilities;
    
    

    public class Entity : IComparable, IXPathNavigable
    {
        private readonly Dictionary<string, object> _fields = new Dictionary<string, object>();


        #region Properties

        /// <summary>
        /// Returns the encapsulated items displayname
        /// </summary>
        public string DisplayName
        {
            get { return CmsService.Instance.GetDisplayName(this); }
        }

        /// <summary>
        /// Returns the encapsulated items's Name
        /// </summary>
        public string EntityName
        {
            get { return CmsService.Instance.GetName(this); }
        }

        public bool HasVersion
        {
            get { return CmsService.Instance.HasVersion(this); }
        }

        /// <summary>
        /// Returns the encapsulated item's ID
        /// </summary>
        public Id Id
        {
            get { return CmsService.Instance.GetId(this); }
        }

        public string Path
        {
            get { return CmsService.Instance.GetPath(this); }
        }

        public virtual Id TemplateId
        {
            get { return null; }
        }

        public virtual string TemplateName
        {
            get { return GetType().Name; }
        }

        public virtual string TemplatePath
        {
            get { return string.Empty; }
        }

        public object WrappedItem
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public T CastAs<T>() where T:Entity, new()
        {
            return new T() { WrappedItem = this.WrappedItem };
        }

        public int CompareTo(object obj)
        {
            var entity = obj as Entity;
            if (entity == null)
                throw new ArgumentException("Cannot compare Entity with " + obj.GetType().Name);
            return this.Id.CompareTo(entity.Id);
        }

        public XPathNavigator CreateNavigator()
        {
            throw new NotImplementedException();
            //return new EntityXPathNavigator(this);
        }

        public XPathNodeIterator GetChildNodeIterator()
        {
            throw new NotImplementedException();
            //return CmsService.Instance.GetChildNodeIterator(this);
        }

        private T Get<T>(string key, Func<T> resolver)
        {
            if (_fields.ContainsKey(key)) 
                return (T)_fields[key];

            var result = resolver();
            _fields.Add(key, result);
            return result;
        }

        protected bool HasValue(string key)
        {
            return CmsService.Instance.HasValue(this, key);
        }

        #region Get Children

        public IEnumerable<T> GetChildren<T>() where T : Entity, new()
        {
            string key = "GetChildren" + typeof(T).Name;
            return Get<IEnumerable<T>>(key, () => CmsService.Instance.GetChildren<T>(this).ToArray());
        }

        public IEnumerable<T> GetChildrenOfType<T,TE>()
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            string key = "GetChildren" + typeof(T).Name;
            return Get<IEnumerable<T>>(key, () => CmsService.Instance.GetChildrenOfType<T,TE>(this).ToArray());
        }

        public IEnumerable<T> GetChildrenOfType<T>()
            where T : Entity, new()
        {
            string key = "GetChildrenOfType" + typeof(T).Name;
            return Get<IEnumerable<T>>(key, () => CmsService.Instance.GetChildrenOfType<T>(this).ToArray());
        }

        #endregion

        #region Get Descendants

        public IEnumerable<T> GetDescendantsOfType<T>()
            where T : Entity, new()
        {
            string key = "GetDescendantsOfType" + typeof(T).Name;
            return Get<IEnumerable<T>>(key, () => CmsService.Instance.GetDescendantsOfType<T>(this).ToArray());
        }

        public IEnumerable<T> GetDescendantsOfType<T, TE>()
            where T : Entity, new()
            where TE : EntityTypeTable, new()
        {
            string key = "GetDescendandtsOfType" + typeof(T).Name + typeof(TE).Name;
            return Get<IEnumerable<T>>(key, () => CmsService.Instance.GetDescendantsOfType<T,TE>(this).ToArray());
        }

        #endregion

        public XPathNodeIterator GetNodeIterator()
        {
            //return CmsService.Instance.GetNodeIterator(this);
            throw new NotImplementedException();
        }

        public string this[string key]
        {
            get { return CmsService.Instance.GetFieldValue<string>(this, key); }
            set
            {
                this.SetValue(key, value);
            }
        }

        public T GetParent<T>()
            where T : Entity, new()
        {
            return CmsService.Instance.GetParent<T>(this);
        }

        public bool? GetTristate(string fieldName)
        {
            if (string.IsNullOrEmpty(this[fieldName]))
                return null;
            return this.GetValue<bool>(fieldName);
        }

        public T GetValue<T>(string fieldName)
        {
            return Get<T>(fieldName, () => CmsService.Instance.GetFieldValue<T>(this, fieldName));
        }

        public IEnumerable<T> GetListValue<T>(string fieldName)
        {
            return this.Get<IEnumerable<T>>(fieldName, () => CmsService.Instance.GetFieldValues<T>(this, fieldName));
        }

        public void Initialize(object wrappedItem)
        {
            WrappedItem = wrappedItem;
        }

        public bool Is(string templateName)
        {
            return string.Compare(TemplateName, templateName, true) == 0;
        }

        public bool Save()
        {
            return CmsService.Instance.SaveItem(this);
        }

        public bool TemplateIs(string templateName)
        {
            return CmsService.Instance.TemplateIs(this, templateName);
        }

        internal NameValueCollection GetAttributes()
        {
            return CmsService.Instance.GetFields(this);
        }

        protected bool GetBool(string fieldName)
        {
            return Get<bool>(fieldName, () => CmsService.Instance.GetBool(this, fieldName));
        }

        protected IEnumerable<T> GetEntities<T>(string fieldName) where T : Entity, new()
        {
            IdList ids = this.GetValue<IdList>(fieldName);
            return CmsService.Instance.GetItems<T>(ids);
        }

        protected IEnumerable<T> GetEntities<T, TE>(string fieldName) where T : Entity, new() where TE:EntityTypeTable,new()
        {
            IdList ids = this.GetValue<IdList>(fieldName);
            return CmsService.Instance.GetItemsOfType<T,TE>(ids);
        }

        protected T GetEntity<T>(string fieldName) where T : Entity, new()
        {
            Id id = this.GetValue<Id>(fieldName);
            if (id == null || id.IsNull)
                return null;
            return CmsService.Instance.GetItem<T>(id);
        }

        protected void SetValue(string fieldName, object value)
        {
            CmsService.Instance.SetFieldValue(this, fieldName, value);
        }

        public T GetAnchestorOfType<T>() where T:Entity, new()
        {
            return CmsService.Instance.GetAnchestorOfType<T>(this);
        }

        public T GetAnchestorOfType<T, TE>() where T : Entity, new() where TE:EntityTypeTable, new()
        {
            return CmsService.Instance.GetAnchestorOfType<T, TE>(this);
        }

        protected Entity GetEntityOrParentWithValue(string fieldName)
        {
            var result = this;
            bool done = false;
            while (!done && result != null)
            {
                if (!string.IsNullOrEmpty(result[fieldName]))
                    done = true;
                else
                    result = result.GetParent<Entity>();
            }
            return result;
        }

        #endregion Methods

        public void Publish()
        {
            CmsService.Instance.PublishItem(this);
        }

        public string Icon { get { return CmsService.Instance.GetIconUrl(this); } }

        public Template Template
        {
            get { return CmsService.Instance.GetTemplate(this); }
        }
    }
}