using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LinqIt.Ajax.Parsing
{
    public class JSONNumber : JSONValue
    {
        // Fields
        private readonly decimal _decimal;
        private readonly long _int;
        private readonly bool _isDecimal;
        private static readonly char[] _validChars = new char[] { '-', '0', '1', '2', '3', '4', '5', '6', '7', '8', '9', '.' };

        // Methods
        public JSONNumber(decimal value)
        {
            _decimal = value;
            _isDecimal = true;
        }

        public JSONNumber(long value)
        {
            _int = value;
            _isDecimal = false;
        }

        private JSONNumber(string value, bool isDecimal)
        {
            if (isDecimal)
                _decimal = Convert.ToDecimal(value, CultureInfo.InvariantCulture);
            else
                _int = Convert.ToInt64(value, CultureInfo.InvariantCulture);
            _isDecimal = isDecimal;
        }

        internal string InnerValue
        {
            get { return _isDecimal ? Convert.ToString(_decimal, CultureInfo.InvariantCulture) : Convert.ToString(_int); }
        }

        public static new JSONNumber Parse(Token token)
        {
            token.MoveToContent();
            if (_validChars.Contains<char>(token.Current))
            {
                var builder = new StringBuilder();
                while (_validChars.Contains<char>(token.Current))
                {
                    builder.Append(token.Current);
                    token.Next();
                }
                var value = builder.ToString();
                return new JSONNumber(value, value.Contains('.'));
            }
            return null;
        }

        public override string ToString()
        {
            return _isDecimal ? Convert.ToString(_decimal, CultureInfo.InvariantCulture) : Convert.ToString(_int);
        }

        public override object Value
        {
            get
            {
                if (_isDecimal)
                    return _decimal;
                else
                    return _int;
            }
        }

        public bool IsDecimal { get { return _isDecimal; } }
    }


}
