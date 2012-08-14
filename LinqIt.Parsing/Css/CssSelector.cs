using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqIt.Parsing.Html;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Css
{
    public abstract class CssSelector
    {
        private static readonly Regex _parsingExpression = new Regex(@"^(?<type>[a-z*][a-z\d]*)?(:?(?<id>#[a-z_0-9]+)|(?<class>\.[a-z_0-9-]+)|(?<pseudoclass>\:[a-z_0-9-]+)|(?<typeface>@[a-z_0-9-]+))*(?<attr>\[[^\]]+\])?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static CssSelector Parse(string text)
        {
            if (text == "+")
                return new CssAdjacentCombinator();
            else if (text == ">")
                return new CssDirectChildCombinator();
            else if (text == "~")
                return new CssFollowSiblingCombinator();

            var match = _parsingExpression.Match(text);
            if (!match.Success)
                return null;
            return new CssSimpleSelector(match);
        }

        internal abstract bool Matches(HtmlTag tag);

        internal abstract CssSpecificity GetSpecificity();
    }
}
