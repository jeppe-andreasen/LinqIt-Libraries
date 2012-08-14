using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public abstract class SearchRecord
    {
        protected SearchRecord(string id)
        {
            Id = id;
        }

        public string Id { get; private set; }

        public abstract string GetString(string key);
        public abstract void SetString(string key, string value);

        public abstract int GetInt(string key);
        public abstract void SetInt(string key, int value);

        public abstract DateTime? GetDate(string key);
        public abstract void SetDate(string key, DateTime? value);
    }
}
