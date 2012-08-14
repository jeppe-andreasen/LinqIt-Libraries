using System;
using LinqIt.Search.Configuration;

namespace LinqIt.Search.Filters
{
    public abstract class Filter
    {
        internal protected virtual void Initialize(IFilterConfiguration configuration)
        {
            MimeType = configuration.MimeType;
            Extension = configuration.Extension;
        }

        public string MimeType { get; set; }

        public string Extension { get; set; }

        public virtual bool CanProcess(CrawlData data)
        {
            return string.Compare(data.MimeType, MimeType, StringComparison.InvariantCultureIgnoreCase) == 0;
        }

        public abstract bool Process(CrawlData data);
    }
}
