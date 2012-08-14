using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class SearchResult
    {
        internal SearchResult(SearchRecord[] records, int skip, int take, long totalResults)
        {
            Records = records;
            Skip = skip;
            Take = take;
            TotalResults = totalResults;
        }

        public SearchRecord[] Records { get; private set; }

        public int Skip { get; private set; }

        public int Take { get; private set; }

        public long TotalResults { get; private set; }
    }
}
