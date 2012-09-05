using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Components.Data;
using LinqIt.Utils.Extensions;

namespace LinqIt.Libraries.WebTest.Providers
{
    public class TestImageEditorProvider : ImageEditorProvider
    {
        public override Type ImageTreeProviderType
        {
            get { return typeof (TreeNodeProvider); }
        }

        public override void GetImageProperties(string itemId, out string url, out string alternativeText)
        {
            url = null;
            alternativeText = null;

            var provider = ProviderHelper.GetProvider<LinqIt.Components.Data.TreeNodeProvider>(ImageTreeProviderType.GetShortAssemblyName(), "jeppe");
            var node = provider.GetNode(itemId);
            if (!provider.GetChildNodes(node).Any())
            {
                url = "http://exiledonline.com/wp-content/uploads/2012/08/Paul-ryan-470x376.jpg";
                alternativeText = "Google";
            }
        }
    }
}