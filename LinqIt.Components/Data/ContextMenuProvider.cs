using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public abstract class ContextMenuProvider
    {
        public abstract ContextMenu GetContextMenu(string itemId);
    }
}
