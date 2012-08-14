using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public class GridItem : Node
    {
        public int ColumnSpan { get; set; }

        internal System.Drawing.Point Position { get; set; }

        internal System.Drawing.Point MatrixPosition { get; set; }

        internal int Width { get; set; }

        internal Ajax.Parsing.JSONValue Index { get; set; }

        public bool IsLocal { get; set; }
    
    }
}
