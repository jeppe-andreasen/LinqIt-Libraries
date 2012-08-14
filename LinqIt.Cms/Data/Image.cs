namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    

    public class Image
    {
        #region Constructors

        public Image()
        {
        }

        public Image(Id id, string url, string alt)
        {
            this.Id = id;
            Url = url;
            AlternateText = alt;
        }

        #endregion Constructors

        #region Properties

        public string AlternateText
        {
            get; set;
        }

        public bool Exists
        {
            get
            {
                return !string.IsNullOrEmpty(Url);
            }
        }

        public string Url
        {
            get; private set;
        }

        public Id Id { get; set; }

        #endregion Properties

        #region Methods

        public void AssignTo(System.Web.UI.WebControls.Image imageControl)
        {
            imageControl.Visible = this.Exists;
            if (imageControl.Visible)
            {
                imageControl.ImageUrl = this.Url.StartsWith("/") ? this.Url : "/" + this.Url;
                imageControl.AlternateText = this.AlternateText;
            }
        }

        public void AssignTo(System.Web.UI.WebControls.Image imageControl, ImageSize size)
        {
            imageControl.Visible = this.Exists;
            if (imageControl.Visible)
            {
                imageControl.ImageUrl = this.Url; //.StartsWith("/") ? this.GetUrl(size) : "/" + this.GetUrl(size);
                imageControl.AlternateText = this.AlternateText;
            }
        }

        public void AssignTo(System.Web.UI.WebControls.HyperLink linkControl)
        {
            linkControl.Visible = this.Exists;
            if (linkControl.Visible)
            {
                linkControl.ImageUrl = this.Url.StartsWith("/") ? this.Url : "/" + this.Url;
                linkControl.Text = this.AlternateText;
            }
        }

        public string GetUrl(ImageSize imageSize)
        {
            if (string.IsNullOrEmpty(Url))
                return string.Empty;
            string url = CmsService.Instance.GetImageUrl(this, imageSize);
            if (url.StartsWith("~"))
                url = "/" + url;
            return url;
        }

        #endregion Methods
    }
}