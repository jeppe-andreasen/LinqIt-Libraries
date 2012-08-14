using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search.Filters
{
    public class EmptyFilter : Filter
    {
        public override bool Process(CrawlData data)
        {
            data.FilteredContent = string.Empty;
            return true;
        }
    }
}
