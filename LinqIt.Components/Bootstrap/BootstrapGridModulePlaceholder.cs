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

            foreach (var row in gridhelper.Rows)
            {
                Controls.Add(new LiteralControl("<div class=\"row\">"));
                foreach (var item in row.Cells)
                {
                    Controls.Add(new LiteralControl("<div class=\"module span" + item.ColumnSpan + "\">"));

                    var renderingDefinition = GridModuleResolver.Instance.GetRenderingDefinition(item.ModuleType);
                    if (renderingDefinition == null)
                    {
                        Controls.Add(new LiteralControl("Could not resolve module implementation on module type [" + item.ModuleType + "]"));
                    }
                    else
                    {
                        switch (renderingDefinition.RenderingType)
                        {
                            case GridModuleRenderingType.Usercontrol:
                                InstantiateUserControl(renderingDefinition, item);
                                break;
                            case GridModuleRenderingType.Control:
                                InstantiateControl(renderingDefinition, item);
                                break;
                        }
                    }
                    Controls.Add(new LiteralControl("</div>"));
                }
                Controls.Add(new LiteralControl("</div>"));
            }
        }

        private void InstantiateControl(GridModuleRenderingDefinition renderingDefinition, GridItem item)
        {
            var control = (Control) Activator.CreateInstance(renderingDefinition.Type);
            var rendering = (IGridModuleRendering) control;
            rendering.InitializeModule(item.Id, item.ColumnSpan);
            Controls.Add(control);
        }

        private void InstantiateUserControl(GridModuleRenderingDefinition definition, GridItem item)
        {
            var control = this.Page.LoadControl(definition.Path);
            if (control == null)
            {
                Controls.Add(new LiteralControl("Could not load control : " + definition.Path));
            }
            else
            {
                var rendering = control as IGridModuleRendering;
                if (rendering == null)
                {
                    Controls.Add(new LiteralControl("The Control '" + definition.Path + "' does not implement IGridModuleRendering"));
                }
                else
                {
                    rendering.InitializeModule(item.Id, item.ColumnSpan);
                    Controls.Add((Control)rendering);
                }
            }
        }
    }
}
