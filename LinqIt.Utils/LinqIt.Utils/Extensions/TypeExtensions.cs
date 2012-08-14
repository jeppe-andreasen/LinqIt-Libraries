using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Extensions
{
    public static class TypeExtensions
    {
        public static string GetShortAssemblyName(this Type type)
        {
            return type.AssemblyQualifiedName.Split(',').Take(2).ToSeparatedString(", ");
        }
    }
}
