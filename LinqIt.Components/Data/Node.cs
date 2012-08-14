using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public class Node
    {
        public string Icon { get; set; }

        public string Text { get; set; }

        public string Id { get; set; }

        public bool Selectable { get; set; }

        public string HelpText { get; set; }

        //public string LocalPath { get; set; }

        public bool Draggable { get; set; }
    }
}
