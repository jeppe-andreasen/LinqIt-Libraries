using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Cms.Data.DataIterators;

namespace LinqIt.Libraries.WebTest
{
    public partial class TestConfigTransform : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            var transform = new WebConfigTransform();
            txtOutput.Text = transform.Process("Release");
        }
    }
}