using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.UI;
using LinqIt.Utils.Collections;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Html
{
    public class HtmlTag : HtmlElement
    {
        private readonly HtmlStyleCollection _styles;

        public HtmlTag(Token token)
        {
            token.Next();
            Name = token.ReadUntil(' ', '/', '>');

            ReadAttributes(token);
            Attributes.Changed += OnAttributesChanged;

            _styles = new HtmlStyleCollection();
            _styles.Changed += OnStylesChanged;
            UpdateStyles();

            token.MoveToContent();

            Closed = token.Peeks('/');

            token.MovePast(">");
        }

        private void UpdateStyles()
        {
            _styles.BeginUpdate();
            _styles.Parse(Attributes["style"]);
            _styles.EndUpdate();
        }

        private void UpdateAttributes()
        {
            Attributes.BeginUpdate();
            Attributes["style"] = _styles.Keys.ToSeparatedString("", k => string.Format("{0}:{1};", k, _styles[k]));
            Attributes.EndUpdate();
        }

        void OnStylesChanged(object sender, EventArgs e)
        {
            UpdateAttributes();
        }

        void OnAttributesChanged(object sender, EventArgs e)
        {
            UpdateStyles();
        }

        public HtmlStyleCollection Styles
        {
            get { return _styles; }
        }

        public bool Closed { get; private set; }

        //private void ReadChildren(Token token)
        //{
        //    bool done = false;
        //    while (!done)
        //    {
        //        token.MoveToContent();
        //        if (token.IsDone || token.Peeks("</" + Name + ">"))
        //            done = true;
        //        else
        //        {
        //            HtmlElement child = HtmlElement.Parse(this, token);
        //            if (child != null)
        //                ChildElements.Add(child);
        //        }
        //    }
        //}

        public override HtmlElementType ElementType
        {
            get { return HtmlElementType.Tag; }
        }

        public string Name { get; set; }

        public override string ToString()
        {
            var writer = new HtmlWriter();
            WriteTo(writer);
            return writer.ToString();
        }

        public bool IsType(string type)
        {
            return string.Compare(type, Name, true) == 0;
        }

        internal override void WriteTo(HtmlWriter writer)
        {
            if (IsType("br"))
            {
                writer.Write("<br />");
                return;
            }
            else if (IsType("hr"))
            {
                writer.Write("<hr />");
                return;
            }



            writer.Write("<");
            writer.Write(Name.ToLower());
            foreach (var key in Attributes.Keys)
            {
                writer.Write(" ");
                writer.WriteFormat(" {0}=\"{1}\"", key, Attributes[key]);
            }

            if (Closed)
            {
                writer.Write(" />");
            }
            else
            {
                writer.Write(">");
                if (ChildElements.Count > 1)
                {
                    writer.NewLine();
                    if (!IsType("html"))
                        writer.Increment();
                    foreach (var childElement in ChildElements)
                    {
                        childElement.WriteTo(writer);
                        writer.NewLine();
                    }
                    if (!IsType("html"))
                        writer.Decrement();
                }
                else
                {
                    foreach (var childElement in ChildElements)
                        childElement.WriteTo(writer);                                        
                }
                if (EndTag != null)
                    writer.WriteFormat("</{0}>", Name.ToLower());
            }
        }

        internal void AssignChildElements(List<HtmlElement> list)
        {
            foreach (var element in list)
            {
                ChildElements.Add(element);
                element.Parent = this;
            }
        }

        public HtmlEndTag EndTag { get; internal set; }

        public override void Remove()
        {
            if (EndTag != null)
            {
                Parent.ChildElements.Remove(EndTag);
            }
            Parent.ChildElements.Remove(this);
        }

        internal HtmlTag PreviousSibling()
        {
            var siblings = Parent.ChildElements.Where(c => c.ElementType == HtmlElementType.Tag).Cast<HtmlTag>().ToList();
            var index = siblings.IndexOf(this);
            if (index > 0)
                return siblings[index - 1];
            return null;
        }

        public override string Path
        {
            get 
            { 
                var path = Parent.Path;
                if (!string.IsNullOrEmpty(path))
                    path += "/";
                path += Name;
                return path;
            }
        }

        public override string InnerText
        {
            get
            {
                var result = new StringBuilder();
                foreach (var child in ChildElements)
                {
                    var text = child.InnerText.Trim(' ');
                    if (!string.IsNullOrEmpty(text))
                    {
                        if (result.Length > 0)
                            result.Append(" ");
                        result.Append(text);
                    }
                }
                return result.ToString();
            }
        }

        internal override string InnerContent
        {
            get
            {
                if (this.IsType("script"))
                    return string.Empty;

                var result = new StringBuilder();
                foreach (var text in ChildElements.Select(child => child.InnerContent.Trim(' ')).Where(text => !string.IsNullOrEmpty(text)))
                {
                    if (result.Length > 0)
                        result.Append(" ");
                    result.Append(text);
                }
                return result.ToString();
            }
        }

        public string InnerHtml
        {
             get
             {
                 var writer = new HtmlWriter();
                 foreach (var child in ChildElements)
                     child.WriteTo(writer);
                 return writer.ToString();
             }
            set
            {
                HtmlDocument doc = new HtmlDocument(value);
                ChildElements.Clear();
                ChildElements.AddRange(doc.ChildElements);
            }
        }

        public void CleanUpChildren()
        {
            while (ChildElements.Count() > 0 && ChildElements.First().Matches("br"))
            {
                ChildElements.First().Remove();
            }
            while (ChildElements.Count() > 0 && ChildElements.Last().Matches("br"))
                ChildElements.Last().Remove();

            FindElements(IsUnrelatedEndTag).ToList().ForEach(e => e.Remove());
            var paragraphs = FindTags("p").ToArray();
            for (int i = 0; i < paragraphs.Length; i++)
            {
                var p = paragraphs[i];
                if (p.EndTag == null)
                {
                    p.CreateEndTag();
                    int startIndex = p.Parent.ChildElements.IndexOf(p) + 1;
                    int endIndex = i == paragraphs.Length -1? p.Parent.ChildElements.Count : p.Parent.ChildElements.IndexOf(paragraphs[i+1]);
                    var itemsToMove = p.Parent.ChildElements.Skip(startIndex).Take(endIndex - startIndex).ToArray();
                    foreach (var item in itemsToMove)
                    {
                        p.Parent.ChildElements.Remove(item);
                        p.ChildElements.Add(item);
                    }
                    p.Parent.ChildElements.Insert(p.Parent.ChildElements.IndexOf(p)+1, p.EndTag);
                }
            }
        }

        private static bool IsUnrelatedEndTag(HtmlElement e)
        {
            if (e.ElementType != HtmlElementType.EndTag)
                return false;

            return ((HtmlEndTag) e).StartTag == null;
        }

        private void CreateEndTag()
        {
            if (this.EndTag != null)
                return;
            this.EndTag = new HtmlEndTag(this.Name);
            EndTag.StartTag = this;
            
        }

        public override bool Matches(Css.CssSelectorStack stack)
        {
            return stack.IsMatch(this);
        }

        public void ChangeType(string type)
        {
            this.Name = type;
            if (this.EndTag != null)
                this.EndTag.TagName = type;
        }
    }
}
