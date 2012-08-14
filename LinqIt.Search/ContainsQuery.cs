using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class ContainsQuery : Query
    {
        #region Constructors

        public ContainsQuery(string fieldName, params string[] values)
        {
            FieldName = fieldName;
            Values = values;
        }

        #endregion Constructors

        #region Properties

        public string FieldName
        {
            get;
            set;
        }

        public string[] Values
        {
            get;
            set;
        }

        #endregion Properties
    }
}
