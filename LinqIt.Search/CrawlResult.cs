using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public enum CrawlReason
    {
        RequestError,
        RequestRedirected,
        DownloadCancelled,
        InvalidContentType,
        Unknown,
        InvalidStatusCode
    };

    public class CrawlResult
    {
        public CrawlResult(string url)
        {
            OriginalUrl = url;
        }

        public bool Success { get; internal set; }

        public string OriginalUrl { get; private set; }

        public string Message { get; internal set; }

        public System.Net.WebException Exception { get; internal set; }

        public CrawlData Data { get; set; }

        public CrawlReason Reason { get; set; }

        public string RedirectUrl { get; set; }
    }
}
