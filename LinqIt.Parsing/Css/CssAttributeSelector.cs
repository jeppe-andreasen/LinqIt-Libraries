using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqIt.Parsing.Html;

namespace LinqIt.Parsing.Css
{
    public abstract class CssAttributeSelector
    {
        private static readonly Regex _regex = new Regex(@"^(?<attribute>[a-z][a-z-]*)(?:(?<expression>(?<operator>=|\*=|\^=|\$=|~=)""(?<value>[^""]*)"")|(?<expression>(?<operator>\||\*\||\^\||\$\||~\|)'(?<value>[^']*)'))?$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

        public static CssAttributeSelector Parse(string text)
        {
            text = text.Trim('[', ']');
            Match match = _regex.Match(text);
            if (!match.Success)
                return null;
            if (match.Groups["expression"].Success)
            {
                switch(match.Groups["operator"].Value)
                {
                    case "=":
                    case"|":
                        return new CssEqualsAttributeSelector(match.Groups["attribute"].Value, match.Groups["value"].Value);
                    case "*=":
                    case "*|":
                        return new CssWildcardAttributeSelector(match.Groups["attribute"].Value, match.Groups["value"].Value);
                    case "~=":
                    case "~|":
                        return new CssAnyAttributeSelector(match.Groups["attribute"].Value, match.Groups["value"].Value);
                    case "^=":
                    case "^|":
                        return new CssStartsWithAttributeSelector(match.Groups["attribute"].Value, match.Groups["value"].Value);
                    case "$=":
                    case "$|":
                        return new CssEndsWithAttributeSelector(match.Groups["attribute"].Value, match.Groups["value"].Value);
                    default:
                        throw new ApplicationException("Unknown attribute selector type");
                }
            }
            return new CssContainsAttributeSelector(match.Groups["attribute"].Value);

        }

        public abstract bool Matches(HtmlTag tag);

    }

    public class CssContainsAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;

        public CssContainsAttributeSelector(string attributeName)
        {
            _attributeName = attributeName;
        }

        public override bool Matches(HtmlTag tag)
        {
            return tag.Attributes.HasKey(_attributeName);
        }
    }

    public class CssEqualsAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;
        private readonly string _value;

        public CssEqualsAttributeSelector(string attributeName, string value)
        {
            _attributeName = attributeName;
            _value = value;
        }

        public override bool Matches(HtmlTag tag)
        {
            return tag.Attributes[_attributeName] == _value;
        }
    }

    public class CssWildcardAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;
        private readonly string _value;

        public CssWildcardAttributeSelector(string attributeName, string value)
        {
            _attributeName = attributeName;
            _value = value;
        }

        public override bool Matches(HtmlTag tag)
        {
            var value = tag.Attributes[_attributeName];
            if (value == null)
                return false;
            return value.Contains(_value);
        }
    }

    public class CssAnyAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;
        private readonly string _value;

        public CssAnyAttributeSelector(string attributeName, string value)
        {
            _attributeName = attributeName;
            _value = value;
        }

        public override bool Matches(HtmlTag tag)
        {
            var value = tag.Attributes[_attributeName];
            if (string.IsNullOrEmpty(value))
                return false;
            return value.Split(' ').Contains(_value);
        }
    }

    public class CssStartsWithAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;
        private readonly string _value;

        public CssStartsWithAttributeSelector(string attributeName, string value)
        {
            _attributeName = attributeName;
            _value = value;
        }

        public override bool Matches(HtmlTag tag)
        {
            var value = tag.Attributes[_attributeName];
            if (string.IsNullOrEmpty(value))
                return false;
            return value.StartsWith(_value);
        }
    }

    public class CssEndsWithAttributeSelector : CssAttributeSelector
    {
        private readonly string _attributeName;
        private readonly string _value;

        public CssEndsWithAttributeSelector(string attributeName, string value)
        {
            _attributeName = attributeName;
            _value = value;
        }

        public override bool Matches(HtmlTag tag)
        {
            var value = tag.Attributes[_attributeName];
            if (string.IsNullOrEmpty(value))
                return false;
            return value.EndsWith(_value);
        }
    }

    
}
