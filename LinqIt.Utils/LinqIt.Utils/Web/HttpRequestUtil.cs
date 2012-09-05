using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils.Web
{
    public static class HttpRequestUtil
    {
        public static string Post(string url, string formData)
        {
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
            httpWebRequest.ContentType = "application/x-www-form-urlencoded";
            httpWebRequest.Method = "POST";

            var requestBytes = Encoding.UTF8.GetBytes(formData);
            httpWebRequest.ContentLength = requestBytes.Length;
            using (var requestStream = httpWebRequest.GetRequestStream())
            {
                requestStream.Write(requestBytes, 0, requestBytes.Length);
                requestStream.Close();
            }

            using (var responseStream = httpWebRequest.GetResponse().GetResponseStream())
            if (responseStream != null)
            {
                using (var reader = new StreamReader(responseStream))
                {
                    return reader.ReadToEnd();
                }
            }
            return null;

            //var responseTask = Task.Factory.FromAsync<WebResponse>(httpWebRequest.BeginGetResponse, httpWebRequest.EndGetResponse, null);
            //using (var responseStream = responseTask.Result.GetResponseStream())
            //{
            //    if (responseStream == null)
            //        return null;
            //    var reader = new StreamReader(responseStream);
            //    return reader.ReadToEnd();
            //}
        }

        public static string Post(string url, NameValueCollection values)
        {
            return Post(url, values.ToUrlParameterList());
        }
    }
}
