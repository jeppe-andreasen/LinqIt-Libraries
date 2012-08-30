using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.IO;
using LinqIt.Ajax.Parsing;
using System.Reflection;
using System.Net;
using System.ComponentModel;
using System.Collections;

namespace LinqIt.Ajax
{
    public class JSONHandler : IHttpHandler, IRequiresSessionState
{
    // Methods
    public void ProcessRequest(HttpContext context)
    {
        #region Initialize Headers
        context.Response.ClearHeaders();
        context.Response.ClearContent();
        context.Response.AddHeader("Cache-Control", "private, max-age=0");
        context.Response.AddHeader("Content-Type", "application/json; charset=utf-8");
        #endregion

        string input = ReadRequest(context);

        JSONObject info = (JSONObject)JSONValue.Parse(input);
        string control = (string)info["control"].Value;
        string method = (string)info["method"].Value;
        JSONObject args = (JSONObject)info["args"];

        try
        {
            object result = InvokeMethod(control, method, args);
            JSONValue response = JSONValue.Serialize(result);
            WriteResponse(context, response.ToString());
        }
        catch (Exception exc)
        {
            var realExc = exc.InnerException ?? exc;
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
            context.Response.AddHeader("jsonerror", "true");
            var errorMessage = new JSONObject();
            errorMessage.AddValue("Message", realExc.Message);
            errorMessage.AddValue("StackTrace", realExc.StackTrace.ToString());
            errorMessage.AddValue("ExceptionType", realExc.GetType().FullName);
            WriteResponse(context, errorMessage.ToString());
        }
    }

    private static object InvokeMethod(string control, string method, JSONObject args)
    {
        MethodInfo methodInfo = Type.GetType(control).GetMethod(method, BindingFlags.Public | BindingFlags.Static);
        if (methodInfo.GetCustomAttributes(true).Where(a => a.GetType() == typeof(AjaxMethod)).Any())
        {
            var parameters = methodInfo.GetParameters();
            if (parameters.Length != args.Keys.Count())
                throw new ArgumentException("Invalid number of arguments supplied. Expected " + parameters.Length + ", recieved " + args.Keys.Count());

            object[] typedArgs = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
                typedArgs[i] = ConvertArgument(parameters[i], args["arg" + i]);
            return methodInfo.Invoke(null, typedArgs);
        }
        else
            throw new ArgumentException("Method not found or inacessible: " + method);
    }

    private static object ConvertArgument(ParameterInfo parameter, JSONValue value)
    {
        if (parameter.ParameterType == typeof(string))
            return (string) value;
        else if (parameter.ParameterType == typeof(int))
            return (int) value;
        else if (parameter.ParameterType == typeof(decimal))
            return (decimal) value;
        else if (parameter.ParameterType == typeof(DateTime))
            return (DateTime) value;
        else if (parameter.ParameterType == typeof(bool))
            return (bool) value;
        else if (parameter.ParameterType.IsSubclassOf(typeof(JSONValue)))
            return value;
        else if (parameter.ParameterType.IsArray && value is JSONArray)
        {
            var arr = (JSONArray) value;
            switch (parameter.ParameterType.Name)
            {
                case "Int32[]":
                    return arr.Values.Select(v => (int)v).ToArray();
                case "String[]":
                    return arr.Values.Select(v => (string)v).ToArray();
                case "Decimal[]":
                    return arr.Values.Select(v => (decimal)v).ToArray();
                case "Double[]":
                    return arr.Values.Select(v => Convert.ToDouble((decimal)v)).ToArray();
                case "Boolean[]":
                    return arr.Values.Select(v => (bool)v).ToArray();
                case "DateTime[]":
                    return arr.Values.Select(v => (DateTime)v).ToArray();
            }
            throw new ArgumentException("Parameter type on supported: " + parameter.ParameterType.Name);
        }
        else if (parameter.ParameterType.GetInterface("IDictionary") != null && parameter.ParameterType.IsGenericParameter && parameter.ParameterType.GetGenericArguments()[0].GetType() == typeof(string))
        {
            Type[] typeArgs = parameter.ParameterType.GetGenericArguments();
            Type genericType = typeof(Dictionary<,>).MakeGenericType(typeArgs);
            IDictionary obj = (IDictionary)Activator.CreateInstance(genericType);
            JSONObject jo = (JSONObject)value;
            foreach (var key in jo.Keys)
                obj.Add(key, jo[key].Value);
            return obj;
        }
        else
        {
            TypeConverter converter = TypeDescriptor.GetConverter(parameter.ParameterType);
            try
            {
                return converter.ConvertFromString(value.Value.ToString());
            }
            catch
            {
                throw new ArgumentException("Supplied argument for " + parameter.Name + " could not be converted into type " + parameter.ParameterType.FullName);
            }
        }
    }

    private static string ReadRequest(HttpContext context)
    {
        using (StreamReader reader = new StreamReader(context.Request.InputStream))
        {
            return reader.ReadToEnd();
        }
    }

    private void WriteResponse(HttpContext context, string value)
    {
        byte[] buffer = Encoding.UTF8.GetBytes(value);
        int length = buffer.Length;
        context.Response.AddHeader("Content-Length", length.ToString());
        context.Response.OutputStream.Write(buffer, 0, length);
    }

    // Properties
    public bool IsReusable
    {
        get
        {
            return true;
        }
    }
}


}
