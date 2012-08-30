using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Components.Data;

namespace LinqIt.Libraries.WebTest.Providers
{
    public class TreeNodeProvider : LinqIt.Components.Data.TreeNodeProvider
    {
        private readonly Dictionary<string, TestTreeNode> _nodes;

        public TreeNodeProvider(string referenceId) : base(referenceId)
        {
            TestTreeNode._count = 0;

            _nodes = new Dictionary<string, TestTreeNode>();

            var knud = new TestTreeNode("Knud", _nodes);
            knud.Add(_nodes, "Louise", "Rikke", "Sofie", "Jeppe");
            var louise = knud.Children.First();
            louise.Add(_nodes, "Mathias", "Kasper", "Rasmus");
            var jeppe = knud.Children.Last();
            jeppe.Add(_nodes, "Villads", "Emma");
        }

        private Node GetNode(string id, string name)
        {
            var result = new Node();
            result.Id = id;
            result.Text = name;
            result.Icon = "/assets/gfx/icons/folder.png";
            return result;
        }

        public override IEnumerable<LinqIt.Components.Data.Node> GetChildNodes(LinqIt.Components.Data.Node node)
        {
            return ((TestTreeNode) node).Children;
        }

        public override LinqIt.Components.Data.Node GetParentNode(LinqIt.Components.Data.Node node)
        {
            return ((TestTreeNode) node).Parent;
        }

        public override IEnumerable<LinqIt.Components.Data.Node> GetRootNodes()
        {
            return _nodes.Values.Where(n => n.Text == "Knud").ToArray();
        }

        public override LinqIt.Components.Data.Node GetNode(string value)
        {
            return _nodes[value];
        }

        
    }
}