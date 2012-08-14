using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using LinqIt.Components.Data;
using LinqIt.Utils.Grids;
using LinqIt.Utils.Web;

namespace LinqIt.Components.Bootstrap
{
    public class BootstrapGridModulePlaceholder : Control
    {
        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string Key { get { return (string)ViewState["Key"] ?? string.Empty; } set { ViewState["Key"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string Provider { get { return (string)ViewState["Provider"] ?? string.Empty; } set { ViewState["Provider"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string ReferenceId { get { return (string)ViewState["ReferenceId"] ?? string.Empty; } set { ViewState["ReferenceId"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string CssClass { get { return (string)ViewState["CssClass"] ?? string.Empty; } set { ViewState["CssClass"] = value; } }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            var provider = ProviderHelper.GetGridItemProvider(Provider, ReferenceId);
            var data = provider.GetPlaceholderData(Key);
            if (data == null)
                return;

            var gridhelper = new GridHelper<GridItem>(data.Span, data.Items, i => i.ColumnSpan);
            if (!gridhelper.Rows.Any())
                return;

            string cssClass = "module-container";
            if (!string.IsNullOrEmpty(CssClass))
                cssClass += " " + CssClass;

            Controls.Add(new LiteralControl("<div class=\"" + cssClass + "\">"));

            foreach (var row in gridhelper.Rows)
            {
                Controls.Add(new LiteralControl("<div class=\"row\">"));
                foreach (var item in row.Cells)
                {
                    Controls.Add(new LiteralControl("<div class=\"module span" + item.ColumnSpan + "\">"));

                    var renderingPath = provider.GetRenderingPath(item.Id);
                    var control = this.Page.LoadControl(renderingPath);
                    if (control == null)
                    {
                        Controls.Add(new LiteralControl("Could not load control : " + renderingPath));
                    }
                    else
                    {
                        var rendering = control as IGridModuleRendering;
                        if (rendering == null)
                        {
                            Controls.Add(new LiteralControl("The Control '" + renderingPath + "' does not implement IGridModuleRendering"));
                        }
                        else
                        {
                            rendering.InitializeModule(item.Id, item.ColumnSpan);
                            Controls.Add((Control)rendering);
                        }
                    }
                    Controls.Add(new LiteralControl("</div>"));
                }
                Controls.Add(new LiteralControl("</div>"));
            }
            Controls.Add(new LiteralControl("</div>"));
        }
    }
}
