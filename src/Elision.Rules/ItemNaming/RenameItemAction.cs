using System;
using System.Linq;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.Rules.Actions;
using Sitecore.SecurityModel;

namespace Elision.Rules.ItemNaming
{
    public abstract class RenameItemAction<T> : RuleAction<T> where T : RuleContext
    {
        protected void RenameItem(Item item, string newName)
        {
            if (item.Name == newName)
                return;

            if (IsStandardValues(item))
                return;

            if (IsHomeItemOfAnySite(item))
                return;

            using (new SecurityDisabler())
            using (new EditContext(item))
            using (new EventDisabler())
                item.Name = newName;
        }

        private static bool IsStandardValues(Item item)
        {
            return item.Template.StandardValues != null && item.ID == item.Template.StandardValues.ID;
        }

        private static bool IsHomeItemOfAnySite(Item item)
        {
            return Sitecore.Configuration.Factory
                           .GetSiteInfoList().Any(site
                                                  => item.Paths.FullPath
                                                         .Equals(site.RootPath + site.StartItem,
                                                                 StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
