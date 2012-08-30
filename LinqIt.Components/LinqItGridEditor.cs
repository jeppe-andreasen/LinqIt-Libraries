using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using LinqIt.Ajax;
using LinqIt.Ajax.Parsing;
using LinqIt.Components.Data;
using LinqIt.Components.Utilities;
using LinqIt.Utils.Extensions;
using LinqIt.Utils.Grids;
using LinqIt.Utils.Web;

namespace LinqIt.Components
{
    public class LinqItGridEditor : Control, INamingContainer
    {
        private const int _moduleHeight = 60;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            ScriptUtility.RegisterEmbeddedCss(typeof(LinqItGridEditor), this, "LinqIt.Components.grideditor.css");
            ScriptUtility.RegisterEmbeddedJs(typeof(LinqItGridEditor), this, "LinqIt.Components.grideditor.js");
            AjaxUtil.RegisterAjaxMethods(this);
        }

        protected override void CreateChildControls()
        {
            Controls.Clear();
            CreateControlHierarchy();
            ClearChildViewState();
        }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string GridItemProvider { get { return (string)ViewState["Provider"] ?? string.Empty; } set { ViewState["Provider"] = value; } }

        [Bindable(true), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public virtual string SelectedValue { get { return (string)ViewState["SelectedValue"] ?? string.Empty; } set { ViewState["SelectedValue"] = value; } }

        [Bindable(false)]
        public virtual string Frame { get { return (string)ViewState["Frame"] ?? string.Empty; } set { ViewState["Frame"] = value; } }

        [Bindable(false)]
        public virtual string HiddenId { get { return (string)ViewState["HiddenId"] ?? string.Empty; } set { ViewState["HiddenId"] = value; } }

        public string Value { get; set; }

        public string Layout { get; set; }

        [Bindable(false), Category("Configuration"), DefaultValue(""), Description(""), Localizable(false)]
        public string ItemId { get { return (string)ViewState["ItemId"] ?? string.Empty; } set { ViewState["ItemId"] = value; } }

        protected virtual void CreateControlHierarchy()
        {
            var treeView = new LinqItTreeView();
            treeView.Provider = GridItemProvider;
            treeView.ItemId = ItemId;
            treeView.ProviderReferenceId = ItemId;
            //treeView.SelectedValue = SelectedValue;
            Controls.Add(treeView);
        }

        protected override void OnPreRender(EventArgs e)
        {
            if (!string.IsNullOrEmpty(Frame))
            {
                Page.ClientScript.RegisterStartupScript(GetType(), "iframeinitialization", @"
    var gridEditorFrame = $('#" + Frame + @"', window.parent.document);
    var gridEditorFrameWidth = 0;
    var gridEditorFrameHeight = 0;
        function adjustFrameHeight() {
            var innerDoc = (gridEditorFrame.get(0).contentDocument) ? gridEditorFrame.get(0).contentDocument : gridEditorFrame.get(0).contentWindow.document;
            gridEditorFrame.height(innerDoc.body.scrollHeight);
            gridEditorFrameHeight = gridEditorFrame.height();
        }
        $(function () {
            gridEditorFrame.load(function() {
                adjustFrameHeight();
                gridEditorFrameWidth = gridEditorFrame.width();
            });
            gridEditorFrame.parent().css('float','none');
        });
", true);
            }

            base.OnPreRender(e);
        }

        protected override void Render(HtmlTextWriter w)
        {
            var writer = new HtmlWriter(w);
            try
            {
                var provider = ProviderHelper.GetGridItemProvider(GridItemProvider, ItemId);
                var data = !string.IsNullOrEmpty(Value) ? GridPlaceholderData.Parse(Value, provider.GetItem) : null;
                GridLayout layout = null;
                var layoutClassName = HttpContext.Current.Request.QueryString["layoutClass"];
                if (!string.IsNullOrEmpty(layoutClassName))
                {
                    var layoutClassType = Type.GetType(layoutClassName);
                    if (layoutClassType == null)
                        throw new ApplicationException("Invalid Layout Class Type : " + layoutClassName);
                    var layoutClass = Activator.CreateInstance(layoutClassType) as IGridModuleControl;
                    if (layoutClass == null)
                        throw new ApplicationException("The layout class must implement IGridModuleControl");
                    layout = layoutClass.GetGridLayout();
                }
                else
                    layout = provider.GetLayout();

                writer.AddAttribute(HtmlTextWriterAttribute.Id, "grid-editor");
                writer.AddAttribute("data-provider", this.GridItemProvider);
                writer.AddAttribute("frame", this.Frame);
                writer.AddAttribute("hiddenId", this.HiddenId);
                writer.AddAttribute("pageId", this.ItemId);
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "clearfix");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "library-column outer-box");
                writer.RenderFullTag(HtmlTextWriterTag.Span, "Module Library");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "inner-box");

                writer.AddAttribute(HtmlTextWriterAttribute.Id, "treeview-container");
                writer.RenderBeginTag(HtmlTextWriterTag.Div);

                base.RenderChildren(w);

                writer.RenderEndTag(); // div#treeview-container
                writer.RenderEndTag(); // div.inner-box
                writer.RenderEndTag(); // div#library-column
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "grid-column outer-box");
                writer.RenderFullTag(HtmlTextWriterTag.Span, "Grid Layout");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "inner-box");


                #region Render Grid Layout


                RenderGridLayout(writer, provider, data, layout);

                #endregion



                writer.RenderEndTag(); // div.inner-box
                writer.RenderEndTag(); // div#grid-column
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "module-column outer-box");
                writer.RenderFullTag(HtmlTextWriterTag.Span, "Templates");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "inner-box");

                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (var moduleTemplate in provider.GetModuleTemplates())
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Li);
                    writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
                    writer.AddAttribute("tref", moduleTemplate.Id);
                    writer.RenderBeginTag(HtmlTextWriterTag.A, "moduleType");
                    writer.RenderImageTag(moduleTemplate.IconUrl, null, null);
                    writer.RenderFullTag(HtmlTextWriterTag.Span, moduleTemplate.Name);
                    writer.RenderEndTag(); // a.moduleType
                    writer.RenderEndTag(); // li
                }
                writer.RenderEndTag();

                writer.RenderEndTag(); // div.inner-box
                writer.RenderEndTag(); // div.module-column

                writer.AddAttribute(HtmlTextWriterAttribute.Id, "colspan-container");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "outer-box");
                writer.RenderFullTag(HtmlTextWriterTag.Span, "Column span");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "inner-box");

                writer.AddAttribute(HtmlTextWriterAttribute.Id, "coloptions");
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "clearfix");
                writer.RenderEndTag(); // div#coloptions
                writer.RenderEndTag(); // div#colspan-container
                writer.RenderEndTag();
                writer.RenderEndTag(); // div#grid-editor

                writer.AddAttribute(HtmlTextWriterAttribute.Id, "grid-dialog");
                writer.AddAttribute(HtmlTextWriterAttribute.Style, "display:none");
                writer.RenderFullTag(HtmlTextWriterTag.Div, "");    
            }
            catch(Exception e)
            {
                w.RenderBeginTag(HtmlTextWriterTag.P);
                w.Write(e.Message);
                w.RenderEndTag();
                base.Render(w);
            }
        }

        private static void RenderGridLayout(HtmlWriter writer, Data.GridItemProvider provider, Dictionary<string, GridPlaceholderData> data, GridLayout layout)
        {
            foreach (var row in layout.Rows)
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Div, "row clearfix");
                foreach (var cell in row.Cells)
                {
                    writer.RenderBeginTag(HtmlTextWriterTag.Div, "span" + cell.ColumnSpan);
                    if (!string.IsNullOrEmpty(cell.DisplayName))
                        writer.RenderFullTag(HtmlTextWriterTag.Span, cell.DisplayName, "containerlabel");

                    if (cell.Type == GridLayoutCellType.Placeholder)
                        RenderPlaceholder(writer, provider, data, cell);
                    else if (cell.Type == GridLayoutCellType.ContentBlock)
                        RenderContentBlock(writer);
                    else if (cell.Type == GridLayoutCellType.InnerGrid)
                        RenderGridLayout(writer, provider, data, (GridLayout)cell);

                    writer.RenderEndTag(); // div.span12   
                }
                writer.RenderEndTag(); // div.clearfix
            }
        }

        private static void RenderContentBlock(HtmlWriter writer)
        {
            writer.RenderBeginTag(HtmlTextWriterTag.Div, "contentblock cell");
            writer.Write("&nbsp;");
            writer.RenderEndTag();
        }

        private static void RenderPlaceholder(HtmlWriter writer, Data.GridItemProvider provider, Dictionary<string, GridPlaceholderData> data, GridCell cell)
        {
            writer.AddAttribute("cols", cell.ColumnSpan.ToString());
            writer.AddAttribute("key", cell.Key);
            writer.RenderBeginTag(HtmlTextWriterTag.Div, "dropcontainer cell");

            if (data != null && data.ContainsKey(cell.Key.ToLower()))
                RenderModules(writer, data[cell.Key.ToLower()].Items, provider);

            writer.RenderEndTag();
        }

        private static void RenderModules(HtmlWriter writer, IEnumerable<GridItem> items, GridItemProvider provider)
        {
            int left = 0;
            int idx = 0;
            foreach (var item in items)
            {
                RenderModule(writer, provider, item, left, idx);
                left += 10;
                idx++;
            }
        }

        public static void RenderModule(HtmlWriter writer, GridItemProvider provider, GridItem item)
        {
            RenderModule(writer, provider, item, null, null);
        }

        private static void RenderModule(HtmlWriter writer, GridItemProvider provider, GridItem item, int? left, int? idx)
        {
            writer.AddAttribute("ref", item.Id);

            var style = "position: absolute;";
            if (left.HasValue)
                style += "left: " + left.Value + "px; top: 0px; width: 10px;";

            writer.AddAttribute(HtmlTextWriterAttribute.Style, style);
            writer.AddAttribute("coloptions", GridModuleResolver.Instance.GetModuleColumnOptions(item.ModuleType).ToSeparatedString(","));
            writer.AddAttribute(HtmlTextWriterAttribute.Colspan, item.ColumnSpan.ToString());
            if (idx.HasValue)
                writer.AddAttribute("idx", idx.ToString());
            writer.AddClasses("module", "draggable");
            writer.AddClass(item.IsLocal ? "local" : "global");
            writer.RenderBeginTag(HtmlTextWriterTag.Div);
            writer.RenderBeginTag(HtmlTextWriterTag.H3);
            writer.AddAttribute(HtmlTextWriterAttribute.Alt, "");
            writer.AddAttribute(HtmlTextWriterAttribute.Src, item.Icon);
            writer.RenderFullTag(HtmlTextWriterTag.Img, "");
            writer.RenderFullTag(HtmlTextWriterTag.Span, item.Text);

            writer.AddAttribute(HtmlTextWriterAttribute.Title, "share");
            writer.AddClass("share");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.RenderFullTag(HtmlTextWriterTag.A, "&nbsp;");

            writer.AddAttribute(HtmlTextWriterAttribute.Title, "detach");
            writer.AddClass("detach");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.RenderFullTag(HtmlTextWriterTag.A, "&nbsp;");


            writer.AddAttribute(HtmlTextWriterAttribute.Title, "edit");
            writer.AddClass("edit");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.RenderFullTag(HtmlTextWriterTag.A, "&nbsp;");

            writer.AddAttribute(HtmlTextWriterAttribute.Title, "remove");
            writer.AddClass("remove");
            writer.AddAttribute(HtmlTextWriterAttribute.Href, "#");
            writer.RenderFullTag(HtmlTextWriterTag.A, "&nbsp;");

            writer.RenderEndTag(); // h3
            writer.RenderBeginTag(HtmlTextWriterTag.P);
            writer.RenderFullTag(HtmlTextWriterTag.Span, item.ColumnSpan.ToString(), "cols");
            writer.RenderEndTag(); // p
            writer.RenderEndTag(); // div.module draggable ui-draggable selected
        }

        [AjaxMethod(AjaxType.Sync)]
        public static JSONObject GetGridEditorItems(JSONObject request)
        {
            var placeholderWidth = request["width"];
            var placeholderColumns = Convert.ToInt32((string) request["cols"]);
            
            var gridItems = new List<GridItem>();
            var provider = ProviderHelper.GetGridItemProvider((string) request["provider"], null);
            foreach (JSONObject item in ((JSONArray) request["existing"]).Values)
            {
                var id = (string) item["id"];
                GridItem gridItem = provider.GetItem(id);
                gridItem.ColumnSpan = Convert.ToInt32((string) item["colspan"] ?? GridModuleResolver.Instance.GetModuleColumnOptions(gridItem.ModuleType).First().ToString());
                gridItem.Width = (placeholderWidth/placeholderColumns)*gridItem.ColumnSpan;
                gridItem.Position = ParsePosition((string) item["style"]);
                gridItem.MatrixPosition = new Point(gridItem.Position.X, gridItem.Position.Y/_moduleHeight);
                gridItem.Index = item["idx"];
                gridItems.Add(gridItem);
            }

            var result = new JSONObject();

            var addedItemId = (string) request["add"];
            if (!string.IsNullOrEmpty(addedItemId))
            {
                var addedItem = provider.GetItem(addedItemId);
                var columnOptions = GridModuleResolver.Instance.GetModuleColumnOptions(addedItem.ModuleType);
                addedItem.ColumnSpan = columnOptions.First();
                addedItem.Width = (placeholderWidth/placeholderColumns)*addedItem.ColumnSpan;

                decimal x = request["addX"];
                decimal y = request["addY"];

                addedItem.Position = new Point(Convert.ToInt32(x), Convert.ToInt32(y));
                addedItem.MatrixPosition = new Point(addedItem.Position.X, addedItem.Position.Y/_moduleHeight);
                gridItems.Add(addedItem);

                var add = new JSONObject();
                add.AddValue("coloptions", columnOptions.ToSeparatedString(","));
                add.AddValue("colspan", addedItem.ColumnSpan);
                add.AddValue("class", addedItem.IsLocal? "local" : "global");
                result.AddValue("add", add);
            }

            switch ((string) request["event"])
            {
                case "repositionAll":
                    gridItems.Sort((v1, v2) => v1.Index - v2.Index);
                    break;
                case "itemDropped":
                    gridItems.Sort((v1, v2) => CompareByMatrix(v1, v2, addedItemId));
                    break;
                case "repositionModule":
                    var movedItemId = (string) request["priorityItemId"];
                    gridItems.Sort((v1, v2) => CompareByMatrix(v1, v2, movedItemId));
                    break;
                default:
                    throw new ApplicationException("Event not implemented : " + (string) request["event"]);
            }

            var helper = new GridHelper<GridItem>(placeholderColumns, gridItems, g => g.ColumnSpan);
            result.AddValue("rows", helper.Rows.Count());
            var top = 0;

            var modules = new JSONArray();
            result.AddValue("modules", modules);
            int idx = 0;
            foreach (var row in helper.Rows)
            {
                var left = 0;
                foreach (var cell in row.Cells)
                {
                    var module = new JSONObject();
                    module.AddValue("id", cell.Id);
                    module.AddValue("top", top);
                    module.AddValue("left", left);
                    module.AddValue("width", cell.Width);
                    module.AddValue("idx", idx);
                    left += cell.Width;
                    idx++;
                    modules.AddValue(module);
                }
                top += _moduleHeight;
            }
            return result;
        }

        private static int CompareByMatrix(GridItem itemA, GridItem itemB, string priorityItemId)
        {
            var result = itemA.MatrixPosition.Y - itemB.MatrixPosition.Y;
            if (result == 0)
                result = itemA.MatrixPosition.X - itemB.MatrixPosition.X;
            if (result == 0)
            {
                if (itemA.Id == priorityItemId && itemB.Id != priorityItemId)
                    return -1;
                if (itemA.Id != priorityItemId && itemB.Id == priorityItemId)
                    return 1;
            }
            return result;
        }


        [AjaxMethod(AjaxType.Async)]
        public static JSONArray GetAllGridEditorItems(JSONArray requests)
        {
            var result = new JSONArray();
            foreach (JSONObject request in requests.Values)
                result.AddValue(GetGridEditorItems(request));
            return result;
        }

        [AjaxMethod(AjaxType.Sync)]
        public static string Detach(string moduleId, string providerName, string referenceId)
        {
            var provider = ProviderHelper.GetGridItemProvider(providerName, referenceId);
            var newGridItem = provider.Detach(moduleId);
            return newGridItem.Id;
        }

        private static Point ParsePosition(string p)
        {
            int left = 0;
            int top = 0;
            foreach (string part in p.Split(';').Select(n => n.Trim()))
            {
                if (string.IsNullOrEmpty(part))
                    continue;
                var parts2 = part.Split(':').Select(k => k.Trim()).ToArray();
                if (parts2.Length != 2)
                    continue;
                string label = parts2[0].ToLower();
                if (label == "left")
                    left = ParseInt(parts2[1]);
                else if (label == "top")
                {
                    top = ParseInt(parts2[1]);
                }
            }
            return new Point(left, top);
        }

        private static int ParseInt(string value)
        {
            value = value.Replace("px", "").Trim();
            var result = decimal.Parse(value, CultureInfo.InvariantCulture);
            return (int)Math.Round(result);
        }
    }
}

