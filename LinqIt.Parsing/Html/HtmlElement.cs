using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using LinqIt.Parsing.Css;
using LinqIt.Utils.Collections;


namespace LinqIt.Parsing.Html
{
    public enum HtmlElementType { Tag, Comment, DocType, Content, EndTag }

    public abstract class HtmlElement : IHtmlElementCollection
    {
        private readonly CaseInvariantNameValueCollection _attributes;

        protected HtmlElement()
        {
            _attributes = new CaseInvariantNameValueCollection();
            ChildElements = new List<HtmlElement>();
        }

        public static HtmlElement Parse(string html)
        {
            return null;
        }

        public List<HtmlElement> ChildElements { get; private set; }

        public IHtmlElementCollection Parent { get; set; }

        internal static void Parse(IHtmlElementCollection parent, Token token)
        {
            HtmlElement element = null;
            if (token.Peeks("</"))
                element = new HtmlEndTag(token);
            else if (token.Peeks("<!DOCTYPE"))
                element = new HtmlDocType(token);
            else if (token.Peeks("<!--"))
                element = new HtmlComment(token);
            else if (token.Current == '<')
                element = new HtmlTag(token);
            else
                element = new HtmlContent(token);

            element.Parent = parent;
            parent.ChildElements.Add(element);
            
            if (element is HtmlTag)
            {
                var tag = (HtmlTag)element;
                if (tag.IsType("script"))
                {
                    HtmlContent script = new HtmlContent(token.ReadUntil("</script>"));
                    tag.ChildElements.Add(script);
                }
            }

            if (element.ElementType != HtmlElementType.EndTag) 
                return;

            var tagName = ((HtmlEndTag) element).TagName;
            int startTagIdex = parent.ChildElements.Count - 2;
            HtmlTag startTag = null;
            while (startTagIdex >= 0)
            {
                var tmp = parent.ChildElements[startTagIdex] as HtmlTag;
                if (tmp != null && !tmp.Closed && tmp.EndTag == null && tmp.Name.Equals(tagName, StringComparison.InvariantCultureIgnoreCase))
                {
                    startTag = tmp;
                    break;
                }
                startTagIdex--;
            }
            if (startTag != null)
            {
                startTag.EndTag = (HtmlEndTag) element;
                ((HtmlEndTag) element).StartTag = startTag;
                int rangeStart = startTagIdex + 1;
                int rangeCount = parent.ChildElements.Count - startTagIdex - 2;
                startTag.AssignChildElements(parent.ChildElements.GetRange(rangeStart, rangeCount));
                parent.ChildElements.RemoveRange(rangeStart, rangeCount);
            }
        }

        public abstract HtmlElementType ElementType { get; }

        protected void ReadAttributes(Token token)
        {
            var done = false;
            while (!done)
            {
                token.MoveToContent();
                if (token.Peeks("/>") || token.Peeks(">") || token.IsDone)
                    done = true;
                else
                {
                    string attributeName = token.ReadUntil(' ', '=');
                    token.MovePast("=");
                    token.MoveToContent();
                    string attributeValue;                    
                    if (token.Current == '\'' || token.Current == '"')
                    {
                        char delimiter = token.Current;
                        token.Next();
                        attributeValue = token.ReadUntil(delimiter);
                        token.Next();
                    }
                    else
                        attributeValue = token.ReadUntil(' ', '/', '>');
                    _attributes[attributeName] = attributeValue;
                }
            }
        }

        public CaseInvariantNameValueCollection Attributes
        {
            get { return _attributes; }
        }

        public IEnumerable<HtmlElement> FindElements(Func<HtmlElement, bool> evaluator)
        {
            var result = new List<HtmlElement>();
            foreach (HtmlElement e in ChildElements)
                EvaluateElement(e, result, evaluator);
            return result;
        }

        private static void EvaluateElement(HtmlElement e, List<HtmlElement> result, Func<HtmlElement, bool> evaluator)
        {
            if (evaluator(e))
                result.Add(e);
            foreach (HtmlElement child in e.ChildElements)
                EvaluateElement(child, result, evaluator);
        }

        internal abstract void WriteTo(HtmlWriter writer);

        public virtual void Remove()
        {
            Parent.ChildElements.Remove(this);
        }
    
        public virtual string Path
        {
            get { return Parent.Path; }
        }

        public virtual string InnerText { get { return string.Empty;  } }

        internal virtual string InnerContent { get { return string.Empty; } }

        public HtmlTag FindSingleTag(string query)
        {
            CssSelectorStack stack = CssSelectorStack.Parse(query);
            return FindElements(e => e.Matches(stack)).Cast<HtmlTag>().FirstOrDefault();
        }

        public IEnumerable<HtmlTag> FindTags(string query)
        {
            CssSelectorStack stack = CssSelectorStack.Parse(query);
            return FindElements(e => e.Matches(stack)).Cast<HtmlTag>().ToArray();
        }

        //private static bool EvaluateStack(HtmlElement element, CssSelectorStack stack)
        //{
        //    if (!(element is HtmlTag))
        //        return false;
        //    return stack.IsMatch((HtmlTag) element);
        //}


        public bool Matches(string query)
        {
            CssSelectorStack stack = CssSelectorStack.Parse(query);
            return Matches(stack);
        }

        public virtual bool Matches(CssSelectorStack stack)
        {
            return false;
        }
    }

}
