using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class FilterResult
    {
        public bool Success { get; internal set; }

        public string Message { get; set; }

        public Exception Exception { get; set; }
    }
}
