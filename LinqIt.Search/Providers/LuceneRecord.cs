using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using Lucene.Net.Documents;

namespace LinqIt.Search.Providers
{
    public class LuceneRecord : SearchRecord
    {
        private readonly Document _document;

        internal LuceneRecord(Document document) : base(document.Get("__id"))
        {
            _document = document;
        }

        internal LuceneRecord(string id) : base(id)
        {
            _document = new Document();
            _document.Add(new Field("__id", id, Field.Store.YES, Field.Index.NOT_ANALYZED));
        }


        internal Document Document { get { return _document; } }

        public override int GetInt(string key)
        {
            return Convert.ToInt32(_document.Get(key));
        }

        public override string GetString(string key)
        {
            return _document.Get(key);
        }

        public override DateTime? GetDate(string key)
        {
            var value = _document.Get(key);
            if (string.IsNullOrEmpty(value))
                return null;
            return DateTime.ParseExact(value, "yyyyMMddHHmmss", CultureInfo.InvariantCulture);
        }

        public override void SetDate(string key, DateTime? value)
        {
            var s = value.HasValue ? value.Value.ToString("yyyyMMddHHmmss") : string.Empty;
            _document.Add(new Field(key, s, Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        public override void SetInt(string key, int value)
        {
            _document.Add(new Field(key, value.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
        }

        public override void SetString(string key, string value)
        {
            _document.Add(new Field(key, value.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));
        }
    }
}
