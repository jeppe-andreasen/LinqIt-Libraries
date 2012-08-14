using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class RangeQuery : Query
    {
        #region Constructors

        public RangeQuery(string fieldName, string fromValue, string toValue, bool inclusive)
        {
            this.FieldName = fieldName;
            this.ToValue = toValue;
            this.FromValue = fromValue;
            this.Inclusive = inclusive;
        }

        #endregion Constructors

        #region Properties

        public string FieldName
        {
            get;
            private set;
        }

        public string ToValue
        {
            get;
            private set;
        }

        public string FromValue
        {
            get;
            private set;
        }

        public bool Inclusive { get; private set; }

        #endregion Properties
    }
}
