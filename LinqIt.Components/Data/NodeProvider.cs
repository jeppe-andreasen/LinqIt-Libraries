using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public abstract class NodeProvider
    {
        protected string _referenceId;

        protected NodeProvider()
        {
            
        }

        protected NodeProvider(string referenceId)
        {
            _referenceId = referenceId;
        }

        public abstract IEnumerable<Node> GetRootNodes();

        public abstract Node GetNode(string value);
    }
}
