using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components.Data;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public abstract class EntityTreeNodeProvider : TreeNodeProvider
    {
        protected EntityTreeNodeProvider(string referenceId) : base(referenceId)
        {
            
        }

        public override IEnumerable<Node> GetChildNodes(Node item)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(item.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                return entity.GetChildren<Entity>().Select(GetNode);
            }
        }

        public override Node GetParentNode(Node item)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(item.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                var parent = entity.GetParent<Entity>();
                return parent == null ? null : GetNode(parent);
            }
        }
        
        public override Node GetNode(string id)
        {
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(id));
                if (entity == null)
                    return null;
                return GetNode(entity);
            }
        }

        protected static Node GetNode(Entity entity)
        {
            if (entity == null)
                return null;

            var result = new Node();
            result.HelpText = entity.DisplayName;
            result.Icon = entity.Icon;
            result.Selectable = true;
            result.Id = entity.Id.ToString();
            result.Text = entity.DisplayName;
            return result;
        }
    }
}
