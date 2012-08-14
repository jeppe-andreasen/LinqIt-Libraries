using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using LinqIt.Mail.Data;
using LinqIt.Mail.Templates;
using LinqIt.Utils.Extensions;

namespace LinqIt.Mail
{
    public class MailService
    {
        private readonly SmtpClient _client;

        public event EventHandler<MailModificationArgs> BeforeSend;

        public MailService()
        {
            _client = new SmtpClient();
        }

        public MailService(string server)
        {
            _client = new SmtpClient(server);
        }

        private static MailMessage GetMessage(MailTemplate template, out string errorMessage)
        {
            errorMessage = string.Empty;
            try
            {
                var mailMessage = new MailMessage();
                mailMessage.IsBodyHtml = true;

                var body = ProcessBodyHtml(template.GetBody());
                if (template.PerformCssInlineProcessing)
                    body = new CssInliner().Process(body);

                mailMessage.Body = body;
                mailMessage.Subject = template.GetSubject();
                mailMessage.Sender = template.GetSender();
                mailMessage.From = template.GetSender();

                foreach (var recipient in template.GetRecipients())
                    mailMessage.To.Add(recipient);

                foreach (var recipient in template.GetBccRecipients())
                    mailMessage.Bcc.Add(recipient);

                foreach (var attachment in template.GetAttachments())
                    mailMessage.Attachments.Add(attachment);

                return mailMessage;
            }
            catch (Exception exc)
            {
                errorMessage = exc.ToString();
                return null;
            }
        }

        private static string ProcessBodyHtml(string html)
        {
            var uri = HttpContext.Current.Request.Url;
            html = Regex.Replace(html, @"(src|href)=(""|')/", "$1=$2" + uri.Scheme + "://" + uri.Host + "/");

            return html;
        }

        public bool Dispatch(MailTemplate template)
        {
            string errorMessage;
            var mailMessage = GetMessage(template, out errorMessage);
            if (mailMessage == null)
                return false;

            var sendSuccess = mailMessage != null;
            try
            {
                if (BeforeSend != null)
                {
                    var args = new MailModificationArgs(mailMessage);
                    BeforeSend(this, args);
                    if (args.Abort)
                        return false;
                }
                _client.Send(mailMessage);
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                sendSuccess = false;
            }

            if (template.StoreHistory)
            {
                var mailId = StoreMailHistory(mailMessage, template.TypeName, sendSuccess, errorMessage);
                template.MailId = mailId;
            }

            return sendSuccess;
        }

        public static bool Send(MailTemplate template)
        {
            var service = new MailService();
            return service.Dispatch(template);
        }

        public bool Resend(int mailId)
        {
            var context = GetDataContext();
            var mail = context.MailHistories.Where(m => m.Id == mailId).FirstOrDefault();
            if (mail == null || mail.Success)
                return false;

            var mailMessage = new MailMessage();
            mailMessage.IsBodyHtml = true;
            mailMessage.Body = mail.Body;
            mailMessage.Subject = mail.Subject;
            mailMessage.Sender = mailMessage.From = new MailAddress(mail.SenderName, mail.SenderAddress);
            foreach (string recipient in mail.Recipients.Split(';'))
                mailMessage.To.Add(recipient);

            foreach (string bccRecipient in mail.BccRecipients.Split(';'))
                mailMessage.Bcc.Add(bccRecipient);

            foreach (string attachmentPath in mail.Attachments.Split(';'))
                mailMessage.Attachments.Add(new Attachment(attachmentPath));


            string errorMessage = string.Empty;

            var sendSuccess = mailMessage != null;
            try
            {
                _client.Send(mailMessage);
            }
            catch (Exception e)
            {
                errorMessage = e.ToString();
                sendSuccess = false;
            }

            mail.LastSendDate = DateTime.Now;
            mail.Success = sendSuccess;
            if (!sendSuccess)
                mail.ErrorMessage = errorMessage;

            context.SubmitChanges();

            return sendSuccess;
        }

