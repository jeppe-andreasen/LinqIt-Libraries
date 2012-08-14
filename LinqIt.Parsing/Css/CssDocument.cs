using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqIt.Parsing.Css
{
    public class CssDocument
    {
        private readonly List<CssRule> _rules;

        public CssDocument(string css)
        {
            _rules = new List<CssRule>();
            var token = new Token(PrepareCss(css));
            token.MoveToContent();
            while (!token.IsDone)
            {
                _rules.Add(new CssRule(token));
                token.MoveToContent();
            }
        }

        public IEnumerable<CssRule> Rules
        {
            get { return _rules; }
        }

        public static string PrepareCss(string css)
        {
            var result = new StringBuilder();
            css = Regex.Replace(css, @"\s+", " ", RegexOptions.Singleline);
            var token = new Token(css);
            while (!token.IsDone)
            {
                if (token.Peeks('\'', '"'))
                    result.Append(token.ReadString(true));
                else if (token.Peeks("/*"))
                    token.MovePast("*/");
                else
                {
                    result.Append(token.Current);
                    token.Next();
                }
            }
            return result.ToString();
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            foreach (var rule in _rules)
            {
                if (result.Length > 0)
                    result.AppendLine("");
                result.Append(rule.ToString());
            }
            return result.ToString();
        }

        public string GetDebuggingText()
        {
            var result = new StringBuilder();
            foreach (var rule in _rules)
            {
                if (result.Length > 0)
                    result.AppendLine("");
                result.Append(rule.GetDebuggingText());
            }
            return result.ToString();
        }
    }
}
