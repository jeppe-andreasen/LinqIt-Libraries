using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Utils.Extensions;

namespace LinqIt.Cms.Data
{
    using System.Collections;

    public class IdList : IEnumerable<Id>
    {
        private readonly List<Id> _list = new List<Id>();

        public IdList()
        {
        }

        public IdList(string value)
        {
            if (!string.IsNullOrEmpty(value))
                _list.AddRange(value.Split(',',';','|').Select(s => new Id(s)));
        }

        public IdList(IEnumerable<Id> value)
        {
            _list.AddRange(value);
        }

        public void Add(Id value)
        {
            _list.Add(value);
        }

        public void AddRange(IEnumerable<Id> value)
        {
            _list.AddRange(value);
        }

        public void Remove(Id value)
        {
            _list.Remove(value);
        }

        public void Clear()
        {
            _list.Clear();
        }

        public override string ToString()
        {
            return _list.ToSeparatedString("|");
        }

        #region IEnumerable<Id> Members

        public IEnumerator<Id> GetEnumerator()
        {
            return _list.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_list).GetEnumerator();
        }

        #endregion
    }
}
