using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax;
using LinqIt.Ajax.Parsing;
using LinqIt.Components.Data;
using LinqIt.Utils.Extensions;

namespace LinqIt.Libraries.WebTest.Components
{
    public partial class LinkEditor : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                InitializeDropDownList(ddlInternalTarget, ddlExternalTarget, ddlMediaTarget);

                var provider = ProviderHelper.GetProvider<LinkEditorProvider>(Provider, ReferenceId);
                internalTree.Provider = provider.InternalTreeProviderType.GetShortAssemblyName();
                internalTree.ProviderReferenceId = ReferenceId;

                mediaTree.Provider = provider.MediaTreeProviderType.GetShortAssemblyName();
                mediaTree.ProviderReferenceId = ReferenceId;
            }
        }

        private static void InitializeDropDownList(params DropDownList[] lists)
        {
            foreach (var list in lists)
            {
                list.Items.Add(new ListItem("", ""));
                list.Items.Add(new ListItem("Opens in the current window", "_self"));
                list.Items.Add(new ListItem("Opens in a new window or tab", "_blank"));
                list.Items.Add(new ListItem("Opens in the parent frame", "_parent"));
                list.Items.Add(new ListItem("Opens in the top window", "_top"));
            }
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string GetInternalUrl(string itemId, string query, string anchor)
        {
            var provider = ProviderHelper.GetProvider<TreeNodeProvider>(typeof(LinqIt.Libraries.WebTest.Providers.TreeNodeProvider).AssemblyQualifiedName, "");
            var node = provider.GetNode(itemId);
            var result = "http://" + node.Text;
            if (!string.IsNullOrEmpty(query))
                result += "?" + query.TrimStart('?');
            if (!string.IsNullOrEmpty(anchor))
                result += "#" + anchor.TrimStart('#');
            return result;
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string GetMediaUrl(string itemId)
        {
            var provider = ProviderHelper.GetProvider<TreeNodeProvider>(typeof(LinqIt.Libraries.WebTest.Providers.TreeNodeProvider).AssemblyQualifiedName, "");
            var node = provider.GetNode(itemId);
            var result = "http://" + node.Text;
            return result;
        }

        [AjaxMethod(AjaxType.Sync)]
        public static JSONObject ParseUrl(string href)
        {
            var result = new JSONObject();
            var parts = href.Split('#');
            result.AddValue("anchor", parts.Length > 1 ? parts[1] : "");
            result.AddValue("query", parts[0].Contains("?") ? parts[0].Substring(parts[0].IndexOf("?")+1) : "");
            return result;
        }

        [AjaxMethod(AjaxType.Sync)]
        public static JSONObject ParseMailTo(string mailTo)
        {
            var result = new JSONObject();
            var parts = mailTo.Replace("mailto:", "").Split('#');
            result.AddValue("address", parts[0]);
            result.AddValue("subject", parts.Length > 1 ? parts[1] : "");
            return result;
        }

        public string Provider { get; set; }
        
        public string ReferenceId { get; set; }

        public string Value
        {
            get { return hiddenValue.Value; }
            set { hiddenValue.Value = value; }
        }

        public bool CancelIncludes { get; set; }
    }
}