using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax.Parsing
{
    public class JSONNull : JSONValue
    {
        private JSONNull()
        {
            
        }

        // Methods
        public static new JSONNull Parse(Token token)
        {
            token.MoveToContent();
            if (token.Peeks("null"))
            {
                token.Skip(4);
                return JSONValue.Null;
            }
            return null;
        }

        public override string ToString()
        {
            return "null";
        }

        public override object Value
        {
            get { return null; }
        }

        internal static JSONNull Create()
        {
            return new JSONNull();
        }
    }
}
