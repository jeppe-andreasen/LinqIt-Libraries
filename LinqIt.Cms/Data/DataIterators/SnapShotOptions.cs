using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Data.DataIterators
{
    public class SnapShotOptions
    {
        public DateTime From { get; set; }

        public DateTime To { get; set; }

        public string[] ValidFileExtensions { get; set; }

        public string[] InvalidPaths { get; set; }
    }
}
