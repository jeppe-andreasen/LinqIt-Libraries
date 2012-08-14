using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Data.DataIterators
{
    public abstract class DataIterator
    {
        protected readonly DateTime _from;
        protected readonly DateTime _to;
        

        protected DataIterator(DateTime from, DateTime to)
        {
            _from = from;
            _to = to;
        }

        internal abstract string ItemType { get; }

        internal abstract bool ReadNext();

        internal abstract void RenderCurrent(System.Xml.XmlWriter writer);

    }
}
