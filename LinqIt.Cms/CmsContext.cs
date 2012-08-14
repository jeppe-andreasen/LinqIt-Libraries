using System.Globalization;
using System.Threading;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinqIt.Utils;

namespace LinqIt.Cms
{
    

    #region Enumerations

    public enum CmsContextType
    {
        Editing, Published, Default, Ajax,Mail, Cms, Handler
    }

    public enum CmsStoreType
    {
        Working,
        Published
    };

    #endregion Enumerations

    public class CmsContext : IDisposable
    {
        private CmsStoreType _storeType = CmsStoreType.Published;

        private static Stack<CmsContext> GetStack()
        {
            return CmsService.Instance.GetCmsContextStack();
        }

        #region Constructors

        public CmsContext(CmsContextType type)
        {
            Type = type;
            Language = System.Threading.Thread.CurrentThread.CurrentUICulture.Name;
            _storeType = Type == CmsContextType.Editing ? CmsStoreType.Working : CmsStoreType.Published;
            GetStack().Push(this);
        }

        

        public CmsContext(CmsContextType type, string language)
        {
            Type = type;
            Language = language;
            _storeType = Type == CmsContextType.Editing ? CmsStoreType.Working : CmsStoreType.Published;
        }

        public CmsContext(CmsContextType type, string path, string sitepath, string language)
        {
            Type = type;
            Path = path;
            SitePath = sitepath;
            Language = language;
            _storeType = Type == CmsContextType.Editing ? CmsStoreType.Working : CmsStoreType.Published;
        }

        #endregion Constructors

        #region Properties

        public static CmsContext Current
        {
            get
            {
                var stack = GetStack();
                return stack != null && stack.Count > 0 ? stack.Peek() : CmsContext.Default;
            }
        }

        public static CmsContext CloneAs(CmsContextType type)
        {
            var current = CmsContext.Current;
            return new CmsContext(type) { Path = current.Path, Language = current.Language, SitePath = current.SitePath };
        }

        public static CmsContext Default
        {
            get { return new CmsContext(CmsContextType.Default);}
        }

        public static CmsContext Editing
        {
            get
            {
                var result = new CmsContext(CmsContextType.Editing);
                result._storeType = CmsStoreType.Working;
                return result;
            }
        }

        public static CmsContext Published
        {
            get { return new CmsContext(CmsContextType.Published); }
        }

        public static CmsContext Mail
        {
            get
            {
                return new CmsContext(CmsContextType.Mail);
            }
        }

        public static CmsContext Ajax
        {
            get
            {
                var result = new CmsContext(CmsContextType.Ajax);
                var url = Regex.Replace(HttpContext.Current.Request.Url.ToString(), @"/[^/]+\.json.*", "");
                string language;
                string sitepath;
                CmsService.Instance.ResolveItem(url, result);
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(result.Language);
                return result;
            }
        }

        public static CmsContext Handler
        {
            get
            {
                var result = new CmsContext(CmsContextType.Handler);
                string language;
                result.SitePath = CmsService.Instance.ResolveSitePath(HttpContext.Current.Request.Url.ToString(), out language);
                result.Language = language;
                Thread.CurrentThread.CurrentUICulture = new CultureInfo(language);
                return result;
            }
        }

        public string Language
        {
            get; set;

        }

        public string Path
        {
            get; set;

        }

        public string SitePath { get; set; }

        public CmsContextType Type
        {
            get; private set;

        }

        #endregion Properties

        #region Methods

        public void Dispose()
        {
            var stack = GetStack();
            if (stack != null && stack.Count > 0)
                stack.Pop();
        }

        #endregion Methods

        public static CmsContext GetCmsContext(Data.Id itemId)
        {
            var result = new CmsContext(CmsContextType.Cms);
            result.Path = CmsService.Instance.GetPath(itemId);
            result.SitePath = PathUtil.Take(result.Path, 3);
            return result;
        }

        public CmsStoreType StoreType { get { return _storeType; } set { _storeType = value; } }
    }
}