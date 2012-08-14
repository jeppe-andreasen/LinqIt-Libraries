using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils.Web
{
    public class UrlBuilder
    {
        private static readonly Regex _schemeRegex = new Regex("^(https?)://", RegexOptions.IgnoreCase);

        public UrlBuilder(string url)
        {
            Url = ParseUrl(url);
            Parameters = ParseParameters(url);
        }

        public UrlBuilder(string url, bool ensureAbsoluteUrl)
        {
            Url = ParseUrl(url);
            if (ensureAbsoluteUrl && url.StartsWith("/"))
                Url = string.Format("{0}://{1}{2}", HttpContext.Current.Request.Url.Scheme, HttpContext.Current.Request.Url.Host, Url);
            Parameters = ParseParameters(url);
        }

        private static string ParseUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return string.Empty;
            return url.IndexOf("?") > -1 ? url.Substring(0, url.IndexOf("?")) : url;
        }

        private static NameValueCollection ParseParameters(string url)
        {
            var result = new NameValueCollection();
            if (string.IsNullOrEmpty(url))
                return result;

            if (url.IndexOf("?") > -1)
            {
                var queryPart = url.Substring(url.IndexOf("?") + 1).Replace("&amp;", "&");
                var parameters = queryPart.Split('&');
                foreach (var pair in parameters)
                    result.Add(pair.Split('=')[0], HttpUtility.UrlDecode(pair.Split('=')[1]));
            }
            return result;
        }

        public string Url { get; private set; }

        public NameValueCollection Parameters { get; private set; }

        public override string ToString()
        {
            string result = Url;
            if (Parameters.Count > 0)
            {
                result += "?" + Parameters.Keys.Cast<string>().ToSeparatedString("&", (k) => string.Format("{0}={1}", k, HttpUtility.UrlEncode(Parameters[k])));
            }
            return result;
        }


        public static bool IsAbsolute(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;

            var regex = new Regex("[^:]+://[^/]+");
            return regex.IsMatch(url);
        }

        public static string MakeRelative(string url)
        {
            if (string.IsNullOrEmpty(url))
                return url;

            var regex = new Regex("[^:]+://[^/]+");
            return regex.IsMatch(url) ? regex.Replace(url, "") : url;
        }

        public string HostName
        {
            get
            {
                Uri uri = new Uri(Url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                    return uri.Host;
                else
                    return string.Empty;
            }
            set
            {
                Uri uri = new Uri(Url, UriKind.RelativeOrAbsolute);
                if (uri.IsAbsoluteUri)
                    Url = uri.Scheme + "://" + value + uri.LocalPath;
                else
                    Url = "http://" + value + Url;
            }
        }

        public string Scheme
        {
            get
            {
                if (!string.IsNullOrEmpty(Url))
                {
                    var match = _schemeRegex.Match(Url);
                    if (match.Success)
                        return match.Groups[1].Value;    
                }
                return null;
            } 
            set
            {
                if (string.IsNullOrEmpty(Url) || Url.StartsWith("/"))
                    return;
                var match = _schemeRegex.Match(Url);
                if (match.Success)
                    Url = _schemeRegex.Replace(Url, value + "://");
                else
                    Url = value + "://" + Url;
            }
        }
    }
}


