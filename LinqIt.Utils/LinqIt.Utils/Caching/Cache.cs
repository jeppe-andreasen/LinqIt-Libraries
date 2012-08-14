using System;
using System.Collections.Generic;
using System.IO;
using System.Web;

namespace LinqIt.Utils.Caching
{
    #region Enumerations

    public enum CacheScope
    {
        Application,
        Request,
        FileDependency
        //Site
    }

    #endregion Enumerations

    public static class Cache
    {
        #region Methods

        public static void Add<T>(string key, CacheScope scope, T value)
        {
            var cacheKey = key;
            if (scope == CacheScope.Application)
                ApplicationScopeCache.Instance.Add(key, value);
            else
                new RequestScopeCache().Add(cacheKey, value);
        }

        public static void Clear(string key, CacheScope scope)
        {
            var cacheKey = key;
            switch (scope)
            {
                case CacheScope.Request:
                    new RequestScopeCache().Clear(cacheKey);
                    break;
                case CacheScope.Application:
                    ApplicationScopeCache.Instance.Clear(cacheKey);
                    break;
                //case CacheScope.Site:
                //    SiteScopeCache.Instance.Clear();
                //    break;
            }
        }

        public static void ClearAll()
        {
            //SiteScopeCache.ClearAll();
            ApplicationScopeCache.ClearAll();
        }

        public static T Get<T>(string key, CacheScope scope, Func<T> getter)
        {
            switch (scope)
            {
                case CacheScope.Application:
                    return ApplicationScopeCache.Instance.Get(key, getter);
                //case CacheScope.Site:
                //    return SiteScopeCache.Instance.Get(key, getter);
                case CacheScope.FileDependency:
                    return FileDependencyCache.Instance.Get(key, getter);
                default:
                    return new RequestScopeCache().Get(key, getter);
            }
        }

        public static T Get<T>(string key, CacheScope scope, Func<T> getter, TimeSpan expiration)
        {
            switch (scope)
            {
                case CacheScope.Application:
                    return ApplicationScopeCache.Instance.Get(key, getter, expiration);
                //case CacheScope.Site:
                //    return SiteScopeCache.Instance.Get(key, getter, expiration);
                case CacheScope.FileDependency:
                    return FileDependencyCache.Instance.Get(key, getter);
                default:
                    return new RequestScopeCache().Get(key, getter);
            }
        }

        #endregion Methods

        #region Nested Types

        internal class ApplicationScopeCache : CacheBase
        {
            #region Fields

            private static readonly ApplicationScopeCache _instance = new ApplicationScopeCache();
            private static readonly int _timeoutMinutes;

            private readonly Dictionary<string, CacheItem> _cache;

            #endregion Fields

            #region Constructors

            static ApplicationScopeCache()
            {
                _timeoutMinutes = Properties.Settings.Default.ApplicationScopeTimeoutMinutes;
            }

            /// <summary>
            /// 	Singleton class
            /// </summary>
            private ApplicationScopeCache()
            {
                _cache = new Dictionary<string, CacheItem>();
            }

            #endregion Constructors

            #region Properties

            public static ApplicationScopeCache Instance
            {
                get { return _instance; }
            }

            #endregion Properties

            #region Methods

            public override void Add<T>(string key, T value)
            {
                lock (this)
                {
                    var item = new CacheItem(value, DateTime.Now.AddMinutes(_timeoutMinutes));
                    _cache.Add(key, item);
                }
            }

            public void Clear()
            {
                lock (this)
                {
                    _cache.Clear();
                }
            }

            public void Clear(string cacheKey)
            {
                lock (this)
                {
                    _cache.Remove(cacheKey);
                }
            }

            public override T Get<T>(string key, Func<T> getter)
            {
                return Get(key, getter, TimeSpan.FromMinutes(_timeoutMinutes));
            }

            public override T Get<T>(string key, Func<T> getter, TimeSpan expiration)
            {
                if (Contains(key))
                    return (T)_cache[key].Item;
                lock (this)
                {
                    if (Contains(key))
                        return (T)_cache[key].Item;
                    
                    object obj = getter();
                    if (obj != null)
                    {
                        var item = new CacheItem(obj, DateTime.Now.Add(expiration));
                        _cache.Add(key, item);
                    }
                    return (T)obj;
                }
            }

