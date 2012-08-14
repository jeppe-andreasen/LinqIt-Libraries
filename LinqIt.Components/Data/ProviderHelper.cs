using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

namespace LinqIt.Components.Data
{
    public class ProviderHelper
    {
        public static TreeNodeProvider GetTreeNodeProvider(string typeName, string referenceId)
        {
            return GetProvider<TreeNodeProvider>(typeName, referenceId);
        }

        public static T GetProvider<T>(string typeName, string referenceId)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                return default(T);

            var constructor = type.GetConstructor(new[] { typeof(string) });
            return (T)constructor.Invoke(new[] { referenceId });
        }

        public static GridItemProvider GetGridItemProvider(string typeName, string referenceId)
        {
            return GetProvider<GridItemProvider>(typeName, referenceId);
        }
    }
}
