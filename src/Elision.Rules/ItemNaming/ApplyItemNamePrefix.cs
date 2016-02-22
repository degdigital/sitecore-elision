using System;
using Sitecore.Rules;

namespace Elision.Rules.ItemNaming
{
    public class ApplyItemNamePrefix<T> : RenameItemAction<T> where T : RuleContext
    {
        public string Prefix { get; set; }

        public override void Apply(T ruleContext)
        {
            if (ruleContext.Item.Name.StartsWith(Prefix)) 
                return;

            var newName = String.Concat(Prefix, ruleContext.Item.Name);
            RenameItem(ruleContext.Item, newName);
        }
    }
}
