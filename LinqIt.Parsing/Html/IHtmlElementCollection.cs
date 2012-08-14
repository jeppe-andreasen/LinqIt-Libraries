using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Parsing.Html
{
    public interface IHtmlElementCollection
    {
        List<HtmlElement> ChildElements { get; }
        IHtmlElementCollection Parent { get; set; }

        string Path { get;}
    }
}
