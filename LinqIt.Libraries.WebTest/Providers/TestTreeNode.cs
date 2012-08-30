using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Components.Data;

namespace LinqIt.Libraries.WebTest.Providers
{
    internal class TestTreeNode : Node
    {
        public static int _count;

        internal TestTreeNode(string name, Dictionary<string, TestTreeNode> dictionary)
        {
            _count++;
            Text = name;
            Id = _count.ToString();
            Children = new List<TestTreeNode>();
            Icon = "/assets/gfx/icons/folder.png";
            dictionary.Add(Id, this);
        }

        internal TestTreeNode Parent { get; set; }

        internal List<TestTreeNode> Children { get; private set; }

        internal TestTreeNode Add(string name, Dictionary<string, TestTreeNode> dictionary)
        {
            var result = new TestTreeNode(name, dictionary);
            Children.Add(result);
            result.Parent = this;
            return result;
        }

        internal void Add(Dictionary<string, TestTreeNode> dictionary, params string[] names)
        {
            foreach (var name in names)
            {
                var node = new TestTreeNode(name, dictionary);
                Children.Add(node);
                node.Parent = this;
            }
        }
        
    }
}