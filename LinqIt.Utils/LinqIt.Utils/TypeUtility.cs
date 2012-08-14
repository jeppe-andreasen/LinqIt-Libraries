using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;

namespace LinqIt.Utils
{
    public class TypeUtility
    {
        public static IEnumerable<Type> GetTypesImplementingInterface<T>()
        {
            var interfaceType = typeof(T);

            var result = new List<Type>();
            AppDomain domain = AppDomain.CreateDomain("versus_core");
            var binDirectory = HttpContext.Current.Server.MapPath("~/bin");
            string[] files = Directory.GetFiles(binDirectory, "*.dll");
            foreach (var file in files)
            {
                try
                {
                    Assembly assembly = domain.Load(Path.GetFileName(file));
                    result.AddRange(assembly.GetTypes().Where(t => HasInterface(t, interfaceType)));    
                }
                catch
                {
                    
                }
            }
            return result;
        }

        private static bool HasInterface(Type type, Type interfaceType)
        {
            return type.GetInterface(interfaceType.Name) == interfaceType;
        }
    }
}
