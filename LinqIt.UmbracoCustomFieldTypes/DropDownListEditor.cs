using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using LinqIt.Cms;
using LinqIt.Components.Data;
using LinqIt.Utils.Extensions;
using umbraco.cms.businesslogic.datatype;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class DropDownListEditor: umbraco.cms.businesslogic.datatype.AbstractDataEditor
    {
        private readonly DropDownList _control = new DropDownList();

        [DataEditorSetting("Provider", description = "The assembly qualified name of the NodeProvider type which provides options", defaultValue = "")]
        public string Provider { get; set; }

        // Set ID, needs to be unique
        public override Guid Id
        {
            get
            {
                return new Guid("2B133DF3-DA6B-4EDD-85BE-CCA02C81C866");
            }
        }

        //Set name, (is what appears in data editor dropdown)
        public override string DataTypeName
        {
            get
            {
                return "Flexible DropDownList";
            }
        }

        public DropDownListEditor()
        {
            base.RenderControl = _control;
            _control.Init += OnControlInitialized;
            DataEditorControl.OnSave += OnControlSaved;
        }

        void OnControlInitialized(object sender, EventArgs e)
        {
            using (CmsContext.Editing)
            {
                var referenceId = _control.Page.Request.QueryString["id"];
                var provider = ProviderHelper.GetProvider<NodeProvider>(Provider, referenceId);
                _control.AddDefaultItem();
                foreach (var node in provider.GetRootNodes())
                    _control.Items.Add(new ListItem(node.Text, node.Id));

                if (base.Data.Value != null)
                {
                    var item = _control.Items.FindByValue(base.Data.Value.ToString());
                    if (item != null)
                        _control.SelectedIndex = _control.Items.IndexOf(item);
                    else
                        _control.SelectedIndex = 0;
                }
                else
                    _control.SelectedIndex = 0;
            }
        }

        void OnControlSaved(EventArgs e)
        {
            base.Data.Value = _control.SelectedValue;
        }
    }

}