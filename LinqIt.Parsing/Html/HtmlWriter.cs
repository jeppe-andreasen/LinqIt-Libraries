using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Parsing.Html
{
    internal class HtmlWriter
    {
        private readonly StringBuilder _builder;
        private int _increment;

        internal HtmlWriter()
        {
            _builder = new StringBuilder();
            _increment = 0;
        }

        public override string ToString()
        {
            return _builder.ToString();
        }

        internal void Write(string text)
        {
            if (_builder.Length > 0 && _builder[_builder.Length - 1] == '\n')
            {
                for (var i = 0; i < _increment; i++)
                    _builder.Append('\t');
            }
            _builder.Append(text);
        }

        internal void WriteFormat(string format, params object[] parameters)
        {
            Write(string.Format(format, parameters));
        }

        internal void Increment()
        {
            _increment++;
        }

        internal void Decrement()
        {
            _increment--;
        }

        internal void NewLine()
        {
            if (_builder.Length == 0 || _builder[_builder.Length - 1] != '\n')
                _builder.Append("\r\n");
        }
    }
}
