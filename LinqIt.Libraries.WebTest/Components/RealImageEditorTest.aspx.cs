using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Utils.Extensions;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class RealImageEditorTest : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LinqItImageEditor1.Provider = typeof(LinqIt.Libraries.WebTest.Providers.TestImageEditorProvider).GetShortAssemblyName();
            LinqItImageEditor1.ReferenceId = "Jeppe";
        }
    }
}