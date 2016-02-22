using Sitecore.Rules.Conditions;

namespace Elision.Rules.PlaceholderSettings
{
    public class PlaceholderKeyPathCondition<T> : StringOperatorCondition<T> where T : PlaceholderSettingsRuleContext
    {
        public string Value { get; set; }

        protected override bool Execute(T ruleContext)
        {
            return Compare(ruleContext.PlaceholderKeyPath, Value);
        }
    }
}