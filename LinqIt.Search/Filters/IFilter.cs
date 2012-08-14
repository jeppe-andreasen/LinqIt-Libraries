using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using EPocalipse.IFilter;
using LinqIt.Search.Utilities;

namespace LinqIt.Search.Filters
{
    public class IFilter : Filter
    {
        public override bool Process(CrawlData data)
        {
            try
            {
                using (var file = new TempFile())
                {
                    file.FileName += "." + Extension;
                    File.WriteAllBytes(file.FileName, data.ResponseStream.ToArray());
                    using (var filterReader = new FilterReader(file.FileName))
                    {
                        data.FilteredContent = filterReader.ReadToEnd().Trim();
                    }
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
