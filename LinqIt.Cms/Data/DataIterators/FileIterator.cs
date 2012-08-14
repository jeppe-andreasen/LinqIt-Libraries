using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using LinqIt.Utils;

namespace LinqIt.Cms.Data.DataIterators
{
    public class FileIterator : DataIterator
    {
        private readonly string[] _items;
        private int _index = -1;

        public FileIterator(DateTime from, DateTime to) : base(from, to)
        {
            var root = HttpContext.Current.Server.MapPath("~/");
            _items = IterationUtil.FindAllDFS(root, GetChildren, f => f != root && ShouldDeploy(root, f, from, to)).ToArray();
        }

        private static IEnumerable<string> GetChildren(string path)
        {
            if (!Directory.Exists(path))
                return new string[0];
            
            var result = new List<string>();
            result.AddRange(Directory.GetDirectories(path));
            result.AddRange(Directory.GetFiles(path, "*.*"));
            return result;
        }

        public bool ShouldDeploy(string rootPath, string path, DateTime from, DateTime to)
        {
            if (Directory.Exists(path))
                return false;

            var info = new FileInfo(path);
            if (string.IsNullOrEmpty(info.Extension))
                return false;

            var validExtensions = new []{".master", ".aspx", ".ashx", ".ascx", ".css", ".js", ".png", ".jpg", ".gif", ".dll", ".pdb" };
            if (!validExtensions.Contains(info.Extension))
                return false;

            var invalidPaths = new[]
            {
                "~/obj",
                "~/App_Data"
            };

            var relativePath = "~/" + path.Substring(rootPath.Length).Replace(@"\", "/");
            if (invalidPaths.Where(relativePath.StartsWith).Any())
                return false;
                

            var lastWriteTime = info.LastWriteTime;
            return lastWriteTime >= from && lastWriteTime <= to;
        }

        internal override bool ReadNext()
        {
            if (_index < _items.Length - 1)
            {
                _index++;
                return true;
            }
            return false;
        }


        internal override void RenderCurrent(System.Xml.XmlWriter writer)
        {
            writer.WriteStartElement("file");
            writer.WriteAttributeString("path", _items[_index]);
            writer.WriteEndElement();
        }

        internal override string ItemType
        {
            get { return "files"; }
        }
    }
}
