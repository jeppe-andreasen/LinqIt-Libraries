using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LinqIt.Ajax.Extensions;

namespace LinqIt.Ajax.Parsing
{
    public class JSONArray : JSONValue
    {
        // Fields
        private readonly List<JSONValue> _values = new List<JSONValue>();

        // Methods
        public void AddRange(IEnumerable<bool> values)
        {
            if (values != null)
            {
                _values.AddRange(values.Select<bool, JSONValue>(v => new JSONBoolean(v)));
            }
        }

        public void AddRange(IEnumerable<DateTime> values)
        {
            if (values != null)
            {
                _values.AddRange(values.Select<DateTime, JSONValue>(v => new JSONDateTime(v)));
            }
        }

        public void AddRange(IEnumerable<decimal> values)
        {
            if (values != null)
            {
                _values.AddRange(values.Select<decimal, JSONValue>(v => new JSONNumber(v)));
            }
        }

        public void AddRange(IEnumerable<int> values)
        {
            if (values != null)
            {
                _values.AddRange(values.Select<int, JSONValue>(v => new JSONNumber(v)));
            }
        }

        public void AddRange(IEnumerable<string> values)
        {
            if (values != null)
            {
                _values.AddRange(values.Select<string, JSONValue>(v => new JSONString(v)));
            }
        }

        public void AddRange(IEnumerable<JSONObject> values)
        {
            if (values != null)
            {
                _values.AddRange(values);
            }
        }

        public void AddValue(bool value)
        {
            _values.Add(new JSONBoolean(value));
        }

        public void AddValue(DateTime value)
        {
            _values.Add(new JSONDateTime(value));
        }

        public void AddValue(decimal value)
        {
            _values.Add(new JSONNumber(value));
        }

        public void AddValue(int value)
        {
            _values.Add(new JSONNumber(value));
        }

        public void AddValue(string value)
        {
            _values.Add(new JSONString(value));
        }

        public void AddValue(JSONValue value)
        {
            _values.Add(value);
        }

        public static new JSONArray Parse(string value)
        {
            return Parse(new Token(value));
        }

        public static new JSONArray Parse(Token token)
        {
            if (!token.Peeks("["))
            {
                return null;
            }
            var array = new JSONArray();
            token.Skip("[");
            while (!token.Peeks("]"))
            {
                var value = JSONValue.Parse(token);
                array.AddValue(value);
                token.MoveToContent();
                if (token.Peeks(","))
                {
                    token.MovePast(",");
                }
                token.MoveToContent();
            }
            token.Skip("]");
            return array;
        }

        public override string ToString()
        {
            return ("[" + _values.Join(", ") + "]");
        }

        public T[] ToArray<T>()
        {
            return _values.Select(v => (T)v.Value).ToArray();
        }

        public override object Value
        {
            get { return _values.Select(v => v.Value).ToArray(); }
        }

        public IEnumerable<JSONValue> Values
        {
            get { return _values; }
        }

        public int Count
        {
            get { return _values.Count; }
        }

        //public IEnumerator<JSONValue> GetEnumerator()
        //{
        //    return _values.GetEnumerator();
        //}

        //System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        //{
        //    return ((IEnumerable)_values).GetEnumerator();
        //}

        public static JSONArray FromRange(IEnumerable<string> range)
        {
            var result = new JSONArray();
            result.AddRange(range);
            return result;
        }
    }
} 