            private bool Contains(string key)
            {
                if (!_cache.ContainsKey(key))
                    return false;
                var item = _cache[key];
                if (item.ExpiresAt > DateTime.Now)
                    return true;
                _cache.Remove(key);
                return false;
            }

            #endregion Methods

            #region Nested Types

            internal class CacheItem
            {
                #region Constructors

                internal CacheItem(object item, DateTime expiresAt)
                {
                    Item = item;
                    ExpiresAt = expiresAt;
                }

                #endregion Constructors

                #region Properties

                public DateTime ExpiresAt { get; set; }

                public object Item { get; set; }

                #endregion Properties
            }

            #endregion Nested Types

            internal static void ClearAll()
            {
                lock (_instance)
                {
                    _instance._cache.Clear();
                }
            }
        }

        //internal class SiteScopeCache : CacheBase
        //{
        //    #region Fields

        //    private static readonly SiteScopeCache _instance = new SiteScopeCache();
        //    private static readonly int _timeoutMinutes;

        //    private readonly Dictionary<string, Dictionary<string, CacheItem>> _cache;

        //    #endregion Fields

        //    #region Constructors

        //    static SiteScopeCache()
        //    {
        //        _timeoutMinutes = Properties.Settings.Default.SiteScopeTimeoutMinutes;
        //    }

        //    /// <summary>
        //    /// 	Singleton class
        //    /// </summary>
        //    private SiteScopeCache()
        //    {
        //        _cache = new Dictionary<string, Dictionary<string, CacheItem>>();
        //    }

        //    #endregion Constructors

        //    #region Properties

        //    public static SiteScopeCache Instance
        //    {
        //        get { return _instance; }
        //    }

        //    #endregion Properties

        //    #region Methods

        //    private Dictionary<string, CacheItem> GetCache()
        //    {
        //        var site = CmsService.Instance.SitePath ?? "";
        //        if (_cache.ContainsKey(site))
        //            return _cache[site];

        //        var result = new Dictionary<string, CacheItem>();
        //        _cache.Add(site, result);
        //        return result;
        //    }

        //    public override void Add<T>(string key, T value)
        //    {
        //        lock (this)
        //        {
        //            var cache = GetCache();
        //            var item = new CacheItem(value, DateTime.Now.AddMinutes(_timeoutMinutes));
        //            cache.Add(key, item);
        //        }
        //    }

        //    public void Clear()
        //    {
        //        lock (this)
        //        {
        //            GetCache().Clear();
        //        }
        //    }

        //    public void Clear(string cacheKey)
        //    {
        //        lock (this)
        //        {
        //            GetCache().Remove(cacheKey);
        //        }
        //    }

        //    public override T Get<T>(string key, Func<T> getter)
        //    {
        //        return Get(key, getter, TimeSpan.FromMinutes(_timeoutMinutes));
        //    }

        //    public override T Get<T>(string key, Func<T> getter, TimeSpan expiration)
        //    {
        //        Dictionary<string, CacheItem> cache;

        //        if (Contains(key, out cache))
        //            return (T)cache[key].Item;
        //        lock (this)
        //        {
        //            if (Contains(key, out cache))
        //                return (T)cache[key].Item;
                    
        //            object obj = getter();
        //            if (obj != null)
        //            {
        //                var item = new CacheItem(obj, DateTime.Now.Add(expiration));
        //                cache.Add(key, item);
        //            }
        //            return (T)obj;
        //        }
        //    }

        //    private bool Contains(string key, out Dictionary<string, CacheItem> cache)
        //    {
        //        cache = GetCache();
        //        if (!cache.ContainsKey(key))
        //            return false;
        //        var item = cache[key];
        //        if (item.ExpiresAt > DateTime.Now)
        //            return true;
        //        cache.Remove(key);
        //        return false;
        //    }

        //    #endregion Methods

        //    #region Nested Types

        //    internal class CacheItem
        //    {
        //        #region Constructors

        //        internal CacheItem(object item, DateTime expiresAt)
        //        {
        //            Item = item;
        //            ExpiresAt = expiresAt;
        //        }

        //        #endregion Constructors

        //        #region Properties

        //        public DateTime ExpiresAt { get; set; }

        //        public object Item { get; set; }

        //        #endregion Properties
        //    }

        //    #endregion Nested Types

        //    internal static void ClearAll()
        //    {
        //        lock (_instance)
        //        {
        //            _instance._cache.Clear();
        //        }
        //    }
        //}

