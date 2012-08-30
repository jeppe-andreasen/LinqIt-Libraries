using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Ajax.Parsing;
using LinqIt.Cms.Data;

namespace LinqIt.Components.Data
{
    public class ImageGalleryData
    {
        private readonly List<ImageGalleryItem> _items = new List<ImageGalleryItem>();

        public IList<ImageGalleryItem> Items { get { return _items; } }

        public static ImageGalleryData Parse(string value)
        {
            var result = new ImageGalleryData();
            if (!string.IsNullOrEmpty(value))
            {
                var items = JSONArray.Parse(value);
                foreach (JSONObject item in items.Values)
                    result.Items.Add(new ImageGalleryItem(item));
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
    }

    public class ImageGalleryItem : Node
    {
        public ImageGalleryItem(string name, Id imageId, string headline, string content)
        {
            this.Text = name;
            this.Id = Guid.NewGuid().ToString();
            Icon = "/umbraco/Images/umbraco/headings.png";
            Headline = headline;
            Content = content;
            ImageId = imageId.IsNull? "" : imageId.ToString();
        }

        internal ImageGalleryItem()
        {
            this.Id = Guid.NewGuid().ToString();
            Icon = "/umbraco/Images/umbraco/headings.png";
        }

        internal ImageGalleryItem(JSONObject obj)
        {
            Id = Guid.NewGuid().ToString();
            Text = (string) obj["name"];
            Headline = (string)obj["headline"];
            Icon = "/umbraco/Images/umbraco/headings.png";
            Content = (string)obj["content"];
            ImageId = (string)obj["imageId"];
        }

        public string Headline { get; set; }

        public string Content { get; set; }

        public string ImageId { get; set; }

        public JSONObject ToObject()
        {
            var result = new JSONObject();
            result.AddValue("name", Text);
            result.AddValue("headline", Headline);
            result.AddValue("content", Content);
            result.AddValue("imageId", ImageId);
            return result;
        }
    }
}
