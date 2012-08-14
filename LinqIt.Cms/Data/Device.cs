using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Data
{
    public class Device : Entity
    {
        public string Name { get { return EntityName; } }
    }
}
