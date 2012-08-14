using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using LinqIt.Parsing.Css;
using LinqIt.Parsing.Html;
using LinqIt.Utils.Extensions;

namespace LinqIt.Mail
{
    public class CssInliner
    {
        private CssDocument _cssDocument;
        private Dictionary<HtmlTag, HtmlStyleCollection> _inlineStyles;

        public string Process(string html)
        {
            var htmlDocument = new LinqIt.Parsing.Html.HtmlDocument(html);
            var css = ExtractCss(htmlDocument);
            _cssDocument = new CssDocument(css);

            BackupStyles(htmlDocument);

            var bodyTag = ExtractBodyTag(htmlDocument);
            Process(bodyTag);

            OverwriteWithBackupStyles();


            return htmlDocument.ToString();
        }

        private void OverwriteWithBackupStyles()
        {
            foreach (var tag in _inlineStyles.Keys)
                tag.Styles.Append(_inlineStyles[tag], true);
        }

        private void BackupStyles(HtmlDocument htmlDocument)
        {
            _inlineStyles = new Dictionary<HtmlTag, HtmlStyleCollection>();
            foreach (var tag in htmlDocument.FindElements(e => e.ElementType == HtmlElementType.Tag).Cast<HtmlTag>().Where(tag => tag.Styles.Count > 0))
            {
                _inlineStyles.Add(tag, tag.Styles.CloneAs<HtmlStyleCollection>());
                tag.Styles.Clear();
            }
        }

        private void Process(HtmlTag tag)
        {
            foreach (var match in _cssDocument.Rules.Select(r => r.Match(tag)).Where(m => m.Success).OrderBy(m => m.Specificity))
                AppendStyles(match.Rule, tag);
            
            foreach (var childTag in tag.ChildElements.Where(c => c.ElementType == HtmlElementType.Tag))
                Process((HtmlTag) childTag);
        }

        private static void AppendStyles(CssRule rule, HtmlTag tag)
        {
            var style = rule.Statements.ToSeparatedString("");
            if (string.IsNullOrEmpty(style))
                return;

            var styles = new HtmlStyleCollection();
            styles.Parse(style);
            tag.Styles.Append(styles, true);
        }

        private static HtmlTag ExtractBodyTag(HtmlDocument htmlDocument)
        {
            return htmlDocument.FindTagsByName("body").FirstOrDefault();
        }

        private static string ExtractCss(HtmlDocument doc)
        {
            var styleTags = doc.FindTagsByName("style", "link").ToArray();
            var css = new StringBuilder();
            foreach (var tag in styleTags)
            {
                if (tag.IsType("style"))
                    css.AppendLine(tag.ChildElements.ToSeparatedString(" "));
                else if (tag.IsType("link") && (string.Compare(tag.Attributes["type"], "text/css", true) == 0 || string.Compare(tag.Attributes["rel"], "stylesheet") == 0))
                {
                    var src = tag.Attributes["href"];
                    if (string.IsNullOrEmpty(src))
                        continue;
                    var uri = new Uri(src, UriKind.RelativeOrAbsolute);
                    if (!uri.IsAbsoluteUri && HttpContext.Current != null && HttpContext.Current.Request != null)
                    {
                        uri = new Uri(HttpContext.Current.Request.Url, uri);
                    }
                    if (!uri.IsAbsoluteUri)
                        continue;
                    var client = new WebClient();
                    css.Append(client.DownloadString(uri));
                }
            }
            foreach (var tag in styleTags)
            {
                tag.Remove();
            }
            return css.ToString();
            
        }
    }
}
