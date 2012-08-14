using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Parsing.Css
{
    public class CssRuleMatch
    {
        public CssRuleMatch(CssRule rule)
        {
            Rule = rule;
        }

        public bool Success { get; internal set; }

        public CssSpecificity Specificity { get; internal set; }

        public CssRule Rule { get; private set; }
    }
}
