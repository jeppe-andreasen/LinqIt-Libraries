using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinqIt.Utils.Web
{
    public static class Converter
    {
        public static T ConvertFrom<T>(object value, T defaultValue = default(T))
        {
            var converter = TypeDescriptor.GetConverter(typeof(T));
            if (converter != null)
                return (T)converter.ConvertFrom(value);
            return defaultValue;
        }
    }
}
