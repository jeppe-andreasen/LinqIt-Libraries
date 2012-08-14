using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Extensions;

namespace LinqIt.Parsing.Css
{
    public class CssSpecificity : IComparable
    {
        private readonly int[] _data;

        public CssSpecificity()
        {
             _data = new [] {0, 0, 0, 0};
        }

        public CssSpecificity(int styleAttributes, int idAttributes, int classAttributes, int typeNames)
        {
            _data = new []{ styleAttributes, idAttributes, classAttributes, typeNames};
        }

        public static CssSpecificity operator +(CssSpecificity x, CssSpecificity y) 
        {
            return new CssSpecificity(x._data[0] + y._data[0], x._data[1] + y._data[1], x._data[2] + y._data[2],x._data[3] + y._data[3]);
        }

        public static bool operator >(CssSpecificity x, CssSpecificity y)
        {
            for (int i = 0; i < 4; i++)
            {
                if (x._data[i] > y._data[i])
                    return true;
                if (x._data[i] < y._data[i])
                    return false;
            }
            return false;
        }

        public static bool operator <(CssSpecificity x, CssSpecificity y)
        {
            for (int i = 0; i < 4; i++)
            {
                if (x._data[i] < y._data[i])
                    return true;
                if (x._data[i] > y._data[i])
                    return false;
            }
            return false;
        }

        public static bool operator ==(CssSpecificity x, CssSpecificity y)
        {
            return !Enumerable.Range(0, 4).Where(i => x._data[i] != y._data[i]).Any();
        }

        public static bool operator !=(CssSpecificity x, CssSpecificity y)
        {
            return Enumerable.Range(0, 4).Where(i => x._data[i] != y._data[i]).Any();
        }

        public override string ToString()
        {
            return _data.ToSeparatedString(",");
        }

        internal void Add(CssSpecificity specificity)
        {
            _data[0] += specificity._data[0];
            _data[1] += specificity._data[1];
            _data[2] += specificity._data[2];
            _data[3] += specificity._data[3];
        }

        internal void Subtract(CssSpecificity specificity)
        {
            _data[0] -= specificity._data[0];
            _data[1] -= specificity._data[1];
            _data[2] -= specificity._data[2];
            _data[3] -= specificity._data[3];
        }

        public int CompareTo(object obj)
        {
            var s = (CssSpecificity) obj;
            if (this < s)
                return -1;
            return this > s ? 1 : 0;
        }

        public bool Equals(CssSpecificity other)
        {
            if (ReferenceEquals(null, other)) return false;
            if (ReferenceEquals(this, other)) return true;
            return Equals(other._data, _data);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != typeof (CssSpecificity)) return false;
            return Equals((CssSpecificity) obj);
        }

        public override int GetHashCode()
        {
            return this.ToString().GetHashCode();
        }
    }

    public static class SpecificityExtensions
    {
        public static CssSpecificity Sum(this IEnumerable<CssSpecificity> collection)
        {
            var result = new CssSpecificity();
            foreach (var specificity in collection)
                result.Add(specificity);
            return result;
        }
    }

}
