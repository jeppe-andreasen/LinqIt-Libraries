using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI.WebControls;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components;
using LinqIt.Utils.Extensions;
using Page = System.Web.UI.Page;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class GridEditorHandler : Page
    {
        protected PlaceHolder plhContent;

        protected override void CreateChildControls()
        {
            base.CreateChildControls();
            using (CmsContext.Editing)
            {
                var itemId = new Id(Request.QueryString["itemid"]);
                var page = CmsService.Instance.GetItem<Entity>(itemId);
                var grideditor = new LinqItGridEditor();
                grideditor.GridItemProvider = typeof (UmbracoTreeModuleProvider).GetShortAssemblyName();
                grideditor.ItemId = itemId.ToString();
                grideditor.Value = page["grid"];
                grideditor.Frame = Request.QueryString["frame"];
                grideditor.HiddenId = Request.QueryString["hiddenId"];
                plhContent.Controls.Add(grideditor);
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            //Page.ClientScript.RegisterStartupScript(GetType(), "grideditorinitialization", "grideditor.editModule = function(moduleId){top.openContent(moduleId);};", true);
        }
    }
}
