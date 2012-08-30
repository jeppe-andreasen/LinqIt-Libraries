using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using LinqIt.Ajax.Parsing;
using LinqIt.Utils.Web;

namespace LinqIt.Components.Data
{
    public class ContextMenu
    {
        private readonly List<ContextMenuItem> _items = new List<ContextMenuItem>();

        public JSONObject ToJson()
        {
            var result = new JSONObject();
            foreach (var item in _items)
            {
                result.AddValue(item.CommandName, item.ToJson());
            }
            return result;
        }

        public List<ContextMenuItem> Items { get { return _items; } }

        public override string ToString()
        {
            return ToJson().ToString();
        }

        public void AddSpacer()
        {
            _items.Add(new ContextMenuSpacerItem(_items.Count));
        }
    }

    public class ContextMenuItem
    {
        private readonly List<ContextMenuItem> _items = new List<ContextMenuItem>();

        public ContextMenuItem(string text, string commandName, string icon)
        {
            Text = text;
            CommandName = commandName;
            Icon = icon;
        }

        public string Text { get; set; }

        public string Icon { get; set; }

        public virtual JSONValue ToJson()
        {
            var result = new JSONObject();
            result.AddValue("name", Text);
            if (!string.IsNullOrEmpty(Icon))
                result.AddValue("icon", Icon);
            if (_items.Any())
            {
                var items = new JSONObject();
                foreach (var item in _items)
                    items.AddValue(item.CommandName, item.ToJson());
                result.AddValue("items", items);
            }
            return result;
        }

        public List<ContextMenuItem> Items { get { return _items; } }

        internal void WriteTo(HtmlWriter writer)
        {
            writer.AddClass(_items.Any()? "p" : "c");
            writer.RenderBeginTag(HtmlTextWriterTag.Li);
            writer.Write(Text);
            if (_items.Any())
            {
                writer.RenderBeginTag(HtmlTextWriterTag.Ul);
                foreach (var item in _items)
                    item.WriteTo(writer);
                writer.RenderEndTag();
            }
            writer.RenderEndTag();
        }

        public string CommandName { get; set; }

        public void AddSpacer()
        {
            _items.Add(new ContextMenuSpacerItem(_items.Count));
        }
    }

    public class ContextMenuSpacerItem : ContextMenuItem
    {
        internal ContextMenuSpacerItem(int n) : base("---------", "s" + n, null)
        {
        }

        public override JSONValue ToJson()
        {
            return new JSONString(Text);
        }
    }
}
