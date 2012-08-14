using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using LinqIt.Utils.Web;

namespace LinqIt.Mail.Templates
{
    public abstract class MailTemplate
    {
        protected readonly string _recipient;
        private readonly string _subject;
        private readonly string _senderName;
        private readonly string _senderEmail;
        private readonly string _ccRecipients;
        private readonly string _bccRecipients;

        protected MailTemplate(string recipient, string ccRecipients, string bccRecipients)
        {
            _recipient = recipient;
            _bccRecipients = bccRecipients;
            _ccRecipients = ccRecipients;
        }

        protected MailTemplate(string subject, string senderName, string senderEmail, string recipient, string ccRecipients, string bccRecipients)
        {
            _subject = subject;
            _bccRecipients = bccRecipients;
            _ccRecipients = ccRecipients;
            _senderEmail = senderEmail;
            _senderName = senderName;
            _recipient = recipient;
        }

        protected internal abstract string GetBody();

        protected string FetchHtml(string localPath, NameValueCollection parameters)
        {
            var builder = new UrlBuilder(localPath, true);
            if (parameters != null)
            {
                if (string.IsNullOrEmpty(parameters["subject"]))
                    parameters["subject"] = GetSubject();
                if (string.IsNullOrEmpty(parameters["teaser"]) && !string.IsNullOrEmpty(Teaser))
                    parameters["teaser"] = Teaser;
                builder.Parameters.Add(parameters);
            }

            var client = new WebClient();
            client.Encoding = Encoding.UTF8;
            return client.DownloadString(builder.ToString());
        }

        public string Teaser { get; set; }

        protected string FetchHtml(string localPath, NameValueCollection parameters, NameValueCollection replacements)
        {
            var result = FetchHtml(localPath, parameters);
            if (replacements == null)
                return result;

            foreach (string key in replacements.Keys)
                result = result.Replace(key, replacements[key]);

            return result;
        }

        protected internal virtual string GetSubject()
        {
            if (string.IsNullOrEmpty(_subject))
                throw new ArgumentException("A subject must be specified on this request either by using an appropriate base constructor, or by overriding the GetSubject method");
            return _subject;
        }

        protected internal virtual MailAddress GetSender()
        {
            if (string.IsNullOrEmpty(_senderEmail))
                throw new ArgumentException("A sender address must be specified on this request either by using an appropriate base constructor, or by overriding the GetSender method");

            return !string.IsNullOrEmpty(_senderName) ? new MailAddress(_senderEmail, _senderName) : new MailAddress(_senderEmail);
        }

        protected internal virtual IEnumerable<string> GetRecipients()
        {
            return _recipient.Split(',', ';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
        }

        protected internal virtual IEnumerable<string> GetCcRecipients()
        {
            return _ccRecipients.Split(',', ';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
        }

        protected internal virtual IEnumerable<string> GetBccRecipients()
        {
            return _bccRecipients.Split(',', ';').Select(s => s.Trim()).Where(s => !string.IsNullOrEmpty(s));
        }

        protected internal virtual IEnumerable<Attachment> GetAttachments()
        {
            return new Attachment[0];
        }

        protected internal bool StoreHistory { get; set; }

        public abstract string TypeName { get; }

        public virtual bool PerformCssInlineProcessing { get { return false; } }

        public long? MailId { get; internal set; }
    }
}
