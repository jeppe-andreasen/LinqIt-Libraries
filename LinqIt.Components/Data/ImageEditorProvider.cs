using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public abstract class ImageEditorProvider
    {
        public abstract Type ImageTreeProviderType { get; }

        public abstract void GetImageProperties(string itemId, out string url, out string alternativeText);
    }
}
