using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Collections
{
    public class CaseInvariantNameValueCollection
    {
        private readonly List<KeyValuePair<string, string>> _pairs;
        private bool _reportChanges;
        public event EventHandler Changed;

        public CaseInvariantNameValueCollection()
        {
            _pairs = new List<KeyValuePair<string, string>>();
            _reportChanges = true;
        }

        public T CloneAs<T>() where T:CaseInvariantNameValueCollection, new ()
        {
            var result = new T();
            foreach (var pair in _pairs)
                result._pairs.Add(new KeyValuePair<string, string>(pair.Key, pair.Value));
            return result;
        }

        public IEnumerable<string> Keys
        {
            get
            {
                return _pairs.Select(p => p.Key);
            }
        }

        private KeyValuePair<string, string>? GetPair(string key)
        {
            foreach (var pair in _pairs)
            {
                if (string.Compare(pair.Key, key, true) == 0)
                    return pair;
            }
            return null;
        }

        public bool HasKey(string key)
        {
            return GetPair(key).HasValue;
        }

        public string this[string key]
        {
            get 
            { 
                var pair = GetPair(key);
                return pair.HasValue ? pair.Value.Value : null;
            }
            set
            {
                var pair = GetPair(key);
                if (pair.HasValue)
                    _pairs[_pairs.IndexOf(pair.Value)] = new KeyValuePair<string, string>(key, value);
                else
                    _pairs.Add(new KeyValuePair<string, string>(key, value));
                DoReportChanges();
            }
        }

        public int Count
        {
            get
            {
                return _pairs.Count;
            }
        }

        private void DoReportChanges()
        {
            if (Changed != null && _reportChanges)
                Changed(this, null);
        }

        public void RemoveAttribute(string key)
        {
            this[key] = null;
            DoReportChanges();
        }

        public void Clear()
        {
            _pairs.Clear();
            DoReportChanges();
        }

        public void BeginUpdate()
        {
            _reportChanges = false;
        }

        public void EndUpdate()
        {
            _reportChanges = true;
        }

        public void EndUpdate(bool reportChanges)
        {
            _reportChanges = true;
            if (reportChanges)
                DoReportChanges();
        }
    
        public void Append(string key, string value)
        {
            var pair = GetPair(key);
            if (pair.HasValue)
                _pairs.Remove(pair.Value);
            _pairs.Add(new KeyValuePair<string, string>(key, value));
        }

        public Dictionary<string, string> ToDictionary()
        {
            return _pairs.ToDictionary(pair => pair.Key.ToLower(), pair => pair.Value);
        }
    }
}
