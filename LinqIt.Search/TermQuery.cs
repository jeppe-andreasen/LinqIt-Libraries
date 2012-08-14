using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class TermQuery : Query
    {
        #region Constructors

        public TermQuery(string fieldName, string value)
        {
            FieldName = fieldName;
            Value = value;
        }

        #endregion Constructors

        #region Properties

        public string FieldName
        {
            get;
            private set;
        }

        public string Value
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
