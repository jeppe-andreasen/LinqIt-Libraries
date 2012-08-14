using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using LinqIt.UmbracoCustomFieldTypes.Components;
using umbraco.interfaces;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class MacroRichTextEditor : Control, INamingContainer, IMacroGuiRendering
    {
        private HtmlGenericControl _iframe;
        private HiddenField _hiddenField;

        protected override void CreateChildControls()
        {
            
            _hiddenField = new HiddenField();
            _hiddenField.ID = "hiddenField";
            Controls.Add(_hiddenField);
            
            _iframe = new HtmlGenericControl("iframe");
            _iframe.Attributes.Add("id", "editor-frame");
            _iframe.Attributes.Add("src", "/handlers/TinyMCEditorHandler.aspx?hiddenId=" + _hiddenField.ClientID);
            _iframe.Attributes.Add("style", "width:720px;");
            _iframe.Attributes.Add("height", "210");
            _iframe.Attributes.Add("frameBorder", "0");
            _iframe.ID = "iframe";
            Controls.Add(_iframe);

            base.CreateChildControls();
        }


        public bool ShowCaption
        {
            get { return true; }
        }

        public string Value
        {
            get
            {
                EnsureChildControls();
                return System.Web.HttpUtility.UrlEncode(_hiddenField.Value);
            }
            set
            {
                EnsureChildControls();
                _hiddenField.Value = System.Web.HttpUtility.UrlDecode(value);
            }
        }
    }
}
