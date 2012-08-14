using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinqIt.Cms;
using LinqIt.Cms.Data;

namespace LinqIt.UmbracoCustomFieldTypes
{
    public class ModuleTreeNodeProvider : EntityTreeNodeProvider
    {
        public ModuleTreeNodeProvider(string referenceId) : base(referenceId)
        {
        }

        public override IEnumerable<LinqIt.Components.Data.Node> GetRootNodes()
        {
            using (CmsContext.Editing)
            {
                var currentItem = CmsService.Instance.GetItem<Entity>(new Id(_referenceId));
                var folderPath = CmsService.Instance.GetSystemPath("SubjectFolder", currentItem.Path);
                var folder = CmsService.Instance.GetItem<Entity>(folderPath);
                return new [] { GetNode(folder) };
            }
        }
    }
}
