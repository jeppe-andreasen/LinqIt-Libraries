using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace LinqIt.Mail
{
    /// <summary>
    /// Summary description for MailHistory
    /// </summary>
    public class MailHistoryHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            context.Response.ContentType = "text/html";

            var mailHistory = MailService.GetMailHistory(Convert.ToInt64(context.Request.QueryString["id"]));
            if (mailHistory != null)
                context.Response.Write(mailHistory.Body);
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}
