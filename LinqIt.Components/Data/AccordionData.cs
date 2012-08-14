using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Ajax.Parsing;

namespace LinqIt.Components.Data
{
    public class AccordionData : AccordionItem
    {
        private readonly Dictionary<string, AccordionItem> _dictionary;

        public AccordionData(string id, string text) : base()
        {
            Id = id;
            Text = text;
            Headline = text;
            _dictionary = new Dictionary<string, AccordionItem>();
            _dictionary.Add(id, this);
        }

        internal void Insert(AccordionItem item)
        {
            _dictionary.Add(item.Id, item);
        }

        public AccordionItem AddItem(string parentId, string headline, string content, string moduleId)
        {
            var parent = _dictionary[parentId];
            var item = new AccordionItem();
            item.Headline = headline;
            item.Text = headline;
            item.Content = content;
            item.ModuleId = moduleId;
            item.Parent = parent;
            parent.Items.Add(item);
            _dictionary.Add(item.Id, item);
            return item;
        }

        public static AccordionData Parse(string id, string text, string icon, string value)
        {
            var result = new AccordionData(id, text);
            result.Icon = icon;
            if (!string.IsNullOrEmpty(value))
            {
                JSONArray items = JSONArray.Parse(value);
                foreach (JSONObject item in items.Values)
                {
                    result.Items.Add(new AccordionItem(item, result, result));
                }
            }
            return result;
        }

        public JSONValue ToJSON()
        {
            var result = new JSONArray();
            foreach (var item in this.Items)
            {
                result.AddValue(item.ToObject());
            }
            return result;
        }

        public AccordionItem GetItem(string id)
        {
            return _dictionary[id];
        }

        public void RemoveItem(AccordionItem item)
        {
            foreach (var child in item.Items.ToArray())
                RemoveItem(child);

            item.Parent.Items.Remove(item);
            _dictionary.Remove(item.Id);
        }
    }

    public class AccordionItem : Node
    {
        public IList<AccordionItem> Items { get; private set; }

        internal AccordionItem()
        {
            this.Id = Guid.NewGuid().ToString();
            Items = new List<AccordionItem>();
            Icon = "/umbraco/Images/umbraco/headings.png";
        }

        internal AccordionItem(JSONObject obj, AccordionItem parent, AccordionData data)
        {
            Id = Guid.NewGuid().ToString();
            Headline = Text = (string)obj["text"];
            Icon = "/umbraco/Images/umbraco/headings.png";
            Content = (string)obj["content"];
            ModuleId = (string)obj["moduleId"];
            Parent = parent;
            Items = new List<AccordionItem>();
            data.Insert(this);
            foreach (JSONObject childObj in ((JSONArray)obj["items"]).Values)
                Items.Add(new AccordionItem(childObj, this, data));
        }

        public string Headline { get; set; }

        public string Content { get; set; }

        public string ModuleId { get; set; }

        public AccordionItem Parent { get; set; }

        public JSONObject ToObject()
        {
            JSONObject result = new JSONObject();
            result.AddValue("text", Headline);
            result.AddValue("content", Content);
            result.AddValue("moduleId", ModuleId);
            JSONArray items = new JSONArray();
            foreach (var child in Items)
                items.AddValue(child.ToObject());
            result.AddValue("items", items);
            return result;
        }
    }
}
