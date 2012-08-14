using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Configuration
{
    public class Environment
    {
        public Environment(string name, bool isCurrent, bool isVisible)
        {
            Name = name;
            IsVisible = isVisible;
            IsCurrent = isCurrent;
        }

        public bool IsVisible { get; private set; }

        public bool IsCurrent { get; private set; }

        public string Name { get; private set; }
    }
}
