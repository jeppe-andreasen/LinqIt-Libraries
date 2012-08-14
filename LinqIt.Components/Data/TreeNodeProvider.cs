using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web.UI;
using LinqIt.Ajax;
using LinqIt.Utils.Web;

namespace LinqIt.Components.Data
{
    public abstract class TreeNodeProvider : NodeProvider
    {
        protected TreeNodeProvider(string referenceId) : base(referenceId)
        {
        }

        public abstract IEnumerable<Node> GetChildNodes(Node node);

        public abstract Node GetParentNode(Node node);

        public virtual Node CreateNode(string name, string parentId, string templateId)
        {
            return null;
        }
    }
}
