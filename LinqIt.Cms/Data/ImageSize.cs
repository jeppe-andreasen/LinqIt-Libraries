using LinqIt.Utils.Extensions;

namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ImageSize
    {
        #region Constructors

        public ImageSize(int? width, int? height)
        {
            Width = width;
            Height = height;
        }

        #endregion Constructors

        #region Properties

        public bool AllowStretch
        {
            get; set;
        }

        public int? Height
        {
            get; set;
        }

        public int? Width
        {
            get; set;
        }

        public string QueryString
        {
            get
            {
                var parameters = new List<string>();
                if (Width.HasValue)
                    parameters.Add("w=" + Width.Value);
                if (Height.HasValue)
                    parameters.Add("h=" + Height.Value);
                return parameters.ToSeparatedString("&");
            }
        }

        #endregion Properties
    }
}