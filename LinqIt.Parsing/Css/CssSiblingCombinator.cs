using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Parsing.Html;

namespace LinqIt.Parsing.Css
{
    public abstract class CssSiblingCombinator : CssSelector
    {
        internal CssSelector X { get; set; }

        internal CssSelector Y { get; set; }

        internal override CssSpecificity GetSpecificity()
        {
            return X.GetSpecificity() + Y.GetSpecificity();
        }
    }

    public class CssAdjacentCombinator : CssSiblingCombinator
    {
        internal override bool Matches(Html.HtmlTag tag)
        {
            if (!Y.Matches(tag))
                return false;

            var previousSibling = tag.PreviousSibling();
            return previousSibling != null && X.Matches(previousSibling);
        }

        public override string ToString()
        {
            return X + " + " + Y;
        }
    }

    public class CssDirectChildCombinator : CssSiblingCombinator
    {
        internal override bool Matches(Html.HtmlTag tag)
        {
            if (tag.Parent is HtmlTag)
            {
                return X.Matches((HtmlTag)tag.Parent) && Y.Matches(tag);
            }
            return false;
        }

        public override string ToString()
        {
            return X + " > " + Y;
        }
    }

    public class CssFollowSiblingCombinator : CssSiblingCombinator
    {
        internal override bool Matches(Html.HtmlTag tag)
        {
            if (!Y.Matches(tag))
                return false;

            var index = tag.Parent.ChildElements.IndexOf(tag) - 1;
            while (index >= 0)
            {
                var element = tag.Parent.ChildElements[index];
                if (element.ElementType == HtmlElementType.Tag && X.Matches((HtmlTag)element))
                    return true;
                index--;
            }
            return false;
        }

        public override string ToString()
        {
            return X + " ~ " + Y;
        }

        
    }
}
