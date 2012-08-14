using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;

namespace LinqIt.Search.Configuration
{
    public abstract class GenericElementCollection<T> : ConfigurationElementCollection where T:ConfigurationElement, new()
    {
        public T this[int index]
        {
            get
            {
                return base.BaseGet(index) as T;
            }
            set
            {
                if (base.BaseGet(index) != null)
                {
                    base.BaseRemoveAt(index);
                }
                this.BaseAdd(index, value);
            }
        }

        protected override ConfigurationElement CreateNewElement()
        {
            return new T();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return GetKey(element as T);
        }

        protected abstract object GetKey(T element);
    }
}
