using Sitecore.Rules;

namespace Elision.Rules.Services
{
    public class EnhancedRuleContext : RuleContext
    {
        public bool StopProcessingThisRuleset { get; set; }
        public bool StopProcessingThisRule { get; set; }
        public bool StopProcessingAfterThisRuleset { get; set; }
    }
}
