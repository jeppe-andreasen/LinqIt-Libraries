using System.Linq;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Html
{
    public class HtmlDocType : HtmlElement
    {
        public HtmlDocType(Token token)
        {
            token.Skip("<!DOCTYPE");
            Values = token.ReadUntil(">").Split(' ').Where(s => !string.IsNullOrEmpty(s.Trim())).ToArray();
            token.MovePast(">");
        }

        public override HtmlElementType ElementType
        {
            get { return HtmlElementType.DocType; }
        }

        public string[] Values { get; set; }

        public override string ToString()
        {
            return "<!DocType>";
        }

        internal override void WriteTo(HtmlWriter writer)
        {
            writer.Write("<!DOCTYPE " + Values.ToSeparatedString(" ") + ">");
            writer.NewLine();
        }
    }
}