        /// <summary>
        /// 	Serves as the base class for cache implementations.
        /// </summary>
        internal abstract class CacheBase
        {
            #region Methods

            public abstract void Add<T>(string key, T value);

            public abstract T Get<T>(string key, Func<T> getter);

            public abstract T Get<T>(string key, Func<T> getter, TimeSpan expiration);

            #endregion Methods
        }

        internal class FileDependencyCache : CacheBase
        {
            #region Fields

            private static readonly FileDependencyCache _instance = new FileDependencyCache();

            private readonly Dictionary<string, CacheItem> _cache;

            #endregion Fields

            #region Constructors

            private FileDependencyCache()
            {
                _cache = new Dictionary<string, CacheItem>();
            }

            #endregion Constructors

            #region Properties

            public static FileDependencyCache Instance
            {
                get { return _instance; }
            }

            #endregion Properties

            #region Methods

            public override void Add<T>(string key, T value)
            {
                lock (this)
                {
                    var item = new CacheItem(value);
                    _cache.Add(key, item);
                }
            }

            public void Clear()
            {
                lock (this)
                {
                    _cache.Clear();
                }
            }

            public void Clear(string cacheKey)
            {
                lock (this)
                {
                    _cache.Remove(cacheKey);
                }
            }

            public override T Get<T>(string key, Func<T> getter)
            {
                if (Contains(key))
                    return (T)_cache[key].Item;
                lock (this)
                {
                    if (Contains(key))
                        return (T)_cache[key].Item;

                    object obj = getter();
                    if (obj != null)
                    {
                        var item = new CacheItem(obj);
                        _cache.Add(key, item);
                    }
                    return (T)obj;
                }
            }

            public override T Get<T>(string key, Func<T> getter, TimeSpan expiration)
            {
                return Get(key, getter);
            }

            private bool Contains(string key)
            {
                if (!_cache.ContainsKey(key))
                    return false;
                var item = _cache[key];
                if (!item.IsExpired(key))
                    return true;
                _cache.Remove(key);
                return false;
            }

            #endregion Methods

            #region Nested Types

            internal class CacheItem
            {
                #region Fields

                private readonly DateTime _timeStamp;

                #endregion Fields

                #region Constructors

                internal CacheItem(object item)
                {
                    Item = item;
                    _timeStamp = DateTime.Now;
                }

                #endregion Constructors

                #region Properties

                public object Item { get; set; }

                #endregion Properties

                #region Methods

                internal bool IsExpired(string key)
                {
                    try
                    {
                        var lastWriteTime = File.GetLastWriteTime(key);
                        return lastWriteTime > _timeStamp;
                    }
                    catch
                    {
                        return false;
                    }
                }

                #endregion Methods
            }

            #endregion Nested Types
        }

        /// <summary>
        /// 	Implements a cache where objects are only valid within the current request scope
        /// 	Only works with in web environment, no locking is needed as all access
        /// 	are made from the same thread(requirement, ie not thread safe)
        /// </summary>
        internal class RequestScopeCache : CacheBase
        {
            #region Indexers

            private object this[string key]
            {
                get
                {
                    if (HttpContext.Current == null)
                    {
                        throw new NullReferenceException("HttpContext.Current is not valid");
                    }
                    return HttpContext.Current.Items[key];
                }
                set
                {
                    if (HttpContext.Current == null)
                    {
                        throw new NullReferenceException("HttpContext.Current is not valid");
                    }
                    HttpContext.Current.Items[key] = value;
                }
            }

            #endregion Indexers

            #region Methods

            public override void Add<T>(string key, T value)
            {
                this[key] = value;
            }

            public override T Get<T>(string key, Func<T> getter)
            {
                if (HttpContext.Current == null)
                {
                    return getter();
                }
                if (!HttpContext.Current.Items.Contains(key))
                {
                    object obj = getter();
                    if (obj != null)
                    {
                        this[key] = obj;
                    }
                    return (T)obj;
                }
                return (T)this[key];
            }

            public override T Get<T>(string key, Func<T> getter, TimeSpan expiration)
            {
                return Get(key, getter);
            }

            internal void Clear(string cacheKey)
            {
                this[cacheKey] = null;
            }

            #endregion Methods
        }

        #endregion Nested Types
    }
}