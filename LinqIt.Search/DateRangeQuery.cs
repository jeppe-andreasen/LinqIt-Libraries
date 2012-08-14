using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class DateRangeQuery : Query
    {
        public DateRangeQuery(string fieldName, DateTime? from, DateTime? to, bool includeLower = true, bool includeUpper = true)
        {
            FieldName = fieldName;
            From = from;
            To = to;
            IncludeLower = includeLower;
            IncludeUpper = includeUpper;
        }

        public string FieldName { get; private set; }

        public DateTime? From { get; private set; }

        public DateTime? To { get; private set; }

        public bool IncludeLower { get; private set; }

        public bool IncludeUpper { get; private set; }
    }
}
