using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax;
using LinqIt.Components.Data;
using LinqIt.Components.Utilities;
using LinqIt.Utils.Extensions;

namespace LinqIt.Components
{
    public class LinqItLinkEditor : Control, INamingContainer
    {
        private HiddenField _hiddenValue;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!CancelIncludes)
            {
                ScriptUtility.RegisterEmbeddedCss(typeof (LinqItLinkEditor), this, "LinqIt.Components.linkeditor.css");
                ScriptUtility.RegisterEmbeddedJs(typeof (LinqItLinkEditor), this, "LinqIt.Components.linkeditor.js");
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
            var provider = ProviderHelper.GetProvider<LinkEditorProvider>(Provider);
            
            base.CreateChildControls();
            AddLiteral("<div class=\"linqit-linkeditor\" data-provider=\"" + Provider + "\" data-referenceId=\"" + ReferenceId + "\">");
            AddLiteral("<div class=\"editor-output\">");
            AddLiteral("</div>");
            AddLiteral("<div class=\"editor-input\">");

            #region Tabs

            AddLiteral("<ul class=\"tabs\">");
            AddLiteral("<li><a href=\"#\" data-type=\"internal\" onclick=\"return linqit.linkeditor.selectTab(this);\">Internal</a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"external\" onclick=\"return linqit.linkeditor.selectTab(this);\">External</a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"media\" onclick=\"return linqit.linkeditor.selectTab(this);\">Media</a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"mailto\" onclick=\"return linqit.linkeditor.selectTab(this);\">Mail To</a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"javascript\" onclick=\"return linqit.linkeditor.selectTab(this);\">Javascript</a></li>");
            AddLiteral("<li><a href=\"#\" data-type=\"anchor\" onclick=\"return linqit.linkeditor.selectTab(this);\">Anchor</a></li>");
            AddLiteral("</ul>");

            #endregion

            AddLiteral("<div class=\"editor-form\">");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td>");

            #region Internal

            AddLiteral("<fieldset class=\"internal active\">");
            AddLiteral("<legend>Internal Link</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"tv\">");

            AddTree("internalTree", provider.InternalTreeProviderType, ReferenceId);

            AddLiteral("</td>");
            AddLiteral("<td class=\"labels\">");

            AddLabel("InternalText", "Link Text:");
            AddTargetLabel("InternalTarget", "Target:");
            AddLabel("InternalTooltip", "Tooltip:");
            AddLabel("InternalClass", "Css class:");
            AddLabel("InternalQueryString", "Querystring:");
            AddLabel("InternalAnchor", "Anchor:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("InternalText");
            AddTargetSelect("InternalTarget");
            AddTextBox("InternalTooltip");
            AddTextBox("InternalClass");
            AddTextBox("InternalQueryString");
            AddTextBox("InternalAnchor");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion 

            #region External

            AddLiteral("<fieldset class=\"external\">");
            AddLiteral("<legend>External Link</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            AddLabel("ExternalText", "Link Text:");
            AddLabel("ExternalPath", "Url:");
            AddTargetLabel("ExternalTarget", "Target:");
            AddLabel("ExternalTooltip", "Tooltip:");
            AddLabel("ExternalClass", "Css class:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("ExternalText");

            AddLiteral("<div class=\"field url\">");

            var txtExternalPath = new TextBox();
            txtExternalPath.ID = "txtExternalPath";
            txtExternalPath.Text = "http://";
            Controls.Add(txtExternalPath);

            AddButton("ExternalTestUrl", "return linqit.linkeditor.testUrl(this);", "Test");
            
            AddLiteral("</div>");

            AddTargetSelect("ExternalTarget");
            AddTextBox("ExternalTooltip");
            AddTextBox("ExternalClass");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("");
            AddLiteral("</fieldset>");

            #endregion 

            #region Media 

            AddLiteral("<fieldset class=\"media\">");
            AddLiteral("<legend>Media Link</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"tv\">");

            AddTree("mediaTree", provider.MediaTreeProviderType, ReferenceId);

            AddLiteral("</td>");
            AddLiteral("<td style=\"padding-left:10px;\">");
            AddLiteral("<div class=\"media-preview\">");
            AddLiteral("</div>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            AddLabel("MediaText", "Link Text:");
            AddTargetLabel("MediaTarget", "Target:");
            AddLabel("MediaTooltip", "Tooltip:");
            AddLabel("MediaClass", "Css class:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("MediaText");
            AddTargetSelect("MediaTarget");
            AddTextBox("MediaTooltip");
            AddTextBox("MediaClass");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion 

            #region MailTo

            AddLiteral("<fieldset class=\"mailto\">");
            AddLiteral("<legend>MailTo Link</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            AddLabel("MailToText", "Link Text:");
            AddLabel("MailToPath", "Email address:");
            AddLabel("MailToSubject", "Subject:");
            AddLabel("MailToTooltip", "Tooltip:");
            AddLabel("MailToClass", "Css class:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("MailToText");
            AddTextBox("MailToPath");
            AddTextBox("MailToSubject");
            AddTextBox("MailToTooltip");
            AddTextBox("MailToClass");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion

            #region Javascript

            AddLiteral("<fieldset class=\"javascript\">");
            AddLiteral("<legend>Javascript</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            #endregion

            #region Javascript

            AddLabel("JavascriptText", "Link Text:");
            AddLabel("JavascriptCode", "Javascript:", "multiline");
            AddLabel("JavascriptTooltip", "Tooltip:");
            AddLabel("JavascriptClass", "Css class:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("JavascriptText");

            AddLiteral("<div class=\"field multiline\">");

            var txtJavascriptCode = new TextBox();
            txtJavascriptCode.ID = "txtJavascriptCode";
            txtJavascriptCode.TextMode = TextBoxMode.MultiLine;
            Controls.Add(txtJavascriptCode);
            
            AddLiteral("</div>");

            AddTextBox("JavascriptTooltip");
            AddTextBox("JavascriptClass");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion

            #region Anchor

            AddLiteral("<fieldset class=\"anchor\">");
            AddLiteral("<legend>Insert a link to an Anchor</legend>");
            AddLiteral("<table>");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"labels\" style=\"padding-left:0;\">");

            AddLabel("AnchorText", "Link Text:");
            AddLabel("AnchorPath", "Anchor:");
            AddLabel("AnchorTooltip", "Tooltip:");
            AddLabel("AnchorClass", "Css class:");

            AddLiteral("</td>");
            AddLiteral("<td>");

            AddTextBox("AnchorText");
            AddTextBox("AnchorPath");
            AddTextBox("AnchorTooltip");
            AddTextBox("AnchorClass");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("</fieldset>");

            #endregion

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("<tr>");
            AddLiteral("<td style=\"text-align:right;\">");

            AddButton("Ok", "linqit.linkeditor.updateValue(this); return false;", "Ok");
            AddButton("Cancel", "linqit.linkeditor.cancelUpdate(this); return false;", "Cancel");

            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("");
            AddLiteral("</div>");
            AddLiteral("</div>");

            _hiddenValue = new HiddenField() {ID = "hiddenValue"};
            Controls.Add(_hiddenValue);
            
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

        private void AddTargetLabel(string name, string text)
        {
            AddLiteral("<div class=\"field\">");
            var label = new Label();
            label.ID = "lbl" + name;
            label.AssociatedControlID = "ddl" + name;
            label.Text = text;
            Controls.Add(label);
            AddLiteral("</div>");
        }

        private void AddLabel(string name, string text, string fieldClass = null)
        {
            if (fieldClass != null)
                AddLiteral("<div class=\"field " + fieldClass + " \">");
            else
                AddLiteral("<div class=\"field\">");
            var label = new Label();
            label.ID = "lbl" + name;
            label.AssociatedControlID = "txt" + name;
            label.Text = text;
            Controls.Add(label);
            AddLiteral("</div>");
        }

        private void AddTargetSelect(string name)
        {
            AddLiteral("<div class=\"field\">");
            var dropdown = new DropDownList();
            dropdown.ID = "ddl" + name;
            dropdown.Items.Add(new ListItem("", ""));
            dropdown.Items.Add(new ListItem("Opens in the current window", "_self"));
            dropdown.Items.Add(new ListItem("Opens in a new window or tab", "_blank"));
            dropdown.Items.Add(new ListItem("Opens in the parent frame", "_parent"));
            dropdown.Items.Add(new ListItem("Opens in the top window", "_top"));
            Controls.Add(dropdown);
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
            var tree = new LinqItTreeView();
            tree.ID = id;
            tree.CancelIncludes = CancelIncludes;
            tree.Provider = providerType.GetShortAssemblyName();
            tree.ProviderReferenceId = referenceId;
            Controls.Add(tree);
        }

        private void AddLiteral(string html)
        {
            Controls.Add(new LiteralControl(html));
        }

        public bool CancelIncludes { get; set; }

        [AjaxMethod(AjaxType.Sync)]
        public static string GetInternalUrl(string providerName, string referenceId, string itemId, string query, string anchor)
        {
            var provider = ProviderHelper.GetProvider<LinkEditorProvider>(providerName, referenceId);
            var result = provider.GetInternalUrl(itemId);
            if (!string.IsNullOrEmpty(query))
                result += "?" + query.TrimStart('?');
            if (!string.IsNullOrEmpty(anchor))
                result += "#" + anchor.TrimStart('#');
            return result;
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string GetMediaUrl(string providerName, string referenceId, string itemId)
        {
            var provider = ProviderHelper.GetProvider<LinkEditorProvider>(providerName, referenceId);
            return provider.GetMediaUrl(itemId);
        }
    }
}
