using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.IO;
using System.Text.RegularExpressions;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils.Web
{
    using System.Collections.Specialized;

    public class HtmlWriter : IDisposable
    {
        private readonly bool _isOwner;

        private readonly HtmlTextWriter _writer;

        private readonly StringBuilder _builder;

        private readonly TextWriter _textWriter;

        private readonly List<string> _classNames = new List<string>();

        private readonly NameValueCollection _styles = new NameValueCollection();

        private int _openCount = 0;

        /// <summary>
        /// Use this for debug information only, if html is buggy try to see if there is a unclosed tag.
        /// </summary>
        /// <remarks>
        /// For debug use only.
        /// </remarks>
        public int OpenCount
        {
            get { return _openCount; }
        }

        public HtmlWriter()
        {
            _builder = new StringBuilder();
            _textWriter = new StringWriter(_builder);
            _writer = new Html32TextWriter(_textWriter);
            _isOwner = true;
        }

        public HtmlWriter(HtmlTextWriter innerWriter)
        {
            _writer = innerWriter;
            _isOwner = false;
        }

        /// <summary>
        /// Tilføjer en class til elementet. Overskriver ikke tidliger tilføjet classes
        /// </summary>
        /// <param name="className">class navn</param>
        public void AddClass(string className)
        {
            if (!string.IsNullOrEmpty(className))
                _classNames.Add(className);
        }

        public void Write(object value)
        {
            if (value != null)
                _writer.Write(value.ToString());
        }

        public void AddClasses(params string[] classNames)
        {
            _classNames.AddRange(classNames);
        }

        public void AddStyle(HtmlTextWriterStyle style, string value)
        {
            _styles.Add(style.ToString().ToLower(), value);
        }

        public void AddStyle(string style, string value)
        {
            _styles.Add(style, value);
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        public void RenderBeginTag(string tag)
        {
            if (_classNames.Any())
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, _classNames.ToSeparatedString(" "));
            if (_styles.Count > 0)
                _writer.AddAttribute(HtmlTextWriterAttribute.Style, _styles.Keys.Cast<string>().ToSeparatedString(";", k => string.Format("{0}:{1}", k, _styles[k])));
            _writer.RenderBeginTag(tag);
            _classNames.Clear();
            _styles.Clear();
            _openCount++;
        }

        public void RenderBeginTag(HtmlTextWriterTag tag)
        {
            if (_classNames.Any())
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, _classNames.ToSeparatedString(" "));
            if (_styles.Count > 0)
                _writer.AddAttribute(HtmlTextWriterAttribute.Style, _styles.Keys.Cast<string>().ToSeparatedString(";", k => string.Format("{0}:{1}", k, _styles[k])));
            _writer.RenderBeginTag(tag);
            _classNames.Clear();
            _styles.Clear();
            _openCount++;
        }

        public void RenderBeginTag(HtmlTextWriterTag tag, string cssClass)
        {
            this.AddClass(cssClass);
            RenderBeginTag(tag);
        }

        public void RenderEndTag()
        {
            _openCount--;
            _writer.RenderEndTag();
        }

        public void RenderEndTags()
        {
            while (_openCount > 0)
                RenderEndTag();
        }

        public static string Generate(Action<HtmlWriter> action)
        {
            using (HtmlWriter writer = new HtmlWriter())
            {
                action(writer);
                return writer.ToString();
            }
        }

        #region IDisposable Members

        public void Dispose()
        {
            if (_isOwner)
            {
                _writer.Dispose();
                _textWriter.Dispose();
            }
        }

        #endregion

        public void RenderBeginDiv(string id, string cssClass)
        {
            if (!string.IsNullOrEmpty(id))
                _writer.AddAttribute(HtmlTextWriterAttribute.Id, id);
            if (!string.IsNullOrEmpty(cssClass))
                AddClasses(cssClass);
            RenderBeginTag(HtmlTextWriterTag.Div);
        }

        public void RenderBeginLink(string url)
        {
            _writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
            RenderBeginTag(HtmlTextWriterTag.A);
        }

        

        public void WriteBreak()
        {
            _writer.WriteBreak();
        }

        public void RenderFullTag(string tag, string content = null, string cssClass = null)
        {
            if (!string.IsNullOrEmpty(cssClass))
                AddClass(cssClass);
            RenderBeginTag(tag);
            if (!string.IsNullOrEmpty(content))
                Write(content);
            RenderEndTag();
        }

        public void RenderFullTag(HtmlTextWriterTag tag, string content)
        {
            RenderFullTag(tag, content, null);
        }

        public void RenderFullTag(HtmlTextWriterTag tag, string content, string cssClass)
        {
            if (!string.IsNullOrEmpty(cssClass))
                this.AddClasses(cssClass);
            RenderBeginTag(tag);

            if (!string.IsNullOrEmpty(content))
                _writer.Write(content);
            RenderEndTag();
        }

        public void RenderGrid<T>(int columnSize, int columnsPerLine, IEnumerable<T> elements, Action<T> action)
        {
            var n = 0;
            foreach (var element in elements)
            {
                AddClass("grid-" + columnSize);
                if (n % columnsPerLine == 0)
                    AddClass("grid-first");
                RenderBeginTag(HtmlTextWriterTag.Div);
                action(element);
                RenderEndTag();
                n++;
            }
        }

        public void RenderImageLink(string url, string imageUrl, string alt, string aClass, string imgClass)
        {
            _writer.AddAttribute(HtmlTextWriterAttribute.Href, url);
            if (!string.IsNullOrEmpty(aClass))
                this.AddClasses(aClass);
            RenderBeginTag(HtmlTextWriterTag.A);

            _writer.AddAttribute(HtmlTextWriterAttribute.Src, imageUrl);
            _writer.AddAttribute(HtmlTextWriterAttribute.Alt, alt);
            if (!string.IsNullOrEmpty(imgClass))
                this.AddClasses(imgClass);
            RenderBeginTag(HtmlTextWriterTag.Img);
            RenderEndTag();
            RenderEndTag();
        }

        public void RenderImageTag(string imageUrl, string alt, string imgClass)
        {
            _writer.AddAttribute(HtmlTextWriterAttribute.Src, imageUrl);
            _writer.AddAttribute(HtmlTextWriterAttribute.Alt, alt);
            if (!string.IsNullOrEmpty(imgClass))
                this.AddClasses(imgClass);
            RenderBeginTag(HtmlTextWriterTag.Img);
            RenderEndTag();
        }

        public void RenderLinkTag(string href, string content)
        {
            RenderBeginLink(href);
            _writer.RenderBeginTag(HtmlTextWriterTag.Span);
            _writer.Write(content);
            _writer.RenderEndTag();
            RenderEndTag();
        }

        public void RenderLinkTag(string url, string content, string cssClass)
        {
            AddClass(cssClass);
            RenderBeginLink(url);
            _writer.RenderBeginTag(HtmlTextWriterTag.Span);
            _writer.Write(content);
            _writer.RenderEndTag();
            RenderEndTag();
        }

        public void RenderLinkTag(string url, string content, string cssClass, string spanClass)
        {
            AddClass(cssClass);
            RenderBeginLink(url);
            AddClass(spanClass);
            RenderBeginTag(HtmlTextWriterTag.Span);
            _writer.Write(content);
            _writer.RenderEndTag();
            RenderEndTag();
        }

        
        public void RenderParagraph(string text)
        {
            RenderParagraph(text, null);
        }

        public void RenderParagraph(string text, string cssClass)
        {
            if (text == null)
                return;
            if (string.IsNullOrEmpty(text.Trim()))
                return;

            text = Regex.Replace(text, @"<[^>]+>", "");

            if (!string.IsNullOrEmpty(cssClass))
                _writer.AddAttribute(HtmlTextWriterAttribute.Class, cssClass);
            RenderBeginTag(HtmlTextWriterTag.P);

            _writer.Write(text);
            RenderEndTag();
        }

        public void AddAttribute(HtmlTextWriterAttribute attribute, string value)
        {
            _writer.AddAttribute(attribute, value);
        }

        public void AddAttribute(string attribute, string value)
        {
            _writer.AddAttribute(attribute, value);
        }

        public void RenderMailtoLink(string mail, string content, string cssClass)
        {
            if (string.IsNullOrEmpty(mail))
                return;

            AddClass(cssClass);
            AddAttribute(HtmlTextWriterAttribute.Title, string.Concat("Email : ", mail));
            AddAttribute(HtmlTextWriterAttribute.Rel, "email");
            if (!mail.Contains("mailto:"))
                AddAttribute(HtmlTextWriterAttribute.Href, string.Concat("mailto:", mail));

            RenderBeginTag(HtmlTextWriterTag.A);
            RenderBeginTag(HtmlTextWriterTag.Span);
            Write(content);
            RenderEndTag(); //span 
            RenderEndTag(); //a
        }

        public void RenderTags(string content, params HtmlTextWriterTag[] tags)
        {
            foreach (var tag in tags)
                RenderBeginTag(tag);

            _writer.Write(content);

            foreach (var tag in tags)
                RenderEndTag();
        }

        public void RenderHeadline(string headline)
        {
            if (string.IsNullOrWhiteSpace(headline))
                return;
            RenderFullTag(HtmlTextWriterTag.H1, headline);
        }

        public void RenderHeadlineEx(HtmlTextWriterTag hx, string headline, string subheadline)
        {
            if (string.IsNullOrWhiteSpace(headline))
                return;

            RenderBeginTag(HtmlTextWriterTag.Div, "headline");
            RenderBeginTag(hx);
            var parts = headline.Trim().Split(' ');
            RenderBeginTag(HtmlTextWriterTag.Span);
            _writer.Write(parts[0]);
            RenderEndTag();
            if (parts.Length > 1)
            {
                _writer.Write(" " + parts.Skip(1).ToSeparatedString(" "));
            }
            RenderEndTag();
            if (!string.IsNullOrWhiteSpace(subheadline))
                RenderFullTag(HtmlTextWriterTag.Span, subheadline, "subheadline");
            RenderEndTag();
            AddStyle("clear", "both");
            RenderFullTag(HtmlTextWriterTag.Div, null);
        }

        
    }
}
