namespace LinqIt.Parsing.Html
{
    public class HtmlContent : HtmlElement
    {
        public HtmlContent(Token token)
        {
            Value = token.ReadUntil('<').Trim();
        }

        public HtmlContent(string content)
        {
            Value = content;
        }

        public override HtmlElementType ElementType
        {
            get { return HtmlElementType.Content; }
        }

        public string Value { get; set; }

        public override string ToString()
        {
            return Value;
        }

        internal override void WriteTo(HtmlWriter writer)
        {
            writer.Write(Value);
        }

        public override string InnerText
        {
            get { return Value; }
        }

        internal override string InnerContent
        {
            get { return Value.Trim(); }
        }
        
    }
}