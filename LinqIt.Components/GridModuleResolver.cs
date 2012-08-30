using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using LinqIt.Utils;
using LinqIt.Utils.Caching;

namespace LinqIt.Components
{
    public enum GridModuleRenderingType
    {
        Usercontrol,
        Control,
        Xslt
    };

    public class GridModuleResolver
    {
        private readonly Dictionary<string, GridModuleRenderingDefinition> _definitions;

        private GridModuleResolver(AppDomain domain)
        {
            _definitions = new Dictionary<string, GridModuleRenderingDefinition>();
            var layouts = ScanLayouts();

            TypeUtility.LoadReferencedAssemblies(domain);
            foreach (var type in TypeUtility.GetTypesImplementingInterface<IGridModuleRendering>(domain))
            {
                if (type.IsAbstract || type.ContainsGenericParameters)
                    continue;

                var moduleName = type.Name.Replace("Rendering", "");
                var rendering = TypeUtility.Activate<IGridModuleRendering>(type);
                GridModuleRenderingDefinition definition;
                if (rendering is UserControl)
                    definition = new GridModuleRenderingDefinition(GridModuleRenderingType.Usercontrol, moduleName, layouts.ContainsKey(moduleName) ? layouts[moduleName] : "", type);
                else if (rendering is Control)
                    definition = new GridModuleRenderingDefinition(GridModuleRenderingType.Control, moduleName, null, type);
                else
                    continue;
                _definitions.Add(moduleName, definition);
            }
        }

        public static GridModuleResolver Instance
        {
            get
            {
                return Cache.Get("GridModuleResolver", CacheScope.Application, () => new GridModuleResolver(AppDomain.CurrentDomain));
            }
        }

        private static Dictionary<string, string> ScanLayouts()
        {
            var result = new Dictionary<string, string>();
            var moduleDirectory = HttpContext.Current.Server.MapPath("~/modules");
            foreach (string filename in Directory.GetFiles(moduleDirectory))
            {
                var ext = (Path.GetExtension(filename.ToLower()) ?? string.Empty).Trim('.');
                switch (ext)
                {
                    case "ascx":
                        {
                            var relativePath = "~/modules/" + Path.GetFileName(filename);
                            var type = ExtractLayoutType(relativePath, "Control");
                            if (type != null)
                            {
                                var moduleName = type.Name.Replace("Rendering", "");
                                if (!result.ContainsKey(moduleName))
                                    result.Add(moduleName, relativePath);
                            }
                        }
                        break;
                    case "xslt":
                        break;
                }
            }
            return result;
        }

        public static Type ExtractLayoutType(string relativePath, string directive)
        {
            var path = HttpContext.Current.Server.MapPath(relativePath);
            if (!File.Exists(path))
                return null;

            using (var reader = new StreamReader(path))
            {
                var text = reader.ReadToEnd();
                var matchA = Regex.Match(text, @"<%@ " + directive + ".*?%>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
                if (!matchA.Success)
                    return null;

                var matchB = Regex.Match(matchA.Value, " Inherits=\"([^\"]+)\"");
                if (!matchB.Success)
                    return null;

                string assemblyQualifiedName = matchB.Groups[1].Value;
                if (assemblyQualifiedName.Split(',').Length < 2)
                    throw new ApplicationException("'Inherits' property must be assembly qualified on the module " + relativePath);

                return Type.GetType(matchB.Groups[1].Value);
            }
        }

        public GridModuleRenderingDefinition GetRenderingDefinition(string moduleName)
        {
            return _definitions.ContainsKey(moduleName) ? _definitions[moduleName] : null;
        }
    
        public int[] GetModuleColumnOptions(string moduleName)
        {
            if (_definitions.ContainsKey(moduleName))
            {
                var definition = _definitions[moduleName];
                if (definition.Type != null)
                {
                    var rendering = TypeUtility.Activate<IGridModuleRendering>(definition.Type);
                    return rendering.GetModuleColumnOptions();
                }
            }
            return new int[0];
        }
    }

    public class GridModuleRenderingDefinition
    {
        internal GridModuleRenderingDefinition(GridModuleRenderingType renderingType, string moduleName, string path, Type type)
        {
            RenderingType = renderingType;
            ModuleName = moduleName;
            Path = path;
            Type = type;
        }

        public string ModuleName { get; private set; }

        public GridModuleRenderingType RenderingType { get; private set; }

        public string Path { get; private set; }

        public Type Type { get; private set; }
    }
}
