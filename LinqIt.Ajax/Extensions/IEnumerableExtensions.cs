using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax.Extensions
{
    internal static class IEnumerableExtensions
    {
        // Methods
        internal static void Iterate<T>(this IEnumerable<T> range, Action<T> action)
        {
            foreach (T item in range)
            {
                action(item);
            }
        }

        internal static string Join<T>(this IEnumerable<T> s, string separator)
        {
            return s.Join<T>("{0}", separator);
        }

        internal static string Join<T>(this IEnumerable<T> s, string separator, Func<T, string> func)
        {
            StringBuilder result = new StringBuilder();
            if (s.Any<T>())
            {
                foreach (T t in s)
                {
                    if (result.Length > 0)
                    {
                        result.Append(separator);
                    }
                    result.Append(func(t));
                }
            }
            return result.ToString();
        }

        internal static string Join<T>(this IEnumerable<T> s, string format, string separator)
        {
            StringBuilder result = new StringBuilder();
            if (s.Any<T>())
            {
                foreach (T t in s)
                {
                    if (result.Length > 0)
                    {
                        result.Append(separator);
                    }
                    result.Append(string.Format(format, t));
                }
            }
            return result.ToString();
        }
    }
}
