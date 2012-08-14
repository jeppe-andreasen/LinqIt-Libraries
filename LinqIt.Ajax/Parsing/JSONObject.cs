using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Ajax.Extensions;

namespace LinqIt.Ajax.Parsing
{
    public class JSONObject : JSONValue
    {
        // Fields
        private readonly Dictionary<string, JSONValue> _values = new Dictionary<string, JSONValue>();

        // Methods
        private void AddArray(string key, Action<JSONArray> fillAction)
        {
            var array = new JSONArray();
            fillAction(array);
            _values.Add(key, array);
        }

        public void AddValue(string key, bool value)
        {
            _values.Add(key, new JSONBoolean(value));
        }

        public void AddValue(string key, DateTime value)
        {
            _values.Add(key, new JSONDateTime(value));
        }

        public void AddValue(string key, IEnumerable<bool> value)
        {
            AddArray(key, a => a.AddRange(value));
        }

        public void AddValue(string key, IEnumerable<DateTime> value)
        {
            AddArray(key, a => a.AddRange(value));
        }

        public void AddValue(string key, IEnumerable<decimal> value)
        {
            AddArray(key, a => a.AddRange(value));
        }

        public void AddValue(string key, IEnumerable<int> value)
        {
            AddArray(key, a => a.AddRange(value));
        }

        public void AddValue(string key, IEnumerable<string> value)
        {
            AddArray(key, a => a.AddRange(value));
        }

        public void AddValue(string key, decimal value)
        {
            _values.Add(key, new JSONNumber(value));
        }

        public void AddValue(string key, int value)
        {
            _values.Add(key, new JSONNumber(value));
        }

        public void AddValue(string key, string value)
        {
            _values.Add(key, new JSONString(value));
        }

        public void AddValue(string key, JSONValue value)
        {
            _values.Add(key, value);
        }

        public static new JSONObject Parse(string value)
        {
            return Parse(new Token(value));
        }

        public static new JSONObject Parse(Token token)
        {
            token.MoveToContent();
            if (!token.Peeks("{"))
            {
                return null;
            }
            var result = new JSONObject();
            token.Skip("{");
            while (!token.Peeks("}"))
            {
                token.MoveToContent();
                token.Skip("\"");
                var key = token.ReadUntil(new[] { "\"" });
                token.Skip("\"");
                token.MovePast(":");
                token.MoveToContent();
                var value = JSONValue.Parse(token);
                result.AddValue(key, value);
                token.MoveToContent();
                if (token.Peeks(","))
                {
                    token.MovePast(",");
                }
                token.MoveToContent();
            }
            token.Skip("}");
            return result;
        }

        public override string ToString()
        {
            return ("{" + _values.Keys.Join(", ", k => string.Format("\"{0}\": {1}", k, _values[k])) + "}");
        }

        public override object Value
        {
            get { return _values; }
        }

        public JSONValue this[string key]
        {
            get 
            {
                return !_values.ContainsKey(key) ? null : _values[key];
            }
            set { _values[key] = value; }
        }

        public string[] Keys
        {
            get { return _values.Keys.ToArray(); }
        }


        public bool HasKey(string key)
        {
            return _values.ContainsKey(key);
        }
    }


}
