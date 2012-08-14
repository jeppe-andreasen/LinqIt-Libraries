using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqIt.Parsing.Css
{
    public class CssSimpleSelector : CssSelector
    {
        private readonly string _typeName;
        private readonly string _id;
        private readonly string _pseudoclass;
        private readonly string _fontface;
        private readonly CssAttributeSelector _attributeSelector;
        private readonly string[] _classes;

        public CssSimpleSelector(Match match)
        {
            if (match.Groups["type"].Success)
                _typeName = match.Groups["type"].Value;
            if (match.Groups["id"].Success)
                _id = match.Groups["id"].Value.Substring(1);
            if (match.Groups["class"].Success)
                _classes = match.Groups["class"].Captures.Cast<Capture>().Select(c => c.Value.Substring(1)).ToArray();
            if (match.Groups["pseudoclass"].Success)
                _pseudoclass = match.Groups["pseudoclass"].Value.Substring(1);
            if (match.Groups["fontface"].Success)
                _fontface = match.Groups["fontface"].Value.Substring(1);
            if (match.Groups["attr"].Success)
                _attributeSelector = CssAttributeSelector.Parse(match.Groups["attr"].Value);
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            if (!string.IsNullOrEmpty(_typeName))
                result.Append(_typeName);
            if (!string.IsNullOrEmpty(_id))
                result.Append("#" + _id);
            else if (_classes != null)
            {
                foreach (var c in _classes)
                    result.Append("." + c);
            }
            if (!string.IsNullOrEmpty(_pseudoclass))
                result.Append(":" + _pseudoclass);
            if (!string.IsNullOrEmpty(_fontface))
                result.Append("@" + _fontface);
            return result.ToString();
        }

        internal override bool Matches(Html.HtmlTag tag)
        {
            if (!string.IsNullOrEmpty(_typeName) && _typeName != "*" && !_typeName.Equals(tag.Name, StringComparison.InvariantCultureIgnoreCase))
                return false;
            if (!string.IsNullOrEmpty(_id) && _id != tag.Attributes["id"])
                return false;
            if (_classes != null && _classes.Any())
            {
                if (!tag.Attributes.HasKey("class"))
                    return false;
                var classValue = tag.Attributes["class"];
                if (string.IsNullOrEmpty(classValue))
                    return false;
                var classes = classValue.Split(' ').Select(s => s.Trim()).ToArray();
                if (_classes.Where(c => !classes.Contains(c)).Any())
                    return false;
            }
            if (_attributeSelector != null && !_attributeSelector.Matches(tag))
                return false;
            return true;
        }

        internal override CssSpecificity GetSpecificity()
        {
            var ids = !string.IsNullOrEmpty(_id) ? 1 : 0;
            var classes = !string.IsNullOrEmpty(_classes + _pseudoclass) ? 1 : 0;
            var typenames = !string.IsNullOrEmpty(_typeName) ? 1 : 0;
            return new CssSpecificity(0, ids, classes, typenames);
        }
    }
}
