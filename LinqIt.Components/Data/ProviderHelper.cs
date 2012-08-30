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

        public static T GetProvider<T>(string typeName, string referenceId = null)
        {
            var type = Type.GetType(typeName);
            if (type == null)
                return default(T);

            var constructor = type.GetConstructor(new[] { typeof(string) });
            if (constructor != null)
                return (T)constructor.Invoke(new[] { referenceId });
            constructor = type.GetConstructor(Type.EmptyTypes);
            return (T)constructor.Invoke(null);
        }

        public static GridItemProvider GetGridItemProvider(string typeName, string referenceId)
        {
            return GetProvider<GridItemProvider>(typeName, referenceId);
        }
    }
}
