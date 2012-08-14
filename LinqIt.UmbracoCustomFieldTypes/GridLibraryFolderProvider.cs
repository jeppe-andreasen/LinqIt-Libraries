using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components.Data;
using umbraco.cms.businesslogic.web;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class GridLibraryFolderProvider : TreeNodeProvider
    {
        public GridLibraryFolderProvider(string referenceId) : base(referenceId)
        {
            
        }

        public override IEnumerable<Node> GetRootNodes()
        {
            using (CmsContext.Editing)
            {
                var currentItem = CmsService.Instance.GetItem<Entity>(new Id(_referenceId));
                var rootPath = CmsService.Instance.GetSystemPath("ModuleFolder", currentItem.Path);
                var entity = CmsService.Instance.GetItem<Entity>(rootPath);
                return new[] { GetTreeNode(entity) };
            }
        }

        private static Node GetTreeNode(Entity entity)
        {
            if (entity == null)
                return null;

            var result = new Node();
            result.HelpText = entity.DisplayName;
            result.Icon = entity.Icon;
            result.Selectable = true;
            result.Id = entity.Id.ToString();
            result.Text = entity.DisplayName;
            result.Draggable = true;
            return result;
        }

        public override IEnumerable<Node> GetChildNodes(Node node)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(node.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                return entity.GetChildren<Entity>().Where(c => c.Template.Name == "GridModuleFolder").Select(GetTreeNode);
            }
        }

        public override Node GetNode(string id)
        {
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(id));
                if (entity == null)
                    return null;
                return GetTreeNode(entity);
            }
        }

        public override Node GetParentNode(Node node)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(node.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                var parent = entity.GetParent<Entity>();
                return parent == null ? null : GetTreeNode(parent);
            }
        }

        public override Node CreateNode(string name, string parentId, string templateId)
        {
            var parent = new Document(Convert.ToInt32(parentId));
            var template = new DocumentType(Convert.ToInt32(templateId));
            var author = umbraco.BusinessLogic.User.GetUser(0);
            var document = Document.MakeNew(name, template, author, parent.Id);
            umbraco.library.UpdateDocumentCache(document.Id);
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(document.Id));
                var result = GetTreeNode(entity);
                return result;
            }
        }
    }
}
