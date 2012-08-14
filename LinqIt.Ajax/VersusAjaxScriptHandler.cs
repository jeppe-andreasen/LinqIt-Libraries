using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Configuration;

namespace LinqIt.Ajax
{
    /// <summary>
    /// Summary description for LinqIt.Ajax.JS
    /// </summary>
    public class LinqItAjaxInclude : IHttpHandler {

        public void ProcessRequest (HttpContext context) {
            context.Response.ContentType = "application/x-javascript";
            context.Response.Write(Properties.Resources.JSON2_Min);
            context.Response.Write(Properties.Resources.Script_Min);
        }
     
        public bool IsReusable 
        {
            get 
            {
                return true;
            }
        }
    }
}
