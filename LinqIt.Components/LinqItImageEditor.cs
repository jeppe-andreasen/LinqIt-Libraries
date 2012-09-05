using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax;
using LinqIt.Ajax.Parsing;
using LinqIt.Components.Data;
using LinqIt.Components.Utilities;
using LinqIt.Utils.Extensions;

namespace LinqIt.Components
{
    public class LinqItImageEditor : Control, INamingContainer
    {
        private HiddenField _hiddenValue;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!CancelIncludes)
            {
                ScriptUtility.RegisterEmbeddedCss(typeof(LinqItImageEditor), this, "LinqIt.Components.imageeditor.css");
                ScriptUtility.RegisterEmbeddedJs(typeof(LinqItImageEditor), this, "LinqIt.Components.imageeditor.js");
            }
        }

        public string Provider { get; set; }

        public string ReferenceId { get; set; }

        public string Value
        {
            get
            {
                EnsureChildControls();
                return _hiddenValue.Value;
            }
            set
            {
                EnsureChildControls();
                _hiddenValue.Value = value;
            }
        }

        protected override void CreateChildControls()
        {
            var provider = ProviderHelper.GetProvider<ImageEditorProvider>(Provider);

            base.CreateChildControls();
            AddLiteral("<div class=\"linqit-imageeditor\" data-provider=\"" + Provider + "\" data-referenceId=\"" + ReferenceId + "\">");
            AddLiteral("<div class=\"editor-output\">");
            AddLiteral("</div>");
            AddLiteral("<div class=\"editor-input\">");

            #region Tabs

            AddLiteral("<ul class=\"tabs\">");
            AddLiteral("<li><a href=\"#\" data-type=\"internal\" onclick=\"return linqit.imageeditor.selectTab(this);\"><span>Internal</span></a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"external\" onclick=\"return linqit.imageeditor.selectTab(this);\"><span>External</span></a></li>");
            AddLiteral("</ul>");

            #endregion

            AddLiteral("<div class=\"editor-form\">");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td>");

            #region Internal

            AddLiteral("<fieldset class=\"internal active\">");
            AddLiteral("<legend>Internal Image</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"tv\">");

            AddTree("internalTree", provider.ImageTreeProviderType, ReferenceId);

            AddLiteral("</td>");
            AddLiteral("<td class=\"labels\">");

            AddLabel("InternalPreview", "Preview", "preview", false);
            AddLabel("InternalText", "Alternative Text:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddPreviewBox("InternalPreview");
            AddTextBox("InternalText");
            
            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion

            #region External

            AddLiteral("<fieldset class=\"external\">");
            AddLiteral("<legend>External Image</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            AddLabel("ExternalPath", "Url:");
            AddLabel("ExternalPreview", "Preview", "preview", false);
            AddLabel("ExternalText", "Alternative Text:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("ExternalPath");
            AddPreviewBox("ExternalPreview");
            AddTextBox("ExternalText");


            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("");
            AddLiteral("</fieldset>");

            #endregion


            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("<tr>");
            AddLiteral("<td style=\"text-align:right;\" class=\"submitbuttons\">");

            AddButton("Ok", "linqit.imageeditor.updateValue(this); return false;", "Ok");
            AddButton("Cancel", "linqit.imageeditor.cancelUpdate(this); return false;", "Cancel");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("");
            AddLiteral("</div>");
            AddLiteral("</div>");

            _hiddenValue = new HiddenField() { ID = "hiddenValue" };
            Controls.Add(_hiddenValue);

            AddLiteral("</div>");
        }

        private void AddPreviewBox(string name)
        {
            AddLiteral("<div class=\"field previewbox\">");
            var image = new Image();
            image.ID = "img" + name;
            Controls.Add(image);
            AddLiteral("</div>");
        }

        private void AddButton(string name, string clientscript, string text)
        {
            var button = new Button();
            button.ID = "btn" + name;
            button.UseSubmitBehavior = false;
            button.OnClientClick = clientscript;
            button.Text = text;
            Controls.Add(button);
        }

        private void AddLabel(string name, string text, string fieldClass = null, bool associateWithTextBox = true)
        {
            if (fieldClass != null)
                AddLiteral("<div class=\"field " + fieldClass + " \">");
            else
                AddLiteral("<div class=\"field\">");
            var label = new Label();
            label.ID = "lbl" + name;
            if (associateWithTextBox)
                label.AssociatedControlID = "txt" + name;
            label.Text = text;
            Controls.Add(label);
            AddLiteral("</div>");
        }

        private void AddTextBox(string name, string defaultText = null)
        {
            AddLiteral("<div class=\"field\">");
            var textbox = new TextBox();
            textbox.ID = "txt" + name;
            if (!string.IsNullOrEmpty(defaultText))
                textbox.Text = defaultText;
            Controls.Add(textbox);
            AddLiteral("</div>");
        }

        private void AddTree(string id, Type providerType, string referenceId)
        {
            AddLiteral("<div class=\"treecontainer\">");
            var tree = new LinqItTreeView();
            tree.ID = id;
            tree.CancelIncludes = CancelIncludes;
            tree.Provider = providerType.GetShortAssemblyName();
            tree.ProviderReferenceId = referenceId;
            Controls.Add(tree);
            AddLiteral("</div>");
        }

        private void AddLiteral(string html)
        {
            Controls.Add(new LiteralControl(html));
        }

        public bool CancelIncludes { get; set; }

        [AjaxMethod(AjaxType.Sync)]
        public static JSONObject GetImageProperties(string providerName, string referenceId, string itemId)
        {
            var provider = ProviderHelper.GetProvider<ImageEditorProvider>(providerName, referenceId);

            string url;
            string alternativeText;
            provider.GetImageProperties(itemId, out url, out alternativeText);

            var result = new JSONObject();
            if (!string.IsNullOrEmpty(url))
                result.AddValue("url", url);
            else
                result.AddValue("url", JSONValue.Null);
            if (!string.IsNullOrEmpty(alternativeText))
                result.AddValue("alt", alternativeText);
            else
                result.AddValue("alt", JSONValue.Null);

            return result;
        }
    }
}