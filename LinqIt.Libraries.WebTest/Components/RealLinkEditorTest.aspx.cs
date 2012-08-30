using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Utils.Extensions;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class RealLinkEditorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LinqItLinkEditor1.Provider = typeof (LinqIt.Libraries.WebTest.Providers.TestLinkEditorProvider).GetShortAssemblyName();
            LinqItLinkEditor1.ReferenceId = "Jeppe";
        }
    }
}