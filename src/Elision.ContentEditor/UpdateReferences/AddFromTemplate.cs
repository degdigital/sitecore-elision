using Sitecore.Data.Items;
using Sitecore.Events;
using System;
using System.Collections.Generic;

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

            var refUpdater = new ReferenceUpdater(targetItem, roots, true);
            refUpdater.Start();
        }
    }
}
