using LinqIt.Utils.Extensions;

namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class Page : Entity
    {
        #region Properties

        public string HostName
        {
            get { return CmsService.Instance.GetHostName(this); }
        }

        public Page ParentPage
        {
            get
            {
                return CmsService.Instance.GetParent<Page>(this);
            }
        }

        public virtual string Url
        {
            get
            {
                return CmsService.Instance.GetUrl(this);
            }
        }

        public string GetUrl(bool includeHost)
        {
            Uri uri = new Uri(Url, UriKind.RelativeOrAbsolute);
            if (includeHost && !uri.IsAbsoluteUri)
            {
                string hostname = CmsService.Instance.GetHostName();
                return string.Format("http://{0}/{1}", hostname, Url.TrimStart('/'));
            }
            return Url;
        }

        #endregion Properties

        #region Methods

        public string GetRelativePath(string relativePath)
        {
            if (relativePath.StartsWith("/"))
                return relativePath;

            var pathFragments = Path.TrimEnd('/').Split('/').ToList();
            var relativeFragments = relativePath.TrimStart('/').Split('/').ToList();
            while (relativeFragments.Any())
            {
                string currentPath = relativeFragments.First();
                if (currentPath == "..")
                    pathFragments.RemoveAt(pathFragments.Count - 1);
                else if (currentPath != ".")
                    pathFragments.Add(currentPath);
                relativeFragments.RemoveAt(0);
            }
            string result = "/" + pathFragments.ToSeparatedString("/");
            return result;
        }

        

        internal IEnumerable<Page> GetSubPages()
        {
            return CmsService.Instance.GetChildren<Page>(this);
        }

        #endregion Methods

        
    }
}