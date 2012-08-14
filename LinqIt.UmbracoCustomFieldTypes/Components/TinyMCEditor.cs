using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax.Parsing;
using LinqIt.Utils.Extensions;

namespace LinqIt.UmbracoCustomFieldTypes.Components
{
    public class TinyMCEditor : Control, INamingContainer
    {
        private TextBox _textbox;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            if (!Page.ClientScript.IsClientScriptIncludeRegistered(Page.GetType(), "tinyMCEditSrc"))
                Page.ClientScript.RegisterClientScriptInclude(Page.GetType(), "tinyMCEditSrc", "/assets/lib/tiny_mce/tiny_mce.js");
        }

        public string HiddenId { get; set; }

        public bool RegisterFormBind { get; set; }

        public Unit? Width
        {
            get
            {
                EnsureChildControls();
                return _textbox.Width;
            }
            set
            {
                EnsureChildControls();
                if (value.HasValue)
                    _textbox.Width = value.Value;
            }
        }

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            _textbox = new TextBox();
            _textbox.TextMode = TextBoxMode.MultiLine;
            _textbox.CssClass = "tinymceditor";
            Controls.Add(_textbox);
        }

        protected override void OnPreRender(EventArgs e)
        {
            var options = new JSONObject();
            options.AddValue("mode", "exact");
            if (Width.HasValue)
                options.AddValue("width", Width.Value.Value.ToString());
            options.AddValue("elements", _textbox.ClientID);
            options.AddValue("theme", "advanced");
            options.AddValue("plugins", GetPlugins());
            options.AddValue("theme_advanced_buttons1", GetButtons1());
            options.AddValue("theme_advanced_buttons2", GetButtons2());
            options.AddValue("theme_advanced_buttons3", GetButtons3());
            //options.AddValue("theme_advanced_buttons4", GetButtons4());
            options.AddValue("theme_advanced_toolbar_location", "top");
            options.AddValue("theme_advanced_toolbar_align", "left");
            options.AddValue("theme_advanced_statusbar_location", "bottom");
            //options.AddValue("theme_advanced_resizing", true);
            options.AddValue("template_external_list_url", "lists/template_list.js");
            options.AddValue("external_link_list_url", "lists/link_list.js");
            options.AddValue("external_image_list_url", "lists/image_list.js");
            options.AddValue("media_external_list_url", "lists/media_list.js");
            //options.AddValue("style_formats", JSONArray.Parse("[{title : 'Bold text', inline : 'b'},{title : 'Red text', inline : 'span', styles : {color : '#ff0000'}},{title : 'Red header', block : 'h1', styles : {color : '#ff0000'}},{title : 'Example 1', inline : 'span', classes : 'example1'},{title : 'Example 2', inline : 'span', classes : 'example2'},{title : 'Table styles'},{title : 'Table row 1', selector : 'tr', classes : 'tablerow1'}]"));
            options.AddValue("template_replace_values", JSONObject.Parse("{ username : \"Some User\", staffid : \"991234\" }"));

            if (!string.IsNullOrEmpty(HiddenId))
            {
                var setup = new JSONDelegate("ed");
                setup.Lines.Add("ed.onInit.add(function(ed, evt) {");
                setup.Lines.Add("initializeEditorValue(ed, doc, '" + HiddenId + "');");
                setup.Lines.Add("var dom = ed.dom;");
                setup.Lines.Add("var doc = ed.getDoc();");
                setup.Lines.Add("tinymce.dom.Event.add(doc, 'blur', function(e) {");
                setup.Lines.Add("updateEditorValue(ed, doc,'" + HiddenId + "');");
                setup.Lines.Add("});");
                setup.Lines.Add("});");
                options.AddValue("setup", setup);
            }
            var script = new StringBuilder();
            script.AppendLine("tinyMCE.init(" + options.ToString() + ");");

            if (RegisterFormBind)
                script.AppendLine("$(theForm).bind(\"onSave\", function() { $(\"#" + _textbox.ClientID + "\").val(tinyMCE.get('" + _textbox.ClientID + "').save()); });");

            Page.ClientScript.RegisterStartupScript(Page.GetType(), "tinyMCEditor" + _textbox.ClientID, script.ToString(), true);


            base.OnPreRender(e);
        }

        private static string GetPlugins()
        {
            var result = new List<string>();
            result.Add("autolink");
            result.Add("lists");
            result.Add("pagebreak");
            result.Add("style");
            result.Add("layer");
            result.Add("table");
            result.Add("save");
            result.Add("advhr");
            result.Add("advimage");
            result.Add("advlink");
            result.Add("emotions");
            result.Add("iespell");
            result.Add("inlinepopups");
            result.Add("insertdatetime");
            result.Add("preview");
            result.Add("media");
            result.Add("searchreplace");
            result.Add("print");
            result.Add("contextmenu");
            result.Add("paste");
            result.Add("directionality");
            result.Add("fullscreen");
            result.Add("noneditable");
            result.Add("visualchars");
            result.Add("nonbreaking");
            result.Add("xhtmlxtras");
            result.Add("template");
            result.Add("wordcount");
            result.Add("advlist");
            result.Add("autosave");
            result.Add("visualblocks");
            return result.ToSeparatedString(",");
        }

        private static IEnumerable<string> GetButtons1List()
        {
            var result = new List<string>();
            result.Add("bold");
            result.Add("italic");
            result.Add("underline");
            result.Add("strikethrough");
            result.Add("|");
            result.Add("justifyleft");
            result.Add("justifycenter");
            result.Add("justifyright");
            result.Add("justifyfull");
            result.Add("styleselect");
            result.Add("formatselect");
            result.Add("fontselect");
            result.Add("fontsizeselect");
            return result;
        }

        private static string GetButtons1()
        {
            return GetButtons1List().ToSeparatedString(",");
        }

        private static IEnumerable<string> GetButtons2List()
        {
            var result = new List<string>();
            result.Add("cut");
            result.Add("copy");
            result.Add("paste");
            result.Add("pastetext");
            result.Add("pasteword");
            result.Add("|");
            result.Add("search");
            result.Add("replace");
            result.Add("|");
            result.Add("bullist");
            result.Add("numlist");
            result.Add("|");
            result.Add("outdent");
            result.Add("indent");
            result.Add("blockquote");
            result.Add("|");
            result.Add("undo");
            result.Add("redo");
            result.Add("|");
            result.Add("link");
            result.Add("unlink");
            result.Add("anchor");
            result.Add("image");
            result.Add("cleanup");
            result.Add("help");
            result.Add("code");
            result.Add("|");
            result.Add("insertdate");
            result.Add("inserttime");
            result.Add("preview");
            result.Add("|");
            result.Add("forecolor");
            result.Add("backcolor");
            return result;
        }

        private static string GetButtons2()
        {
            return GetButtons2List().ToSeparatedString(",");
        }

        private static IEnumerable<string> GetButtons3List()
        {
            var result = new List<string>();
            result.Add("tablecontrols");
            result.Add("|");
            result.Add("hr");
            result.Add("removeformat");
            result.Add("visualaid");
            result.Add("|");
            result.Add("sub");
            result.Add("sup");
            result.Add("|");
            result.Add("charmap");
            result.Add("emotions");
            result.Add("iespell");
            result.Add("media");
            result.Add("advhr");
            result.Add("|");
            result.Add("print");
            result.Add("|");
            result.Add("ltr");
            result.Add("rtl");
            result.Add("|");
            result.Add("fullscreen");
            return result;
            
        }

        private static string GetButtons3()
        {
            
            return GetButtons3List().ToSeparatedString(",");
        }

        private static IEnumerable<string> GetButtons4List()
        {
            var result = new List<string>();
            result.Add("insertlayer");
            result.Add("moveforward");
            result.Add("movebackward");
            result.Add("absolute");
            result.Add("|");
            result.Add("styleprops");
            result.Add("|");
            result.Add("cite");
            result.Add("abbr");
            result.Add("acronym");
            result.Add("del");
            result.Add("ins");
            result.Add("attribs");
            result.Add("|");
            result.Add("visualchars");
            result.Add("nonbreaking");
            result.Add("template");
            result.Add("pagebreak");
            result.Add("restoredraft");
            result.Add("visualblocks");
            return result;
        }

        private static string GetButtons4()
        {
            return GetButtons4List().ToSeparatedString(",");
        }

        public string Value
        {
            get
            {
                EnsureChildControls();
                return _textbox.Text;
            }
            set
            {
                EnsureChildControls();
                _textbox.Text = value;
            }
        }
    }
}
