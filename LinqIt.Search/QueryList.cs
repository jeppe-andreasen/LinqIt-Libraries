using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class QueryList
    {
        public QueryList()
        {
            SubQueries = new List<Query>();
        }

        public IList<Query> SubQueries { get; private set; }

        internal Query AsQuery
        {
            get 
            {
                return SubQueries.Count == 1 ? SubQueries[0] : BooleanQuery.And(SubQueries.ToArray());
            }
        }
    }
}
