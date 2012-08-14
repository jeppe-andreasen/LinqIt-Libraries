using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    #region Enumerations

    public enum BooleanQueryType
    {
        And,
        Or
    }

    #endregion Enumerations

    public class BooleanQuery : Query
    {
        #region Constructors

        public BooleanQuery(BooleanQueryType type, params Query[] queries)
        {
            this.Type = type;
            this.Queries = queries;
        }

        #endregion Constructors

        #region Properties

        public Query[] Queries
        {
            get;
            private set;
        }

        public BooleanQueryType Type
        {
            get;
            private set;
        }

        #endregion Properties

        #region Methods

        public static BooleanQuery And(params Query[] queries)
        {
            return new BooleanQuery(BooleanQueryType.And, queries);
        }

        public static BooleanQuery Or(params Query[] queries)
        {
            return new BooleanQuery(BooleanQueryType.Or, queries);
        }

        #endregion Methods
    }
}
