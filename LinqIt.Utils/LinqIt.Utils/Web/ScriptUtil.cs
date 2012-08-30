using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Caching;
using LinqIt.Utils.Extensions;

namespace LinqIt.Utils.Web
{
    public class ScriptUtil
    {
        private readonly List<string> _initializationLines = new List<string>();

        #region Constructors

        private ScriptUtil()
        {
            
        }

        #endregion Constructors

        public static ScriptUtil Instance
        {
            get
            {
                return Cache.Get(typeof(ScriptUtil).FullName, CacheScope.Request, () => new ScriptUtil());
            }
        }

        /// <summary>
        /// Return the full init script with all script tags
        /// </summary>
        /// <returns></returns>
        public String GetInitializationScript()
        {
            if (!_initializationLines.Any())
                return string.Empty;

            var builder = new StringBuilder();
            builder.AppendLine("<script type=\"text/javascript\">");
            foreach (var script in _initializationLines)
            {
                builder.Append(script);
                if (!script.EndsWith(";"))
                    builder.AppendLine(";");
                else
                    builder.AppendLine();
            }
            builder.AppendLine();
            builder.AppendLine("</script>");
            return builder.ToString();
        }

        public static void RegisterInitScript(string componentName, params string[] values)
        {
            Instance.AddScript(componentName, values);
        }

        private void AddScript(string componentName, params string[] values)
        {
            var parameters = values != null ? values.ToSeparatedString(",") : string.Empty;
            var script = "application." + componentName + ".init(" + parameters + ");";
            if (!_initializationLines.Contains(script))
                _initializationLines.Add(script);
        }
    }
}

