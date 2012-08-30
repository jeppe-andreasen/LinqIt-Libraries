using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Cms.Data.DataIterators
{
    public abstract class DataIterator
    {
        protected DataIterator()
        {
        }

        protected internal abstract string ItemType { get; }

        protected internal abstract bool ReadNext();

        protected internal abstract void RenderCurrent(System.Xml.XmlWriter writer);

    }
}
