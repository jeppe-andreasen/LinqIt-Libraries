using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class LinkListEditor : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        public string LinkEditorProvider
        {
            get { return LinkEditor1.Provider; }
            set { LinkEditor1.Provider = value; }
        }

        public string ReferenceId
        {
            get { return LinkEditor1.ReferenceId; }
            set { LinkEditor1.ReferenceId = value; }
        }

        public bool CancelIncludes
        {
            get { return LinkEditor1.CancelIncludes; }
            set { LinkEditor1.CancelIncludes = value; }
        }

        public string Value
        {
            get { return hiddenValue.Value; }
            set { hiddenValue.Value = value; }
        }
    }
}