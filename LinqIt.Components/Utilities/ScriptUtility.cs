using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;

namespace LinqIt.Components.Utilities
{
    public static class ScriptUtility
    {
        public static void RegisterEmbeddedCss(Type type, Control control, string projectQualifiedResourceName)
        {
            RegisterEmbeddedResource(type, control, projectQualifiedResourceName, "<link href=\"{0}\" type=\"text/css\" rel=\"stylesheet\" />");
        }

        public static void RegisterEmbeddedJs(Type type, Control control, string projectQualifiedResourceName)
        {
            RegisterEmbeddedResource(type, control, projectQualifiedResourceName, "<script type=\"text/javascript\" src=\"{0}\"></script>");
        }

        private static void RegisterEmbeddedResource(Type type, Control control, string projectQualifiedResourceName, string scriptFormat)
        {
            if (control.Page.ClientScript.IsClientScriptIncludeRegistered(projectQualifiedResourceName))
                return;
            var resourceUrl = control.Page.ClientScript.GetWebResourceUrl(type, projectQualifiedResourceName);
            var includeTag = string.Format(scriptFormat, resourceUrl);
            control.Page.ClientScript.RegisterClientScriptBlock(control.GetType(), projectQualifiedResourceName, includeTag, false); 
        }
    }
}
