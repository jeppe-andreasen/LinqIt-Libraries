using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace LinqIt.Ajax.Parsing
{
    public class JSONDateTime : JSONValue
    {
        // Fields
        private DateTime m_Value;

        // Methods
        public JSONDateTime(DateTime value)
        {
            this.m_Value = value;
        }

        public override string ToString()
        {
            TimeSpan timeSpan = (TimeSpan)(this.m_Value.ToUniversalTime() - new DateTime(1970, 1, 1));
            string result = Convert.ToInt64(timeSpan.TotalMilliseconds).ToString();
            return result;
        }

        public override object Value
        {
            get { return m_Value; }
        }
    }
}
