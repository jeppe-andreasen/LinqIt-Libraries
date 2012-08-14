using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax.Parsing
{
    public class JSONDelegate : JSONValue
    {
        private string[] _parameters;
        private readonly List<string> _lines;

        public JSONDelegate(params string[] parameters)
        {
            _parameters = parameters;
            _lines = new List<string>();
        }

        public List<string> Lines { get { return _lines; } }

        public override object Value
        {
            get { return this.ToString(); }
        }

        public override string ToString()
        {
            var result = new StringBuilder();
            result.AppendLine("function(" + _parameters.ToSeparatedString(",") + ") {");
            foreach (var line in _lines)
                result.AppendLine(line);
            result.AppendLine("}");
            return result.ToString();
        }
    }
}
