using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Reflection;
using LinqIt.Ajax.Extensions;
using System.IO;
using System.Xml;

namespace LinqIt.Ajax
{
    /// <summary>
    /// This utility is used by the Web Application to generate javascript code from methods.
    /// </summary>
    public static class AjaxUtil
    {

        /// <summary>
        /// This method generates a javascript function from a static method on the control.
        /// To Generate functions for all static methods on the control, use RegisterPageMethods instead
        /// </summary>
        /// <param name="control">The user control which contains the static method</param>
        /// <param name="method">The name of the static method</param>
        /// <param name="functionName">The name of the resulting javascript function</param>
        public static void RegisterPageMethod(Control control, string method)
        {
            Type type = control.GetType().BaseType;
            RegisterPageMethod(control, type, method);
        }

        public static void RegisterPageMethod(Control control, object obj, string method)
        {
            RegisterPageMethod(control, obj.GetType(), method);
        }

        private static void RegisterPageMethod(Control control, Type type, string method)
        {
            MethodInfo info = type.GetMethod(method, BindingFlags.Public | BindingFlags.Static);
            if (info == null)
            {
                throw new ApplicationException("Method could not be found " + method);
            }
            var attribute = info.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).FirstOrDefault();
            if (attribute == null)
                throw new ApplicationException("The method does not have the AjaxMethod attribute");

            RegisterPageMethod(control, type, info, (AjaxMethod)attribute);
        }

