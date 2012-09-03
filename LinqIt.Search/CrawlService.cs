using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using LinqIt.Search.Arguments;
using LinqIt.Search.Configuration;
using System.Collections.Generic;
using LinqIt.Utils;

namespace LinqIt.Search
{
    public class CrawlService : IDisposable
    {
        private readonly LinqItSearchConfigurationSection _configuration;
        private readonly ISearchProvider _provider;

        public event EventHandler<RequestEventArgs> BeforeDownload;

        public CrawlService(string indexName)
        {
            _configuration = (LinqItSearchConfigurationSection)ConfigurationManager.GetSection("LinqItSearch");
            _provider = (ISearchProvider)Activator.CreateInstance(Type.GetType(_configuration.Provider));
            _provider.Open(indexName, ServiceType.Write);
            Logging.Log(LogType.Info, "Crawl Service Provider Opened");
        }

        public void ClearDatabase()
        {
            _provider.ClearDatabase();
        }

        public SearchRecord NewRecord(string id)
        {
            return _provider.CreateRecord(id);
        }

        public void AddRecord(SearchRecord record)
        {
            _provider.UpsertRecords(new[] { record });
        }

        public void AddBatch(IEnumerable<SearchRecord> records)
        {
            _provider.UpsertRecords(records);
        }

        public void RemoveRecord(string recordId)
        {
            _provider.RemoveRecord(recordId);
        }

        public SearchRecord GetRecordFromUrl(string url, string contentField, string additionalText)
        {
            var crawlResult = Crawl(url);
            if (!crawlResult.Success)
                throw new ApplicationException(crawlResult.Message, crawlResult.Exception);

            
            var filterService = new FilterService(_configuration);

            var filterResult = filterService.Process(crawlResult.Data);
            if (!filterResult.Success)
                throw new ApplicationException(filterResult.Message, filterResult.Exception);

            var result = _provider.CreateRecord(url);
            result.SetString("url", url);

            var text = (crawlResult.Data.FilteredContent ?? string.Empty).ToLower().Trim();
            additionalText = (additionalText ?? string.Empty).ToLower().Trim();
            if (!string.IsNullOrEmpty(text) && !string.IsNullOrEmpty(additionalText))
                text += " ";
            text += additionalText;
            result.SetString(contentField, text);
            return result;
        }

        public CrawlResult Crawl(string url)
        {
            var result = new CrawlResult(url);
            ServicePointManager.ServerCertificateValidationCallback = new RemoteCertificateValidationCallback(ValidateClientCertificate);
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(url);
            }
            catch (WebException exc)
            {
                result.Message = "Error creating request to " + url + ": " + exc;
                result.Reason = CrawlReason.RequestError;
                result.Exception = exc;
                return result;
            }

            if (BeforeDownload != null)
            {
                var args = new RequestEventArgs(request);
                BeforeDownload(this, args);
                if (args.IsCancelled)
                {
                    result.Message = "Download was cancelled";
                    result.Reason = CrawlReason.DownloadCancelled;
                    return result;
                }
            }
            var data = new CrawlData();
            try
            {
                data.RequestUri = new Uri(url);
                using (var response = (HttpWebResponse)request.GetResponse())
                {
                    if (response.StatusCode == HttpStatusCode.Found || response.StatusCode == HttpStatusCode.Moved)
                    {
                        result.RedirectUrl = response.Headers["Location"];
                        result.Reason = CrawlReason.RequestRedirected;
                        result.Message = "Request was redirected to " + result.RedirectUrl;
                        return result;
                    }

                    if (response.ContentType == "application/octet-stream")
                    {
                        result.Reason = CrawlReason.InvalidContentType;
                        result.Message = "Skipping mimetype application/octet-stream";
                        return result;
                    }

                    data.CharacterSet = response.CharacterSet;
                    data.ContentEncoding = response.ContentEncoding;
                    data.ContentLength = response.ContentLength;
                    data.ContentType = response.ContentType;
                    data.Cookies = response.Cookies;
                    data.Headers = response.Headers;
                    data.IsFromCache = response.IsFromCache;
                    data.IsMutuallyAuthenticated = response.IsMutuallyAuthenticated;
                    data.LastModified = response.LastModified;
                    data.Method = response.Method;
                    data.ProtocolVersion = response.ProtocolVersion;
                    data.ResponseUri = response.ResponseUri;
                    data.Server = response.Server;
                    data.StatusCode = response.StatusCode;
                    data.StatusDescription = response.StatusDescription;
                    data.ResponseStream = GetMemoryStream(response.GetResponseStream());

                    using (var reader = new StreamReader(data.ResponseStream))
                    {
                        data.OriginalContent = reader.ReadToEnd();
                        data.ResponseStream.Seek(0, SeekOrigin.Begin);
                    }
                    result.Data = data;
                    result.Success = data.StatusCode == HttpStatusCode.OK;
                    if (!result.Success)
                    {
                        result.Message = "Invalid status code " + data.StatusCode;
                        result.Reason = CrawlReason.InvalidStatusCode;
                    }
                }
            }
            catch (WebException exc)
            {
                result.Message = exc.Message;
                result.Reason = CrawlReason.Unknown;
                result.Exception = exc;
                return result;
            }
            return result;
        }

        private static bool ValidateClientCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
        {
            return true;
        }

        private static MemoryStream GetMemoryStream(Stream source)
        {
            var result = new MemoryStream();
            const int bufferSize = 1024;
            var buffer = new byte[bufferSize];
            int bytesRead;
            while ((bytesRead = source.Read(buffer, 0, bufferSize)) > 0)
            {
                result.Write(buffer, 0, bytesRead);
            }
            result.Seek(0, SeekOrigin.Begin);
            return result;
        }

        public void Dispose()
        {
            _provider.Close();
            Logging.Log(LogType.Info, "Crawl Service Provider Closed");
        }
    }
}
