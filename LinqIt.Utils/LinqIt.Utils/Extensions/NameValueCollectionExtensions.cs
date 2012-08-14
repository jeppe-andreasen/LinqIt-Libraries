using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Web;
using LinqIt.Utils.Web;

namespace LinqIt.Utils.Extensions
{
    public static class NameValueCollectionExtensions
    {
        public static string ToUrlParameterList(this NameValueCollection collection)
        {
            return collection.Keys.Cast<string>().ToSeparatedString("&", k => string.Format("{0}={1}", k, HttpUtility.UrlEncode(collection[k])));
        }

        public static bool ContainsKey(this NameValueCollection collection, string key)
        {
            return collection.AllKeys.Contains(key);
        }

        public static T GetValue<T>(NameValueCollection collection, string key, T defaultValue = default(T))
        {
            return !ContainsKey(collection, key) ? defaultValue : Converter.ConvertFrom(collection[key], defaultValue);
        }
    }
}
