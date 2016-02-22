using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.ItemInformation
{
    public class ItemInheritsFromCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public string TemplateId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrWhiteSpace(TemplateId) || !ID.IsID(TemplateId))
                return false;

            return ruleContext.Item.InheritsFrom(ID.Parse(TemplateId));
        }
    }
}
