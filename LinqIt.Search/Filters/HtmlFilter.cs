using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqIt.Parsing.Html;
using System.Collections.Specialized;
using LinqIt.Search.Configuration;
using LinqIt.Utils.Collections;

namespace LinqIt.Search.Filters
{
    public class HtmlFilter : Filter
    {
        private List<KeyValuePair<string, string>> _indexStrips;
        private List<KeyValuePair<string, string>> _followStrips;

        protected internal override void Initialize(IFilterConfiguration configuration)
        {
            base.Initialize(configuration);

            #region Initialize Strips
            _indexStrips = new List<KeyValuePair<string,string>>();
            _followStrips = new List<KeyValuePair<string,string>>();
            foreach (StripConfigurationElement stripConfiguration in configuration)
            {
                var strip = new KeyValuePair<string, string>(stripConfiguration.StartTag, stripConfiguration.EndTag);
                if (stripConfiguration.Type == StripTextConfigurationType.index)
                    _indexStrips.Add(strip);
                else
                    _followStrips.Add(strip);
            }
            #endregion
        }


        public override bool Process(CrawlData crawlData)
        {
            try
            {
                DoExtractContent(crawlData);
                DoExtractLinks(crawlData);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private void DoExtractContent(CrawlData crawlData)
        {
            var text = _indexStrips.Aggregate(crawlData.OriginalContent, (current, strip) => StripText(current, strip.Key, strip.Value));
            var html = new HtmlDocument(text);

            crawlData.Title = html.FindTagsByName("title").Select(t => t.InnerText).FirstOrDefault();

            var metaData = new CaseInvariantNameValueCollection(); 
            foreach (var keyValue in html.FindTagsByName("meta").Select(GetKeyValue).Where(keyValue => keyValue.HasValue))
            {
                if (metaData.HasKey(keyValue.Value.Key))
                    throw new ApplicationException("Duplicate meta tags:" + keyValue.Value.Key);
                metaData.Append(keyValue.Value.Key, keyValue.Value.Value);
            }
            crawlData.MetaData = metaData.ToDictionary();
            crawlData.FilteredContent = html.ExtractContent();
        }

        private static KeyValuePair<string, string>? GetKeyValue(HtmlTag meta)
        {
            var name = meta.Attributes["name"];
            var equiv = meta.Attributes["http-equiv"];
            string key;
            if (!string.IsNullOrEmpty(name))
                key = name;
            else if (!string.IsNullOrEmpty(equiv))
                key = equiv;
            else
                return null;

            var value = meta.Attributes["content"] ?? string.Empty;

            return new KeyValuePair<string, string>(key.ToLower(), value);
        }

        private void DoExtractLinks(CrawlData data)
        {
            var text = _followStrips.Aggregate(data.OriginalContent, (current, strip) => StripText(current, strip.Key, strip.Value));

            var html = new HtmlDocument(text);
            data.Links = html.FindTagsByName("a").Select(a => a.Attributes["href"]).ToArray();
        }

        private static string StripText(string text, string beginTag, string endTag)
        {
            var a = Regex.Escape(beginTag);
            var b = Regex.Escape(endTag);
            var pattern = string.Format("({0})(.*?)({1})", a, b);
            const RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.CultureInvariant | RegexOptions.Compiled;
            return Regex.Replace(text, pattern, string.Empty, options);
        }
    }
}
