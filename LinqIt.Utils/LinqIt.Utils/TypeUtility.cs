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
        public static void LoadReferencedAssemblies(AppDomain currentDomain)
        {
            var loadedAssemblies = currentDomain.GetAssemblies().ToList();
            var loadedPaths = loadedAssemblies.Select(GetLocation).Where(l => !string.IsNullOrEmpty(l)).ToArray();

            var referencedPaths = Directory.GetFiles(currentDomain.BaseDirectory, "*.dll");
            var toLoad = referencedPaths.Where(r => !loadedPaths.Contains(r, StringComparer.InvariantCultureIgnoreCase)).ToList();
            toLoad.ForEach(path => loadedAssemblies.Add(currentDomain.Load(AssemblyName.GetAssemblyName(path)))); 
        }

        private static string GetLocation(Assembly assembly)
        {
            try
            {
                return assembly.Location;
            }
            catch (NotSupportedException)
            {
                return null;
            }
        }

        public static IEnumerable<Type> GetTypesImplementingInterface<T>(AppDomain domain)
        {
            var interfaceType = typeof(T);

            var result = new List<Type>();
            foreach (var assembly in domain.GetAssemblies())
            {
                try
                {
                    result.AddRange(assembly.GetTypes().Where(t => HasInterface(t, interfaceType)));
                }
                catch
                {

                }
            }
            return result;
        }

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

        public static T Activate<T>(Type type)
        {
            bool isGeneric = type.IsGenericType;
            try
            {
                if (!isGeneric)
                    return (T) Activator.CreateInstance(type);
                return (T) Activator.CreateInstance(type.MakeGenericType(type.GetGenericArguments()));
            }
            catch (ArgumentException)
            {
                throw new ArgumentException("Cannot activate type " + type.AssemblyQualifiedName + ". Is generic: " + (isGeneric? "true" : "false"));
            }
        }
    }
}
