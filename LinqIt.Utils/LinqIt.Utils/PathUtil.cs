using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils
{
    public class PathUtil
    {
        public static string Combine(params string[] paths)
        {
            if (paths == null || !paths.Any())
                return string.Empty;
            if (paths.Where(p => p == null).Any())
                throw new ArgumentException("Path part cannot be null");

            bool startsWithSlash = paths.First().StartsWith("/");
            bool endsWithSlash = paths.Last().EndsWith("/");

            string result = paths.Select(s => s.TrimStart('/').TrimEnd('/')).ToSeparatedString("/");
            if (startsWithSlash)
                result = "/" + result;
            if (endsWithSlash)
                result = result + "/";

            return result;
        }

        public static string Combine(bool convertToLegalPath, params string[] paths)
        {
            string result = Combine(paths);
            if (convertToLegalPath)
                result = ConvertToLegalPath(result);
            return result;
        }

        public static string Take(string path, int count)
        {
            if (string.IsNullOrEmpty(path))
                return null;
            bool addSlash = path.StartsWith("/");
            if (addSlash)
                path = path.TrimStart('/');
            var parts = path.Split('/');
            var result = parts.Take(count).ToSeparatedString("/");
            if (addSlash)
                result = "/" + result;
            return result;
        }

        public static string ConvertToLegalPath(string path)
        {
            string legalPath = path.Replace(".", "").
                Replace("-", " ").
                Replace("æ", "ae").
                Replace("ø", "oe").
                Replace("å", "aa").
                Replace("Æ", "Ae").
                Replace("Ø", "Oe").
                Replace("Å", "Aa").Trim();

            // Remove spaces before or after /
            legalPath = Regex.Replace(legalPath, @"\s*/\s*", "/");

            // Remove multispaces
            legalPath = Regex.Replace(legalPath, @"\s{2,}", " ");

            legalPath = Regex.Replace(legalPath, "[^a-zA-Z_0-9 /]", "");

            return legalPath.Trim();
        }

    }

}
