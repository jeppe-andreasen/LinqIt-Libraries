using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinqIt.Components.Data
{
    public abstract class GridItemProvider : TreeNodeProvider
    {
        protected GridItemProvider(string referenceId) : base(referenceId)
        {
            
        }

        public sealed override Node GetNode(string id)
        {
            return GetItem(id);
        }

        public abstract GridItem GetItem(string id);

        public sealed override IEnumerable<Node> GetChildNodes(Node node)
        {
            return GetChildItems((GridItem) node);
        }

        public abstract IEnumerable<GridItem> GetChildItems(GridItem gridItem);

        public sealed override IEnumerable<Node> GetRootNodes()
        {
            return GetRootItems();
        }

        public abstract IEnumerable<GridItem> GetRootItems();

        public sealed override Node GetParentNode(Node node)
        {
            return GetParentItem((GridItem) node);
        }

        public abstract GridItem GetParentItem(GridItem item);

        public abstract GridLayout GetLayout();

        public abstract GridPlaceholderData GetPlaceholderData(string key);

        public abstract IEnumerable<ModuleTemplate> GetModuleTemplates();

        public override Node CreateNode(string name, string parentId, string templateId)
        {
            return CreateItem(name, parentId, templateId);
        }

        public abstract GridItem CreateItem(string name, string parentId, string templateId);

        public abstract GridItem Detach(string moduleId);
    }
}