        // This method can be used for testing purposes, to save an email to disk instead of sending it.
        public static bool Save(MailTemplate template, string filename)
        {
            // make sure extension is .eml
            filename = Path.ChangeExtension(filename, ".eml");

            string errorMessage;
            var message = GetMessage(template, out errorMessage);
            if (message == null)
                return false;

            var assembly = typeof(SmtpClient).Assembly;
            var mailWriterType = assembly.GetType("System.Net.Mail.MailWriter");

            using (var fileStream = new FileStream(filename, FileMode.Create))
            {
                var mailWriterContructor = mailWriterType.GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Stream) }, null);
                var mailWriter = mailWriterContructor.Invoke(new object[] { fileStream });
                var sendMethod = typeof(MailMessage).GetMethod("Send", BindingFlags.Instance | BindingFlags.NonPublic);
                sendMethod.Invoke(message, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { mailWriter, true }, null);
                var closeMethod = mailWriter.GetType().GetMethod("Close", BindingFlags.Instance | BindingFlags.NonPublic);
                closeMethod.Invoke(mailWriter, BindingFlags.Instance | BindingFlags.NonPublic, null, new object[] { }, null);
            }

            if (template.StoreHistory)
            {
                long? mailId = StoreMailHistory(message, template.TypeName, true, errorMessage);
                template.MailId = mailId;
            }


            return true;
        }

        private static long StoreMailHistory(System.Net.Mail.MailMessage mailMessage, string type, bool sendSuccess, string errorMessage)
        {
            var mail = new MailHistory();
            mail.CreatedOn = mail.LastSendDate = DateTime.Now;
            mail.SenderName = !string.IsNullOrEmpty(mailMessage.Sender.DisplayName) ? mailMessage.Sender.DisplayName : mailMessage.Sender.Address;
            mail.SenderAddress = mailMessage.Sender.Address;
            mail.Subject = mailMessage.Subject;
            mail.Success = sendSuccess;
            mail.Recipients = mailMessage.To.ToSeparatedString(";");
            mail.BccRecipients = mailMessage.Bcc.ToSeparatedString(";");
            mail.ErrorMessage = sendSuccess ? "" : errorMessage;
            mail.Body = mailMessage.Body;
            mail.Type = type;

            mail.Attachments = mailMessage.Attachments.Select(a => a.ContentDisposition.FileName).ToSeparatedString(";");

            var dataContext = GetDataContext();
            dataContext.MailHistories.InsertOnSubmit(mail);
            dataContext.SubmitChanges();

            return mail.Id;
        }

        private static HistoryDataContext GetDataContext()
        {
            HistoryDataContext result = null;
            if (System.Web.HttpContext.Current != null)
                result = (HistoryDataContext)System.Web.HttpContext.Current.Items["LinqIt.Mail.HistoryDataContext"];
            if (result == null)
            {
                result = new HistoryDataContext(ConfigurationManager.ConnectionStrings["linqit.mail"].ConnectionString);
                if (System.Web.HttpContext.Current != null)
                    System.Web.HttpContext.Current.Items["LinqIt.Mail.HistoryDataContext"] = result;
            }
            return result;
        }

        public static MailHistory GetMailHistory(long id)
        {
            return GetDataContext().MailHistories.Where(m => m.Id == id).FirstOrDefault();
        }

        public static IEnumerable<MailHistory> GetMails(string type, string recipient, DateTime? fromDate, DateTime? toDate, bool? successFull)
        {
            IEnumerable<MailHistory> mails = GetDataContext().MailHistories.OrderByDescending(m => m.LastSendDate);
            if (!string.IsNullOrEmpty(type))
                mails = mails.Where(m => m.Type == type);
            if (!string.IsNullOrEmpty(recipient))
                mails = mails.Where(m => m.Recipients.ToLower().Contains(recipient.ToLower()));
            if (fromDate.HasValue)
                mails = mails.Where(m => m.LastSendDate >= fromDate.Value);
            if (toDate.HasValue)
                mails = mails.Where(m => m.LastSendDate <= toDate.Value);
            if (successFull.HasValue)
                mails = mails.Where(m => m.Success == successFull.Value);

            return mails;
        }

        public IEnumerable<string> GetMailTypes()
        {
            return GetDataContext().MailHistories.Select(m => m.Type).Distinct().ToArray();
        }
    }

    public class MailModificationArgs : EventArgs
    {
        internal MailModificationArgs(MailMessage message)
        {
            Message = message;
        }

        public MailMessage Message { get; private set; }

        public bool Abort { get; set; }
    }
}
