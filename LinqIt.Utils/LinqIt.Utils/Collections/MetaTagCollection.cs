using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqIt.Utils.Collections
{
    public class MetaTagCollection
    {
        private readonly List<MetaTag> _tags = new List<MetaTag>();

        public void AddName(string name, string content)
        {
            _tags.Add(new MetaTag { Name = name, Content = content });
        }

        public void AddHttpEquiv(string httpEquiv, string content)
        {
            _tags.Add(new MetaTag { HttpEquiv = httpEquiv, Content = content });
        }

        public void AddContent(string content)
        {
            _tags.Add(new MetaTag { Content = content });
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var tag in _tags)
                result.AppendLine(tag.ToString());
            return result.ToString();
        }

        public class MetaTag
        {
            private string _content;
            public string Name { get; set; }
            public string HttpEquiv { get; set; }
            public string Content
            {
                get { return _content; }
                set
                {
                    var tmp = value;
                    if (!string.IsNullOrEmpty(tmp))
                    {
                        // Replace whitespace with a single space
                        tmp = Regex.Replace(tmp, @"\s+", " ", RegexOptions.Singleline);

                        // Strip Html Tags
                        tmp = Regex.Replace(tmp, @"<[^>]+>", "");

                        // Replace odd characters
                        tmp = tmp.Replace("\"", "");
                    }
                    _content = tmp;
                }
            }

            public override string ToString()
            {
                var result = new StringBuilder();
                result.Append("<meta");
                if (!string.IsNullOrEmpty(Name))
                    result.AppendFormat(" name=\"{0}\"", Name);
                if (!string.IsNullOrEmpty(HttpEquiv))
                    result.AppendFormat(" http-equiv=\"{0}\"", HttpEquiv);
                if (!string.IsNullOrEmpty(Content))
                    result.AppendFormat(" content=\"{0}\"", Content);
                result.Append(">");
                return result.ToString();
            }
        }
    }
}
