namespace LinqIt.Parsing.Html
{
    public class HtmlComment : HtmlElement
    {
        public HtmlComment(Token token)
        {
            token.Skip("<!--");
            Value = token.ReadUntil("-->").Trim();
            token.Skip("-->");
        }

        public override HtmlElementType ElementType
        {
            get { return HtmlElementType.Comment; }
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return "<!-- " + Value + "-->";
        }

        internal override void WriteTo(HtmlWriter writer)
        {
            writer.Write(this.ToString());
        }
    }
}