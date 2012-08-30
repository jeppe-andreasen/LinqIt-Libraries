using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax;
using LinqIt.Ajax.Parsing;
using LinqIt.Components.Data;
using LinqIt.Components.Utilities;
using LinqIt.Utils.Web;
using TreeNode = LinqIt.Components.Data.Node;


namespace LinqIt.Components
{

    [DefaultProperty("Provider")]
    [ToolboxData("<{0}:LinqItTreeView runat=server></{0}:LinqItTreeView>")]
    public class LinqItTreeView : Control, INamingContainer
    {
        private HiddenField _hiddenField;

        #region Properties

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string ItemId { get { return (string)ViewState["ItemId"] ?? string.Empty; } set { ViewState["ItemId"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string Provider { get { return (string)ViewState["Provider"] ?? string.Empty; } set { ViewState["Provider"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string ProviderReferenceId { get { return (string)ViewState["ProviderReferenceId"] ?? string.Empty; } set { ViewState["ProviderReferenceId"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string ContextMenuProvider { get { return (string)ViewState["ContextMenuProvider"] ?? string.Empty; } set { ViewState["ContextMenuProvider"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string SelectedValue
        {
            get
            {
                EnsureChildControls();
                return _hiddenField.Value;
            }
            set
            {
                EnsureChildControls();
                _hiddenField.Value = value;
            }
        }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string DropTarget { get { return (string)ViewState["DropTarget"] ?? string.Empty; } set { ViewState["DropTarget"] = value; } }

        #endregion

        protected override void CreateChildControls()
        {
            _hiddenField = new HiddenField();
            _hiddenField.ID = "hiddenValue";
            Controls.Add(_hiddenField);
            base.CreateChildControls();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!CancelIncludes)
            {
                ScriptUtility.RegisterEmbeddedCss(typeof(LinqItTreeView), this, "LinqIt.Components.treeview.css");
                ScriptUtility.RegisterEmbeddedJs(typeof(AjaxUtil), this, "LinqIt.Ajax.combined.js");
                ScriptUtility.RegisterEmbeddedJs(typeof(LinqItTreeView), this, "LinqIt.Components.treeview.js");
            }
        }

        public bool UseEmbeddedJs { get; set; }

        public bool UseEmbeddedCss { get; set; }

        protected override void Render(HtmlTextWriter writer)
        {
            EnsureChildControls();
            GenerateRoot(new HtmlWriter(writer), ProviderHelper.GetTreeNodeProvider(Provider, ProviderReferenceId), SelectedValue);
            base.Render(writer);
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string FetchChildren(string value, string provider, string referenceId)
        {
            return HtmlWriter.Generate(writer => GenerateChildren(writer, value, ProviderHelper.GetTreeNodeProvider(provider, referenceId)));
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string FetchNode(string value, string providerName, string referenceId, bool expanded)
        {
            var provider = ProviderHelper.GetTreeNodeProvider(providerName, referenceId);
            var treeNode = provider.GetNode(value);
            return HtmlWriter.Generate(writer => RenderItem(writer, treeNode, provider, new[] { treeNode }, null, true));
        }

        [AjaxMethod(AjaxType.Sync)]
        public static JSONObject FindNode(string value, string providerName, string referenceId)
        {
            var provider = ProviderHelper.GetTreeNodeProvider(providerName, referenceId);
            var treeNode = provider.GetNode(value);
            var found = treeNode != null;
            var result = new JSONObject();
            result.AddValue("found", found);
            if (found)
            {

                var expandedItems = new List<Node>();
                var parent = provider.GetParentNode(treeNode);
                var root = parent;
                while (parent != null)
                {
                    expandedItems.Insert(0, parent);
                    parent = provider.GetParentNode(parent);
                    if (parent != null)
                        root = parent;
                }
                var html = HtmlWriter.Generate(w => RenderItem(w, root, provider, expandedItems, value, true));
                result.AddValue("root", root.Id);
                result.AddValue("html", html);
            }
            return result;
        }

        private static void GenerateChildren(HtmlWriter writer, string value, TreeNodeProvider provider)
        {
            var parent = provider.GetNode(value);
            var children = provider.GetChildNodes(parent);
            if (!children.Any())
                return;

            writer.RenderBeginTag(HtmlTextWriterTag.Ul);
            foreach (var node in children)
                RenderItem(writer, node, provider, null, null);
            writer.RenderEndTag();
        }

        private void GenerateRoot(HtmlWriter writer, TreeNodeProvider provider, string value)
        {
            if (provider == null)
            {
                writer.Write("Please specify a provider for the treeview");
                return;
            }

            var rootItems = provider.GetRootNodes().ToArray();
            var selectedItem = !string.IsNullOrEmpty(value) ? provider.GetNode(value) : null;
            var expandedItems = new List<TreeNode>();
            if (selectedItem != null && !rootItems.Where(n => n.Id == value).Any())
            {
                var parent = provider.GetParentNode(selectedItem);
                while (parent != null && !rootItems.Where(i => i.Id == parent.Id).Any())
                {
                    expandedItems.Add(parent);
                    parent = provider.GetParentNode(parent);
                }
            }
            writer.AddAttribute("hiddenId", _hiddenField.ClientID);
            writer.AddAttribute("data-provider", Provider);
            if (!string.IsNullOrEmpty(ContextMenuProvider))
                writer.AddAttribute("data-contextmenuprovider", ContextMenuProvider);
            writer.AddAttribute("data-item", ItemId);
            if (!string.IsNullOrEmpty(DropTarget))
                writer.AddAttribute("data-droptarget", DropTarget);
            writer.AddAttribute(HtmlTextWriterAttribute.Id, ClientID);
            if (!string.IsNullOrEmpty(ProviderReferenceId))
                writer.AddAttribute("data-referenceId", ProviderReferenceId);
            writer.RenderBeginTag(HtmlTextWriterTag.Ul, "linqit-treeview");
            foreach (var node in rootItems)
            {
                RenderItem(writer, node, provider, expandedItems.AsEnumerable(), value);
            }
            writer.RenderEndTag();
        }

        private static void RenderItem(HtmlWriter writer, TreeNode node, TreeNodeProvider provider, IEnumerable<TreeNode> expandedItems, string selectedValue, bool omitOuterTag = false)
        {
            bool expanded = expandedItems != null && expandedItems.Where(n => n.Id == node.Id).Any();
            if (!omitOuterTag)
                writer.RenderBeginTag(HtmlTextWriterTag.Li);
            if (provider.GetChildNodes(node).Any())
            {
                if (expanded)
                    writer.AddClass("expanded");
                writer.RenderLinkTag("#", "", "toggler");
            }
            else
            {
                writer.RenderLinkTag("#", "", "toggler disabled");
            }
            writer.AddAttribute("ref", node.Id);
            writer.AddAttribute(HtmlTextWriterAttribute.Title, node.HelpText);
            writer.AddAttribute("selectable", node.Selectable ? "true" : "false");
            writer.AddClass("node");
            if (!string.IsNullOrEmpty(selectedValue) && node.Id == selectedValue)
                writer.AddClass("selected");
            if (node.Draggable)
                writer.AddClass("draggable");
            writer.RenderBeginLink("#");
            writer.RenderImageTag(node.Icon, null, null);
            writer.RenderFullTag(HtmlTextWriterTag.Span, node.Text);
            writer.RenderEndTag(); // a

            if (expanded)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (var child in provider.GetChildNodes(node))
                    RenderItem(writer, child, provider, expandedItems, selectedValue);
                writer.RenderEndTag();
            }
            if (!omitOuterTag)
                writer.RenderEndTag(); // li
        }

        public bool CancelIncludes { get; set; }
    }
}

