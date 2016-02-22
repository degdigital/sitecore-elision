using Sitecore.Rules.Actions;

namespace Elision.Rules.CompatibleRenderings
{
    public class ClearCompatibleRenderingsAction<T> : RuleAction<T> where T : GetCompatibleRenderingsRuleContext
    {
        public override void Apply(T ruleContext)
        {
            ruleContext.CompatibleRenderings.Clear();
        }
    }
}
