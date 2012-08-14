using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components.Data;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class AccordionEditorProvider : TreeNodeProvider
    {
        public AccordionEditorProvider(string referenceId) : base(referenceId)
        {
            
        }

        public override IEnumerable<Node> GetRootNodes()
        {
            var data = GetData();
            return new Node[] {data};
        }

        private AccordionData GetData()
        {
            return (AccordionData)HttpContext.Current.Session["AccordionData_" + _referenceId];
        }

        public override IEnumerable<Node> GetChildNodes(Node node)
        {
            var data = GetData();
            var item = data.GetItem(node.Id);
            return item.Items;
        }

        public override Node GetNode(string value)
        {
            var data = GetData();
            return data.GetItem(value);
        }

        public override Node GetParentNode(Node node)
        {
            var data = GetData();
            var item = data.GetItem(node.Id);
            return item.Parent;
        }
    }
}
