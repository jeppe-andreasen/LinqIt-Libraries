namespace LinqIt.Cms.Data
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class ClippingRectangle
    {
        #region Constructors

        public ClippingRectangle(int x, int y, int width, int height)
        {
            X = x;
            Y = y;
            Width = width;
            Height = height;
        }

        #endregion Constructors

        #region Properties

        public int Height
        {
            get; set;
        }

        public int Width
        {
            get; set;
        }

        public int X
        {
            get; set;
        }

        public int Y
        {
            get; set;
        }

        #endregion Properties
    }
}