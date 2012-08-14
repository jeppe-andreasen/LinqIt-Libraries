using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Utilities
{
    public class AssemblyQualifiedName
    {
        #region Constructors

        public AssemblyQualifiedName(string value)
        {
            string[] parts = value.Split(',');
            TypeName = parts[0].Trim();
            AssemblyName = parts[1].Trim();
        }

        public AssemblyQualifiedName(string assemblyName, string typeName)
        {
            AssemblyName = assemblyName;
            TypeName = typeName;
        }

        #endregion Constructors

        #region Properties

        public string AssemblyName
        {
            get; set;
        }

        public string TypeName
        {
            get; set;
        }

        #endregion Properties

        #region Methods

        public T ActivateObject<T>()
        {
            try
            {
                Type type = Type.GetType(this.ToString());
                object result = Activator.CreateInstance(type);
                return (T)result;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}", TypeName, AssemblyName);
        }

        #endregion Methods
    }
}