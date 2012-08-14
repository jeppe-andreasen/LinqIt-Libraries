using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace LinqIt.Parsing.Html
{
    public class HtmlDocument : IDisposable, IHtmlElementCollection
    {
        private readonly List<HtmlElement> _elements;

        public HtmlDocument(string html)
        {
            _elements = new List<HtmlElement>();
            var token = new Token(html);
            token.MoveToContent();
            while (!token.IsDone)
            {
                HtmlElement.Parse(this, token);
                token.MoveToContent();
            }
        }

        public List<HtmlElement> ChildElements
        {
            get { return _elements; }
        }

        public IHtmlElementCollection Parent
        {
            get { return null; }
            set{}
        }

        #region IDisposable Members

        public void Dispose()
        {
        }

        #endregion

        public IEnumerable<HtmlElement> FindElements(Func<HtmlElement, bool> evaluator)
        {
            var result = new List<HtmlElement>();
            foreach (var e in _elements)
                EvaluateElement(e, result, evaluator);
            return result;
        }

        public IEnumerable<HtmlTag>FindTagsByName(params string[] names)
        {
            if (names == null || names.Length == 0)
                return new HtmlTag[0];

            return FindElements(e => e.ElementType == HtmlElementType.Tag && names.Where(n => n.Equals(((HtmlTag)e).Name, StringComparison.InvariantCultureIgnoreCase)).Any()).Cast<HtmlTag>();
        }

        public HtmlTag FindTagById(string id)
        {
            return FindElements(e => e.ElementType == HtmlElementType.Tag && e.Attributes["id"] == id).Cast<HtmlTag>().FirstOrDefault();
        }

        private static void EvaluateElement(HtmlElement e, List<HtmlElement> result, Func<HtmlElement, bool> evaluator)
        {
            if (evaluator(e))
                result.Add(e);
            foreach (var child in e.ChildElements)
                EvaluateElement(child, result, evaluator);
        }

        public override string ToString()
        {
            var result = new HtmlWriter();
            foreach (var element in _elements)
                element.WriteTo(result);
            return result.ToString();
        }

        public string Path { get { return string.Empty; } }

        public string ExtractContent()
        {
            var body = FindTagsByName("body").FirstOrDefault();
            return body == null ? string.Empty : body.InnerContent;
        }
    }
}