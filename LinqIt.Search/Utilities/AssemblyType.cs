using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;

namespace LinqIt.Search.Utilities
{
    public class AssemblyType
    {
        public AssemblyType(string value)
        {
            string[] parts = value.Split(',').Select(t => t.Trim()).ToArray();
            TypeName = parts[0];
            AssemblyName = parts[1];
        }

        public string AssemblyName { get; private set; }

        public string TypeName { get; private set; }

        public T Instantiate<T>()
        {
            Assembly assembly = Assembly.Load(AssemblyName);
            return Instantiate<T>(assembly);
        }

        public T Instantiate<T>(Assembly assembly)
        {
            Type type = assembly.GetType(TypeName);
            ConstructorInfo info = type.GetConstructor(Type.EmptyTypes);
            T result = (T)info.Invoke(null);
            return result;
        }
    }
}
