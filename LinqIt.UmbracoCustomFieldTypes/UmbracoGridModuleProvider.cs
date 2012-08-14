using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinqIt.Cms;
using LinqIt.Cms.Data;
using LinqIt.Components;
using LinqIt.Components.Data;
using LinqIt.Utils.Caching;
using umbraco;
using umbraco.cms.businesslogic.web;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class UmbracoTreeModuleProvider : GridItemProvider
    {
        public UmbracoTreeModuleProvider(string referenceId) : base(referenceId)
        {
        }

        public override int[] GetItemColumnOptions(string itemId)
        {
            var relativePath = GetRenderingPath(itemId);
            return GridModuleService.GetModuleColumnOptions(relativePath);
        }


        public override GridItem GetItem(string id)
        {
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(id));
                if (entity == null)
                    return null;
                return GetGridItem(entity);
            }
        }

        public override IEnumerable<GridItem> GetChildItems(GridItem item)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(item.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                return entity.GetChildren<Entity>().Select(GetGridItem);
            }
        }

        public override IEnumerable<GridItem> GetRootItems()
        {
            using (CmsContext.Editing)
            {
                var currentItem = CmsService.Instance.GetItem<Entity>(new Id(_referenceId));
                var rootPath = CmsService.Instance.GetSystemPath("ModuleFolder", currentItem.Path);
                var entity = CmsService.Instance.GetItem<Entity>(rootPath);
                return new[] {GetGridItem(entity)};
            }
        }

        public override GridItem GetParentItem(GridItem item)
        {
            using (CmsContext.Editing)
            {
                var id = new Id(item.Id);
                var entity = CmsService.Instance.GetItem<Entity>(id);
                var parent = entity.GetParent<Entity>();
                return parent == null ? null : GetGridItem(parent);
            }
        }

        private static GridItem GetGridItem(Entity entity)
        {
            if (entity == null)
                return null;

            var result = new GridItem();
            result.HelpText = entity.DisplayName;
            result.Icon = entity.Icon;
            //result.LocalPath = entity.Path.Substring(GetRootPath().Length);
            result.Selectable = true;
            result.Id = entity.Id.ToString();
            result.Text = entity.DisplayName;
            result.Draggable = true;
            result.IsLocal = entity.GetParent<Entity>().EntityName == "__Modules";
            return result;
        }

        public override GridLayout GetLayout()
        {
            var document = new Document(Convert.ToInt32(_referenceId));
            if (document.Template == 0)
                throw new ApplicationException("Please assign a layout template to the document");

            var template = new template(document.Template);
            return GridModuleService.GetPageGridLayout(template.MasterPageFile);
        }

        public Dictionary<string, GridPlaceholderData> GetPlaceholderData()
        {
            using (CmsContext.Editing)
            {
                return Cache.Get("GridData", CacheScope.Request, () =>
                {
                    var node = CmsService.Instance.GetItem<Entity>(new Id(_referenceId));
                    var data = node["grid"];
                    return GridPlaceholderData.Parse(data, GetItem);
                });
            }
        }

        public override GridPlaceholderData GetPlaceholderData(string key)
        {
            var gridData = GetPlaceholderData();
            return gridData.ContainsKey(key.ToLower()) ? gridData[key.ToLower()] : null;
        }

        public override string GetRenderingPath(string itemId)
        {
            using (CmsContext.Editing)
            {
                var item = CmsService.Instance.GetItem<Entity>(new Id(Convert.ToInt32(itemId)));
                return "~/modules/" + item.Template.Name + "Rendering.ascx";    
            }
        }

        public override IEnumerable<ModuleTemplate> GetModuleTemplates()
        {
            return CmsService.Instance.GetTemplates().Where(t => t.BaseTemplates.Where(b => b.Name == "GridModule").Any()).Select(GetModuleTemplate);
        }

        private static ModuleTemplate GetModuleTemplate(Template template)
        {
            var result = new ModuleTemplate();
            result.Id = template.Id.ToString();
            result.Name = template.Name.ToString();
            result.RenderingPath = "~/modules/" + result.Name + "Rendering.ascx";
            result.IconUrl = template.IconUrl;
            return result;
        }

        public override GridItem CreateItem(string name, string parentId, string templateId)
        {
            var parent = new Document(Convert.ToInt32(parentId));
            var template = new DocumentType(Convert.ToInt32(templateId));
            var author = umbraco.BusinessLogic.User.GetUser(0);

            Document modulesFolder;

            modulesFolder = GetModulesFolder(parent, author);
            var document = Document.MakeNew(name, template, author, modulesFolder.Id);
            umbraco.library.UpdateDocumentCache(document.Id);
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(document.Id));
                var result = GetGridItem(entity);
                result.IsLocal = true;
                return result;
            }
        }

        private static Document GetModulesFolder(Document parent, umbraco.BusinessLogic.User author)
        {
            Document modulesFolder;
            modulesFolder = parent.Children.Where(c => c.Text == "__Modules").FirstOrDefault();
            if (modulesFolder == null)
            {
                modulesFolder = Document.MakeNew("__Modules", DocumentType.GetByAlias("GridModuleFolder"), author, parent.Id);
                umbraco.library.UpdateDocumentCache(modulesFolder.Id);
            }
            return modulesFolder;
        }

        public override GridItem Detach(string moduleId)
        {
            var parent = new Document(Convert.ToInt32(_referenceId));
            var source = new Document(Convert.ToInt32(moduleId));
            var author = umbraco.BusinessLogic.User.GetUser(0);
            var modulesFolder = GetModulesFolder(parent, author);
            var destination = source.Copy(modulesFolder.Id, author);
            umbraco.library.UpdateDocumentCache(destination.Id);
            using (CmsContext.Editing)
            {
                var entity = CmsService.Instance.GetItem<Entity>(new Id(destination.Id));
                var result = GetGridItem(entity);
                result.IsLocal = true;
                return result;
            }
        }
    }
}
