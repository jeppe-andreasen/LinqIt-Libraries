using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search.Filters
{
    public class PlainTextFilter : Filter
    {
        public override bool Process(CrawlData data)
        {
            data.FilteredContent = data.OriginalContent;
            return true;
        }
    }
}
