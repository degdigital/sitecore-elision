using Sitecore.Data.Items;
using Sitecore.Events;
using System;
using System.Collections.Generic;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Data.Events;
using Sitecore.Data.Fields;
using Sitecore.SecurityModel;

namespace Elision.ContentEditor.UpdateReferences
{
    public class AddFromTemplate
    {
        public void OnItemAdded(object sender, EventArgs args)
        {
            var targetItem = Event.ExtractParameter(args, 0) as Item;

            if (targetItem == null)
                return;

            var roots = new Dictionary<string, string>();
            if (targetItem.Branch != null && targetItem.Branch.InnerItem.Children.Count == 1)
                roots.Add(targetItem.Branch.InnerItem.Children[0].Paths.Path, targetItem.Paths.Path);

            if (roots.Count <= 0) return;

            var sourceItem = targetItem.Branch.InnerItem.Children[0];
            var cache = Sitecore.Caching.CacheManager.FindCacheByName("master[items]");
            cache.RemovePrefix(sourceItem.ID.ToString());
            cache.RemovePrefix(targetItem.ID.ToString());
            EnsureLayoutWasCopied(targetItem.Branch.InnerItem.Children[0], targetItem);
            cache.RemovePrefix(targetItem.ID.ToString());

            var refUpdater = new ReferenceUpdater(targetItem, roots, true);
            refUpdater.Start();

            cache.RemovePrefix(targetItem.ID.ToString());
        }

        private void EnsureLayoutWasCopied(Item sourceItem, Item targetItem)
        {
            var sourceLayout = sourceItem.Fields[FieldIDs.LayoutField].GetValue(false, false, false, false, true);
            var sourceFinalLayout = sourceItem.Fields[FieldIDs.FinalLayoutField].GetValue(false, false, false, false, true);

            using (new EditContext(targetItem, SecurityCheck.Disable))
            {
                targetItem[FieldIDs.LayoutField] = sourceLayout;
                targetItem[FieldIDs.FinalLayoutField] = sourceFinalLayout;
            }
        }
    }
}
