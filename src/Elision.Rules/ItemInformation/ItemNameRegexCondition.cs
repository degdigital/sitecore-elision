using System.Text.RegularExpressions;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.ItemInformation
{
    public class ItemNameRegexCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public string Pattern { get; set; }

        protected override bool Execute(T ruleContext)
        {
            var regex = new Regex(Pattern);
            return regex.IsMatch(ruleContext.Item.Name);
        }
    }
}