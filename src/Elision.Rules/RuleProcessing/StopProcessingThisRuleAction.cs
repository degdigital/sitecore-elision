using Elision.Rules.Services;
using Sitecore.Rules.Actions;

namespace Elision.Rules.RuleProcessing
{
    public class StopProcessingThisRuleAction<T> : RuleAction<T> where T : EnhancedRuleContext
    {
        public override void Apply(T ruleContext)
        {
            ruleContext.StopProcessingThisRule = true;
        }
    }
}
