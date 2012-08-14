using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Ajax.Parsing
{
    public class JSONBoolean : JSONValue
    {
        // Fields
        private bool m_Value;

        // Methods
        public JSONBoolean(bool value)
        {
            this.m_Value = value;
        }

        public static new JSONBoolean Parse(Token token)
        {
            token.MoveToContent();
            if (token.Peeks("true"))
            {
                token.Skip("true");
                return new JSONBoolean(true);
            }
            if (token.Peeks("false"))
            {
                token.Skip("false");
                return new JSONBoolean(false);
            }
            return null;
        }

        public override string ToString()
        {
            return (this.m_Value ? "true" : "false");
        }

        public override object Value
        {
            get { return m_Value; }
        }
    }


}