        /// <summary>
        /// This method generates a javascript function for each static method on the control, which have the "AjaxMethod" attribute
        /// A good idea is to call this method from a BaseUserControl by overriding the OnPreRender method
        /// </summary>
        /// <param name="control">The usercontrol containing the static methods</param>
        public static void RegisterPageMethods(UserControl control)
        {
            Type type = control.GetType().BaseType;
            foreach (var info in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = info.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).FirstOrDefault();
                if (attribute != null)
                    RegisterPageMethod(control, type, info, (AjaxMethod)attribute);
            }
        }

        /// <summary>
        /// This method generates a javascript function for each static method on the control, which have the "AjaxMethod" attribute
        /// A good idea is to call this method from a BaseUserControl by overriding the OnPreRender method
        /// </summary>
        /// <param name="control">The usercontrol containing the static methods</param>
        public static void RegisterPageMethods(Page control)
        {
            Type type = control.GetType().BaseType;
            foreach (var info in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = info.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).FirstOrDefault();
                if (attribute != null)
                    RegisterPageMethod(control, type, info, (AjaxMethod)attribute);
            }
        }

        /// <summary>
        /// This method generates a javascript function for each static method on the control, which have the "AjaxMethod" attribute
        /// A good idea is to call this method from a BaseUserControl by overriding the OnPreRender method
        /// </summary>
        /// <param name="control">The usercontrol containing the static methods</param>
        public static void RegisterAjaxMethods(Control control)
        {
            Type type = control.GetType();
            foreach (var info in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = info.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).FirstOrDefault();
                if (attribute != null)
                    RegisterPageMethod(control, type, info, (AjaxMethod)attribute);
            }
        }

        private static void RegisterPageMethod(Control control, Type type, MethodInfo info, AjaxMethod attribute)
        {
            if (attribute.Type == AjaxType.Async)
                RegisterAsyncMethod(control, type, info, false);
            else if (attribute.Type == AjaxType.Sync)
                RegisterSyncMethod(control, type, info, false);
            else
            {
                RegisterAsyncMethod(control, type, info, true);
                RegisterSyncMethod(control, type, info, true);
            }
        }

        private static void RegisterAsyncMethod(Control control, Type type, MethodInfo info, bool appendType)
        {
            var function = new StringBuilder();
            var functionName = char.ToLower(info.Name[0]) + info.Name.Substring(1);
            if (appendType)
                functionName += "Async";
            const string callName = "callMethodAsync";
            var parameters = info.GetParameters().Select(p => p.Name).ToList();
            parameters.Insert(0, "onSuccess");

            string parameterString = parameters.Join(",");
            function.AppendFormat("function {0}({1})\r\n", functionName, parameterString);

            if (parameters.Any())
                parameterString = ", " + parameterString;

            function.AppendLine("{");
            function.AppendFormat("{0}('{1}','{2}'{3});\r\n", callName, type.AssemblyQualifiedName, info.Name, parameterString);
            function.AppendLine("}");
            control.Page.ClientScript.RegisterClientScriptBlock(control.Page.GetType(), functionName, function.ToString(), true);
        }

        private static void RegisterSyncMethod(Control control, Type type, MethodInfo info, bool appendType)
        {
            var function = new StringBuilder();
            var functionName = char.ToLower(info.Name[0]) + info.Name.Substring(1);
            if (appendType)
                functionName += "Sync";
            const string callName = "callMethod";
            var parameters = info.GetParameters().Select(p => p.Name).ToList();

            string parameterString = parameters.Join(",");
            function.AppendFormat("function {0}({1})\r\n", functionName, parameterString);

            if (parameters.Any())
                parameterString = ", " + parameterString;

            function.AppendLine("{");
            function.Append("return ");
            function.AppendFormat("{0}('{1}','{2}'{3});\r\n", callName, type.AssemblyQualifiedName, info.Name, parameterString);
            function.AppendLine("}");
            control.Page.ClientScript.RegisterClientScriptBlock(control.Page.GetType(), functionName, function.ToString(), true);
        }



        internal static string ToSeparatedString<T>(this IEnumerable<T> collection, string separator, Func<T, string> formatter)
        {
            var builder = new StringBuilder();
            foreach (T item in collection)
            {
                if (item == null)
                    continue;
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.Append(formatter(item));
            }
            return builder.ToString();
        }

        internal static string ToSeparatedString<T>(this IEnumerable<T> collection, string separator)
        {
            var builder = new StringBuilder();
            foreach (T item in collection)
            {
                if (item == null)
                    continue;
                if (builder.Length > 0)
                    builder.Append(separator);
                builder.Append(item.ToString());
            }
            return builder.ToString();
        }

        public static string RenderHtml(Action<Html32TextWriter> action)
        {
            var builder = new StringBuilder();
            using (var sw = new StringWriter(builder))
            using (var hw = new Html32TextWriter(sw))
            {
                action(hw);
            }
            return builder.ToString();
        }
    
        public static void RegisterPageMethods(Control control, object handler, string scriptKey, string nameSpace)
        {
            if (control.Page.ClientScript.IsClientScriptBlockRegistered(scriptKey))
                return;

            var type = handler.GetType();
            var script = new StringBuilder();

            if (!string.IsNullOrEmpty(nameSpace))
                script.AppendLine("var " + nameSpace + " = {");

            var isFirst = true;

            foreach (var info in type.GetMethods(BindingFlags.Public | BindingFlags.Static))
            {
                var attribute = (AjaxMethod)info.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).FirstOrDefault();
                if (attribute == null) 
                    continue;

                var functionName = char.ToLower(info.Name[0]) + info.Name.Substring(1);
                var callName = "callMethod";
                var parameters = info.GetParameters().Select(p => p.Name).ToList();
                if (attribute.Type == AjaxType.Async)
                {
                    callName += "Async";
                    parameters.Insert(0, "onSuccess");
                }

                var parameterString = parameters.Join(",");

                if (string.IsNullOrEmpty(nameSpace))
                    script.AppendFormat("function {0}({1})\r\n", functionName, parameterString);
                else
                {
                    if (!isFirst)
                        script.Append(",\r\n");
                    else
                        isFirst = false;
                    script.AppendFormat("{0}: function({1})", functionName, parameterString);
                }

                if (parameters.Any())
                    parameterString = ", " + parameterString;

                script.Append("{\r\n");
                if (attribute.Type == AjaxType.Sync)
                    script.Append("return ");
                script.AppendFormat("{0}('{1}','{2}'{3});\r\n", callName, type.AssemblyQualifiedName, info.Name, parameterString);
                script.AppendLine("}\r\n");
                
            }

            if (!string.IsNullOrEmpty(nameSpace))
                script.AppendLine("}");

            control.Page.ClientScript.RegisterClientScriptBlock(control.Page.GetType(), scriptKey, script.ToString(), true);
        }
    }
}
