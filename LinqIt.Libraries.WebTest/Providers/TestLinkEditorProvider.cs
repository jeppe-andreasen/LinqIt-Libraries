using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Components.Data;

namespace LinqIt.Libraries.WebTest.Providers
{
    public class TestLinkEditorProvider : LinkEditorProvider
    {
        public override Type InternalTreeProviderType
        {
            get { return typeof (TreeNodeProvider); }
        }

        public override Type MediaTreeProviderType
        {
            get { return typeof(TreeNodeProvider); }
        }

        protected override string GetInternalUrl(string itemId)
        {
            return "http://" + itemId + ".aspx";
        }

        protected override string GetMediaUrl(string itemId)
        {
            return "http://media" + itemId + ".aspx";
        }
    }
}