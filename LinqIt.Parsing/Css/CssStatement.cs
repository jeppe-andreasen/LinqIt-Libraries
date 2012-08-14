using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Parsing.Css
{
    public class CssStatement
    {
        private readonly string _property;
        private readonly string _value;

        internal CssStatement(string data)
        {
            var index = data.IndexOf(':');
            _property = data.Substring(0, index);
            _value = data.Substring(index + 1);
        }

        public override string ToString()
        {
            return _property + ":" + _value + ";";
        }
    }
}
