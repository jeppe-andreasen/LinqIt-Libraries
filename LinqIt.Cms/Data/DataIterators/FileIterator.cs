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
        private readonly SnapShotOptions _options;

        public FileIterator(SnapShotOptions options)
        {
            _options = options;
            var root = HttpContext.Current.Server.MapPath("~/");
            _items = IterationUtil.FindAllDFS(root, GetChildren, f => f != root && ShouldDeploy(root, f)).ToArray();
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

        public bool ShouldDeploy(string rootPath, string path)
        {
            if (Directory.Exists(path))
                return false;

            var info = new FileInfo(path);
            if (string.IsNullOrEmpty(info.Extension))
                return false;

            if (_options.ValidFileExtensions != null)
            {
                if (!_options.ValidFileExtensions.Contains(info.Extension.ToLower()))
                    return false;
            }

            if (_options.InvalidPaths != null)
            {
                var relativePath = "files/" + path.Substring(rootPath.Length).Replace(@"\", "/").TrimStart('/');
                if (_options.InvalidPaths.Contains(relativePath.ToLower()))
                    return false;
            }

            var lastWriteTime = info.LastWriteTime;
            return lastWriteTime >= _options.From && lastWriteTime <= _options.To;
        }

        protected internal override bool ReadNext()
        {
            if (_index < _items.Length - 1)
            {
                _index++;
                return true;
            }
            return false;
        }


        protected internal override void RenderCurrent(System.Xml.XmlWriter writer)
        {
            writer.WriteElementString("file", _items[_index]);
        }

        protected internal override string ItemType
        {
            get { return "files"; }
        }
    }
}
