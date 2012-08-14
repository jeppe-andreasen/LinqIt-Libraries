using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinqIt.Components;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class GridModuleService
    {
        public static int[] GetModuleColumnOptions(string relativePath)
        {
            var type = GridModuleService.ExtractLayoutType(relativePath, "Control");
            if (type == null)
                throw new ApplicationException("Could not extract layout type: " + relativePath);

            var control = Activator.CreateInstance(type) as IGridModuleRendering;
            return control == null ? new int[0] : control.GetModuleColumnOptions();
        }

        internal static GridLayout GetPageGridLayout(string layoutPath)
        {
            var type = ExtractLayoutType(layoutPath, "Master");
            if (type == null)
                return GridLayout.Empty;

            var page = Activator.CreateInstance(type) as IGridModuleControl;
            return page == null ? GridLayout.Empty : page.GetGridLayout();
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
    }
}
