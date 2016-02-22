using Sitecore.Diagnostics;
using Sitecore.Shell.Framework.Pipelines;
using System.Collections.Generic;
using System.Linq;

namespace Elision.ContentEditor.UpdateReferences
{
    public class CopyOrCloneItem : Sitecore.Shell.Framework.Pipelines.CopyItems
    {
        public virtual void ProcessFieldValues(CopyItemsArgs args)
        {
            var sourceRoot = GetItems(args).FirstOrDefault();
            Assert.IsNotNull(sourceRoot, "sourceRoot is null.");

            var targetItem = args.Copies.FirstOrDefault();
            Assert.IsNotNull(targetItem, "targetItem is null.");

            var roots = new Dictionary<string, string>
                {
                    {sourceRoot.Paths.Path, targetItem.Paths.Path}
                };

            if (roots.Count <= 0)
                return;

            var refUpdater = new ReferenceUpdater(targetItem, roots, true);
            refUpdater.Start();
        }
    }
}
