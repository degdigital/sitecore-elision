using System;
using Sitecore.Data.Items;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.GetRenderingDatasource
{
    [Obsolete("For educational purposes only. Use Sitecore.Rules.Conditions.WhenIsDescendantOrSelf,Sitecore.Kernel instead")]
    public class IsDescendentOfCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public Item PotentialParent { get; set; }
        protected override bool Execute(T ruleContext)
        {
            if (ruleContext.Item == null || PotentialParent == null)
                return false;

            return ruleContext.Item.Axes.IsDescendantOf(PotentialParent);
        }
    }
}