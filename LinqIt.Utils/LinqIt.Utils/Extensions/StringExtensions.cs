using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace LinqIt.Utils.Extensions
{
    public static class StringExtensions
    {
        #region Methods

        public static bool IsLike(this string a, string b)
        {
            return string.Compare(a, b, true) == 0;
        }

        public static bool IsVisible(this string s)
        {
            return s.Trim() != string.Empty;
        }

        public static bool IsWildcardMatch(this string text, string wildcard)
        {
            var regex = new Regex("^" + Regex.Escape(wildcard).Replace(@"\*", ".*") + "$");
            return regex.IsMatch(text);
        }

        public static List<string> Lines(this string value, bool removeEmptyLines, bool trimLines)
        {
            var reader = new StringReader(value);
            var result = new List<string>();
            var line = reader.ReadLine();
            while (line != null)
            {
                if (trimLines)
                    line = line.Trim();
                if (removeEmptyLines)
                {
                    if (!string.IsNullOrEmpty(line))
                        result.Add(line);
                }
                else
                    result.Add(line);
                line = reader.ReadLine();
            }
            reader.Close();
            return result;
        }

        public static string Max(this string value, int maxLength)
        {
            if (value == null)
            {
                return null;
            }
            return value.Length > maxLength ? value.Substring(0, maxLength) : value;
        }

        public static ILookup<string, string> ToLookup(this string text, char itemSeparator, char keyValueSeparator)
        {
            return text.
                Split(itemSeparator).
                Select(item => item.Split(keyValueSeparator)).
                Where(a => a.Length == 2).
                ToLookup(
                    pair => pair[0],
                    pair => pair[1]);
        }

        public static string Take(this string str, int count, char separator)
        {
            var parts = str.Split(separator);
            return parts.Take(count).ToSeparatedString(separator.ToString());
        }

        public static string[] SplitTo(this string str, int count, string fillEmpty, params char[] separator)
        {
            var result = new string[count];
            var parts = str.Split(separator);
            for (int i = 0; i < result.Length; i++)
            {
                if (i < parts.Length)
                    result[i] = parts[i];
                else
                    result[i] = fillEmpty;
            }
            return result;
        }

        #endregion Methods
    }
}