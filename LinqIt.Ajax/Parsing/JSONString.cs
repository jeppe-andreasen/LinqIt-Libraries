using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LinqIt.Ajax.Parsing
{
    public class JSONString : JSONValue
    {
        // Fields
        private string m_Value;

        // Methods
        public JSONString(string value)
        {
            this.m_Value = value;
        }

        private string Enquote(string s)
        {
            if ((s == null) || (s.Length == 0))
            {
                return "\"\"";
            }
            int len = s.Length;
            StringBuilder sb = new StringBuilder(len + 4);
            sb.Append('"');
            for (int i = 0; i < len; i++)
            {
                char c = s[i];
                if (c == '\\' || c == '"')
                {
                    sb.Append('\\');
                    sb.Append(c);
                }
                else if (c == '\b')
                {
                    sb.Append(@"\b");
                }
                else if (c == '\t')
                {
                    sb.Append(@"\t");
                }
                else if (c == '\n')
                {
                    sb.Append(@"\n");
                }
                else if (c == '\f')
                {
                    sb.Append(@"\f");
                }
                else if (c == '\r')
                {
                    sb.Append(@"\r");
                }
                else if (c < ' ')
                {
                    string tmp = new string(c, 1);
                    string t = "000" + int.Parse(tmp, NumberStyles.HexNumber);
                    sb.Append(@"\u" + t.Substring(t.Length - 4));
                }
                else
                {
                    sb.Append(c);
                }
            }
            sb.Append('"');
            return sb.ToString();
        }

        public static new JSONString Parse(Token token)
        {
            StringBuilder builder = new StringBuilder();
            token.MoveToContent();
            if (!token.Peeks("\""))
            {
                return null;
            }
            token.MovePast("\"");
            while (!token.Peeks("\""))
            {
                if (token.Peeks("\\\""))
                {
                    builder.Append("\"");
                    token.Skip(2);
                }
                else
                {
                    builder.Append(token.Current);
                    token.Skip(1);
                }
            }
            token.MovePast("\"");
            return new JSONString(builder.ToString());
        }

        public override string ToString()
        {
            return this.Enquote(this.m_Value);
        }

        public override object Value
        {
            get { return m_Value; }
        }
    }


}
