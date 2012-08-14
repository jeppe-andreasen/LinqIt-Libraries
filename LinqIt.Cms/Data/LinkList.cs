using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace LinqIt.Cms.Data
{
    public class LinkList : IEnumerable<Link>
    {
        private readonly List<Link> _links = new List<Link>();

        public IEnumerable<Link> Links
        {
            get
            {
                return _links.AsEnumerable();
            }
        }

        public void RemoveAt(int index)
        {
            _links.RemoveAt(index);
        }

        public static LinkList Parse(string value)
        {
            return CmsService.Instance.ParseLinkList(value);
        }

        public override string ToString()
        {
            return CmsService.Instance.ConvertToString(this);
        }

        public void Clear()
        {
            _links.Clear();
        }

        public void Add(Link link)
        {
            _links.Add(link);
        }

        #region IEnumerable<Link> Members

        public IEnumerator<Link> GetEnumerator()
        {
            return _links.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((System.Collections.IEnumerable)_links).GetEnumerator();
        }

        #endregion

        public void Move(int index, int step)
        {
            int newIndex = index + step;
            var item = _links[index];
            _links.RemoveAt(index);
            _links.Insert(newIndex, item);
        }

        public void Insert(int index, Link link)
        {
            _links.Insert(index, link);
        }

        public void Remove(Link link)
        {
            _links.Remove(link);
        }

        public Link this[int index]
        {
            get
            {
                return _links[index];
            }
        }

        public void ReplaceAt(int index, Link link)
        {
            _links[index] = link;
        }

    }
}
