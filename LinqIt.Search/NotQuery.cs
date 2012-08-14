using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Search
{
    public class NotQuery : Query
    {
        #region Constructors

        public NotQuery(Query query)
        {
            this.Query = query;
        }

        #endregion Constructors

        #region Properties

        public Query Query
        {
            get;
            private set;
        }

        #endregion Properties
    }
}
