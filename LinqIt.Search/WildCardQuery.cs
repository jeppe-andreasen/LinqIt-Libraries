using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    /// <summary>
    /// Wildcard Search Query
    /// </summary>
    public class WildCardQuery : Query
    {
        #region Constructors

        public WildCardQuery(string fieldName, string values)
        {
            FieldName = fieldName;
            this.Value = values;
        }

        #endregion Constructors

        #region Properties

        public string FieldName
        {
            get;
            set;
        }

        public string Value
        {
            get;
            set;
        }

        #endregion Properties
    }
}
