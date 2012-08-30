using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Ajax.Parsing;

namespace LinqIt.Components.Data
{
    public class GridPlaceholderData
    {
        private readonly List<GridItem> _items;

        public GridPlaceholderData(string key, int span)
        {
            Key = key;
            Span = span;
            _items = new List<GridItem>();
        }

        public GridPlaceholderData(string key, int span, IEnumerable<GridItem> items)
        {
            Key = key;
            Span = span;
            _items = items.ToList();
        }

        public void AddItem(GridItem item)
        {
            _items.Add(item);
        }

       
        public int Span { get; private set; }

        public string Key { get; private set; }

        public IEnumerable<GridItem> Items
        {
            get { return _items; }
        }

        public static Dictionary<string,GridPlaceholderData> Parse(string value, Func<string, GridItem> itemGetter)
        {
            if (string.IsNullOrEmpty(value))
                return new Dictionary<string, GridPlaceholderData>();
            var json = JSONArray.Parse(value);
            if (json == null)
                return new Dictionary<string, GridPlaceholderData>();

            return json.Values.Cast<JSONObject>().Select(j => GetPlaceholderData(j, itemGetter)).ToDictionary(d => d.Key.ToLower());
        }

        private static GridPlaceholderData GetPlaceholderData(JSONObject json, Func<string, GridItem> itemGetter)
        {
            var key = (string)json["key"];
            var span = json["span"];
            var items = new List<GridItem>();
            foreach (JSONObject module in ((JSONArray)json["modules"]).Values)
            {
                GridItem gridItem = itemGetter((string)module["Id"]);
                if (gridItem == null)
                    continue;
                gridItem.ColumnSpan = module["span"];
                items.Add(gridItem);
            }
            return new GridPlaceholderData(key, span, items);
        }

        public JSONObject ToJSON()
        {
            var result = new JSONObject();
            result.AddValue("key", Key);
            result.AddValue("span", Span);
            var modules = new JSONArray();
            result.AddValue("modules", modules);
            foreach (var item in Items)
            {
                var module = new JSONObject();
                module.AddValue("Id", item.Id);
                module.AddValue("span", item.ColumnSpan);
                modules.AddValue(module);
            }
            return result;
        }
    }
}
