using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class LinkListEditorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LinkListEditor1.LinkEditorProvider = typeof(LinqIt.Libraries.WebTest.Providers.TestLinkEditorProvider).AssemblyQualifiedName;
            LinkListEditor1.ReferenceId = "x";
        }
    }
}