using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.XPath;

namespace LinqIt.Utils.Extensions
{
    public static class IEnumerableExtensions
    {
        #region Methods

        public static bool IsFirst<T>(this IEnumerable<T> collection, T item)
        {
            if (!collection.Any())
                return false;
            return collection.First().Equals(item);
        }

        public static string ToSeparatedString(this IEnumerable collection, string separator)
        {
            var builder = new StringBuilder();
            foreach (var item in collection)
            {
                if (item == null || string.IsNullOrEmpty(item.ToString()))
                    continue;
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.Append(item.ToString());
            }
            return builder.ToString();
        }

        public static string ToSeparatedString(this IEnumerable collection, string separator, string format)
        {
            var builder = new StringBuilder();
            foreach (object item in collection)
            {
                if (item == null)
                    continue;
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.AppendFormat(format, item);
            }
            return builder.ToString();
        }

        public static string ToSeparatedString<T>(this IEnumerable<T> collection, string separator, Func<T, string> formatter)
        {
            var builder = new StringBuilder();
            foreach (T item in collection)
            {
                if (item == null)
                    continue;
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.Append(formatter(item));
            }
            return builder.ToString();
        }

        public static bool Intersects<T>(this IEnumerable<T> collectionA, IEnumerable<T> collectionB)
        {
            if (collectionB == null)
                return false;
            if (!collectionA.Any() || !collectionB.Any())
                return false;

            return collectionA.Where(a => collectionB.Contains(a)).Any();
        }

        /// <summary>
        /// Segments the collection into groups of siblings with the same segmentation key
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="collection"></param>
        /// <param name="keyFunc"></param>
        /// <returns></returns>
        public static IEnumerable<Segment<TKey, TValue>> Segment<TKey, TValue>(this IEnumerable<TValue> collection, Func<TValue, TKey> keyFunc)
        {
            var prevKey = default(TKey);
            Segment<TKey, TValue> segment = null;
            var result = new List<Segment<TKey, TValue>>();
            foreach (var value in collection)
            {
                var key = keyFunc(value);
                if (key.Equals(prevKey) && segment != null)
                    segment.Add(value);
                else
                {
                    segment = new Segment<TKey, TValue>(key) { value };
                    result.Add(segment);
                }
                prevKey = key;
            }
            return result;
        }

        public static IEnumerable<TSource> DistinctBy<TSource, TKey>(this IEnumerable<TSource> source, Func<TSource, TKey> keySelector)
        {
            var knownKeys = new HashSet<TKey>();
            return source.Where(element => knownKeys.Add(keySelector(element)));
        }

        #endregion Methods

        public static void Do<T>(this IEnumerable<T> collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }

        public static IEnumerable<T[]> IntoGroupsOf<T>(this IEnumerable<T> collection, int size)
        {
            var result = new List<T[]>();
            var i = 0;
            var count = collection.Count();
            while (i < count)
            {
                result.Add(collection.Skip(i).Take(size).ToArray());
                i += size;
            }
            return result;
        }

        public static void Iterate<T>(this IEnumerable<T> range, Action<T> action)
        {
            foreach (T item in range)
                action(item);
        }
    }

    public class Segment<TKey, TValue> : List<TValue>
    {
        public Segment(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; private set; }
    }
}
