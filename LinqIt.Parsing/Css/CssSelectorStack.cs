using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqIt.Parsing.Html;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Css
{
    public class CssSelectorStack
    {
        private readonly List<CssSelector> _selectors;
        private readonly CssSpecificity _specificity;

        public CssSelectorStack(List<CssSelector> selectors)
        {
            _selectors = selectors;
            _specificity = selectors.Select(s => s.GetSpecificity()).Sum();
        }

        public List<CssSelector> Selectors { get { return _selectors; }}

        public CssSpecificity Specificity { get { return _specificity; } }

        public override string ToString()
        {
            return _selectors.ToSeparatedString(" ");
        }

        public static CssSelectorStack Parse(string text)
        {
            var selectors = new List<CssSelector>();
            foreach (var data in text.Trim().Split(' ').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s)))
            {
                var selector = CssSelector.Parse(data);
                if (selector != null)
                    selectors.Add(selector);
                else
                    return null;
            }

            var i = 1;
            var selectorsToRemove = new List<CssSelector>();
            while (i < selectors.Count-1)
            {
                if (selectors[i] is CssSiblingCombinator)
                {
                    var combinator = (CssSiblingCombinator) selectors[i];
                    combinator.X = selectors[i - 1];
                    combinator.Y = selectors[i + 1];
                    if (!selectorsToRemove.Contains(combinator.X))
                        selectorsToRemove.Add(combinator.X);
                    if (!selectorsToRemove.Contains(combinator.Y))
                        selectorsToRemove.Add(combinator.Y);
                    i += 2;
                }
                else
                    i++;
            }

            foreach (var selector in selectorsToRemove)
                selectors.Remove(selector);
            

            var invalidCombinator = selectors.Where(s => s is CssSiblingCombinator).Cast<CssSiblingCombinator>().Where(c => c.X == null || c.Y == null).FirstOrDefault();
            if (invalidCombinator != null)
            {
                throw new ApplicationException("Unable to parse " + invalidCombinator.GetType().Name + ". [" + text + "]");
            }

            return new CssSelectorStack(selectors);
        }

        internal string GetDebuggingString()
        {
            return _selectors.ToSeparatedString(" ") + "(" + _specificity + ")";
        }

        public bool IsMatch(HtmlTag tag)
        {
            var tmp = tag;
            var selectorIndex = Selectors.Count - 1;
            while (selectorIndex >= 0 && tmp != null)
            {
                if (Selectors[selectorIndex].Matches(tmp))
                    selectorIndex--;
                else if (selectorIndex == Selectors.Count - 1)
                {
                    break;
                }
                tmp = tmp.Parent as HtmlTag;
            }
            return selectorIndex == -1;
        }
    }
}
