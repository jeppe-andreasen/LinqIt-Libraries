using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinqIt.Parsing.Html
{
    //public class HtmlAttributeCollection : IEnumerable<HtmlAttribute>
    //{
    //    private readonly Dictionary<string, HtmlAttribute> _attributes;

    //    public HtmlAttributeCollection()
    //    {
    //        _attributes = new Dictionary<string, HtmlAttribute>();
    //    }

    //    public HtmlAttributeCollection(string head)
    //    {
    //        _attributes = new Dictionary<string, HtmlAttribute>();
    //        var match = Regex.Match(head,
    //                                "^<[a-z0-9_:]+(?<attr>\\s+[a-z0-9_]+(?:\\s*=\\s*(?<c>\"|')(?:.|\\n)*?\\k<c>)?)*\\s*/?>",
    //                                RegexOptions.Multiline | RegexOptions.IgnoreCase);
    //        if (match.Success)
    //        {
    //            foreach (Capture attr in match.Groups["attr"].Captures)
    //            {
    //                var index = attr.Value.IndexOf('=');
    //                var name = index == -1 ? attr.Value.Trim() : attr.Value.Substring(0, index).Trim();
    //                var value = index == -1 ? string.Empty : attr.Value.Substring(index + 1).Trim();
    //                if (value.StartsWith("\"") || value.StartsWith("'"))
    //                    value = value.Substring(1, value.Length - 2);
    //                if (!ContainsAttribute(name))
    //                    SetValue(name, value);
    //            }
    //        }
    //    }

    //    #region IEnumerable<HtmlAttribute> Members

    //    public IEnumerator<HtmlAttribute> GetEnumerator()
    //    {
    //        return _attributes.Values.AsEnumerable().GetEnumerator();
    //    }

    //    System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
    //    {
    //        return _attributes.Values.GetEnumerator();
    //    }

    //    #endregion

    //    public bool ContainsAttribute(string attributeName)
    //    {
    //        return _attributes.ContainsKey(attributeName.ToLower());
    //    }

    //    public HtmlAttribute GetAttribute(string attributeName)
    //    {
    //        if (ContainsAttribute(attributeName))
    //            return _attributes[attributeName.ToLower()];
    //        else
    //            return null;
    //    }

    //    public void SetValue(string attributeName, string value)
    //    {
    //        var attribute = GetAttribute(attributeName);
    //        if (attribute == null)
    //        {
    //            attribute = new HtmlAttribute(attributeName, value);
    //            _attributes.Add(attributeName.ToLower(), attribute);
    //        }
    //        else
    //        {
    //            attribute.Name = attributeName;
    //            attribute.Value = value;
    //        }
    //    }

    //    public void RemoveAttribute(string attributeName)
    //    {
    //        _attributes.Remove(attributeName.ToLower());
    //    }

    //    public string GetValue(params string[] attributeNames)
    //    {
    //        foreach (var name in attributeNames)
    //        {
    //            var attribute = GetAttribute(name);
    //            if (attribute != null)
    //                return attribute.Value;
    //        }
    //        return null;
    //    }
    //}
}