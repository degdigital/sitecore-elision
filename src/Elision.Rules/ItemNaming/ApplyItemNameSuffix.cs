using System;
using Sitecore.Rules;

namespace Elision.Rules.ItemNaming
{
    public class ApplyItemNameSuffix<T> : RenameItemAction<T> where T : RuleContext
    {
        public string Suffix { get; set; }

        public override void Apply(T ruleContext)
        {
            if (ruleContext.Item.Name.EndsWith(Suffix)) 
                return;

            var newName = String.Concat(ruleContext.Item.Name, Suffix);
            RenameItem(ruleContext.Item, newName);
        }
    }
}
