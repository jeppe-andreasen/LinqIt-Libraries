using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components.Bootstrap;
using LinqIt.Utils.Extensions;

namespace LinqIt.UmbracoCustomFieldTypes
{
    [DefaultProperty("Provider")]
    [ToolboxData("<{0}:GridModulePlaceholder runat=server></{0}:GridModulePlaceholder>")]
    public class GridModulePlaceholder : Control, INamingContainer
    {
        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string Key { get { return (string)ViewState["Key"] ?? string.Empty; } set { ViewState["Key"] = value; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var placeholder = new BootstrapGridModulePlaceholder();
            placeholder.Provider = typeof(UmbracoTreeModuleProvider).GetShortAssemblyName();
            placeholder.ReferenceId = CmsService.Instance.GetItem<Entity>().Id.ToString();
            placeholder.Key = Key;
            Controls.Add(placeholder);
        }

        //protected override void CreateChildControls()
        //{
        //    Controls.Clear();
        //    CreateControlHierarchy();
        //    ClearChildViewState();
        //}

        //private void CreateControlHierarchy()
        //{
            
        //}
    }
}
