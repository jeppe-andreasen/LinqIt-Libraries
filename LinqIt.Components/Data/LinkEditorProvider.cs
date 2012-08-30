using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public abstract class LinkEditorProvider
    {
        public abstract Type InternalTreeProviderType { get; }

        public abstract Type MediaTreeProviderType { get; }

        protected internal abstract string GetInternalUrl(string itemId);

        protected internal abstract string GetMediaUrl(string itemId);
    }
}
