using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Utils.Extensions;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class LinkEditorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LinkEditor1.Provider = typeof(LinqIt.Libraries.WebTest.Providers.TestLinkEditorProvider).GetShortAssemblyName();
            LinkEditor1.ReferenceId = "x";
            LinkEditor1.Value = "<a type=\"internal\" target=\"_blank\" title=\"my tool\" class=\"my class\" itemId=\"3\" href=\"http://sofie?my query#my anc\">my text</a>";
        }
    }
}