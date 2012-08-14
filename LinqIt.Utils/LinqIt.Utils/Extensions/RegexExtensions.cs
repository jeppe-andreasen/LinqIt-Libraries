using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace LinqIt.Utils.Extensions
{
    public static class RegexExtensions
    {
        public static List<RegexExtraction> Extract(this Regex regex, string input)
        {
            List<RegexExtraction> result = new List<RegexExtraction>();

            var matches = regex.Matches(input);
            if (matches.Count == 0)
                result.Add(new RegexTextExtraction(input));
            else
            {
                int index = 0;
                foreach (Match match in matches)
                {
                    if (match.Index > index)
                        result.Add(new RegexTextExtraction(input.Substring(index, match.Index - index)));
                    result.Add(new RegexMatchExtraction(match));
                    index = match.Index + match.Length;
                }
                if (index < input.Length - 1)
                    result.Add(new RegexTextExtraction(input.Substring(index)));
            }
            return result;
        }

        public static string ReplaceMatches(this Regex regex, string input, Func<Match, string> replacement)
        {
            StringBuilder result = new StringBuilder();
            foreach (var extraction in regex.Extract(input))
            {
                if (extraction.Type == RegexExtractionType.Text)
                    result.Append(((RegexTextExtraction)extraction).Value);
                else
                    result.Append(replacement(((RegexMatchExtraction)extraction).Value));
            }
            return result.ToString();
        }
    }

    public enum RegexExtractionType
    {
        Match,
        Text
    };

    public abstract class RegexExtraction
    {
        public abstract RegexExtractionType Type { get; }
    }

    public class RegexMatchExtraction : RegexExtraction
    {
        public RegexMatchExtraction(Match match)
        {
            Value = match;
        }

        public override RegexExtractionType Type
        {
            get
            {
                return RegexExtractionType.Match;
            }
        }

        public Match Value { get; set; }
    }

    public class RegexTextExtraction : RegexExtraction
    {
        public RegexTextExtraction(string value)
        {
            Value = value;
        }

        public override RegexExtractionType Type
        {
            get
            {
                return RegexExtractionType.Text;
            }
        }

        public string Value { get; set; }
    }
}
