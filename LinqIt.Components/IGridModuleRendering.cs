using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components
{
    public interface IGridModuleRendering
    {
        int[] GetModuleColumnOptions();
        void InitializeModule(string id, int? columnSpan);
    }
}
