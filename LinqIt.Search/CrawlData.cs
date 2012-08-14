using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqIt.Search
{
    public class CrawlData
    {
        public string[] Links { get; set; }
            
        public Uri RequestUri { get; set; }

        public string CharacterSet { get; set; }

        public string ContentEncoding { get; set; }

        public long ContentLength { get; set; }

        public string ContentType { get; set; }

        public System.Net.CookieCollection Cookies { get; set; }

        public System.Net.WebHeaderCollection Headers { get; set; }

        public bool IsFromCache { get; set; }

        public bool IsMutuallyAuthenticated { get; set; }

        public DateTime LastModified { get; set; }

        public string Method { get; set; }

        public Version ProtocolVersion { get; set; }

        public Uri ResponseUri { get; set; }

        public string Server { get; set; }

        public System.Net.HttpStatusCode StatusCode { get; set; }

        public string StatusDescription { get; set; }

        public System.IO.MemoryStream ResponseStream { get; set; }

        public string MimeType
        {
            get
            {
                if (string.IsNullOrEmpty(ContentType))
                    return null;
                var match = Regex.Match(ContentType, "^[^;]+");
                return match.Success ? match.Value : null;
            }
        }

        public string OriginalContent { get; set; }

        public string FilteredContent { get; set; }

        public string Title { get; set; }

        public Dictionary<string, string> MetaData { get; set; }
    }
}
