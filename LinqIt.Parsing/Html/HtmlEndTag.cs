using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Parsing.Html
{
    public class HtmlEndTag : HtmlElement
    {
        public HtmlEndTag(Token token)
        {
            token.MovePast("</");
            TagName = token.ReadUntil(">").ToLower();
            token.MovePast(">");
        }

        public HtmlEndTag(string name)
        {
            TagName = name;
        }

        public string TagName { get; internal set; }

        public override string ToString()
        {
            return "</" + TagName.ToLower() + ">";
        }

        internal override void WriteTo(HtmlWriter writer)
        {
            //writer.Decrement();
            //writer.WriteFormat("</{0}>", TagName);
            //writer.NewLine();
        }

        public override HtmlElementType ElementType
        {
            get { return HtmlElementType.EndTag; }
        }

        public HtmlTag StartTag { get; internal set; }
    }
}
