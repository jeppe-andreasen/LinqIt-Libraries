namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    

    public enum LinkType
    {
        Internal,
        Media,
        External,
        MailTo,
        Anchor,
        JavaScript
    };
       
    public class Link
    {
        #region Constructors

        public Link()
        {
            
        }

        public Link(Page page)
        {
            if (page != null)
            {
                this.LinkType = LinkType.Internal;
                this.LinkedItemId = page.Id;
                this.Href = page.Url;
            }
        }

        //public Link(string title, string href, string cssClass)
        //{
        //    Title = title;
        //    Href = href;
        //    CssClass = cssClass;
        //}

        #endregion Constructors

        #region Properties

        public string CssClass
        {
            get; set;
        }

        public string Href
        {
            get; set;
        }

        public string Title
        {
            get; set;
        }

        #endregion Properties

        public LinkType LinkType { get; set; }

        public Id LinkedItemId { get; set; }

        public string AlternateText { get; set; }

        public string Target { get; set; }

        public string Anchor { get; set; }

        public static Link Parse(string value)
        {
            return CmsService.Instance.ParseLink(value);
        }

        public override string ToString()
        {
            return CmsService.Instance.ConvertToString(this);
        }
    }
}