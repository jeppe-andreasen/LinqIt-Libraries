using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Components.Utilities;

namespace LinqIt.Components
{
    public class LinqItLinkListEditor : Control, INamingContainer
    {
        private HiddenField _hiddenValue;
        private LinqItLinkEditor _editor;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!CancelIncludes)
            {
                ScriptUtility.RegisterEmbeddedCss(typeof(LinqItLinkListEditor), this, "LinqIt.Components.linklisteditor.css");
                ScriptUtility.RegisterEmbeddedJs(typeof(LinqItLinkListEditor), this, "LinqIt.Components.linklisteditor.js");
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            AddLiteral("<div class=\"linqit-linklisteditor\">");
            AddLiteral("<table style=\"width:100%\">");
            AddLiteral("<tr>");
            AddLiteral("<td class=\"links\">");
            AddLiteral("<ul class=\"sortable\">");
            AddLiteral("</ul>");
            AddLiteral("</td>");
            AddLiteral("<td class=\"editor\">");
            AddLiteral("<div class=\"editor\">");

            _editor = new LinqItLinkEditor();
            _editor.CancelIncludes = CancelIncludes;
            _editor.ID = "linkEditor";
            Controls.Add(_editor);

            AddLiteral("</div>");
            AddLiteral("</td>");
            AddLiteral("</tr>");
            AddLiteral("</table>");
            AddLiteral("<div class=\"buttons\">");

            AddButton("AddLink", "linqit.linklisteditor.addLink(this); return false;", "Add Link");
            AddButton("RemoveLink", "linqit.linklisteditor.removeLink(this); return false;", "Remove Link");

            AddLiteral("</div>");
            
            _hiddenValue = new HiddenField() { ID = "hiddenValue" };
            Controls.Add(_hiddenValue);
            
            AddLiteral("</div>");
        }

        private void AddLiteral(string html)
        {
            Controls.Add(new LiteralControl(html));
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

        public string Provider
        {
            get
            {
                EnsureChildControls();
                return _editor.Provider;
            }
            set
            {
                EnsureChildControls();
                _editor.Provider = value;
            }
        }

        public string ReferenceId
        {
            get
            {
                EnsureChildControls();
                return _editor.ReferenceId;
            }
            set
            {
                EnsureChildControls();
                _editor.ReferenceId = value;
            }
        }

        public bool CancelIncludes
        {
            get; set;
        }
    
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
    }
}
