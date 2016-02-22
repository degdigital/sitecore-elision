using Elision.Fields;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.ItemInformation
{
    public class ItemThemeIsCondition<T> : WhenCondition<T> where T : RuleContext
    {
        public string ThemeId { get; set; }

        protected override bool Execute(T ruleContext)
        {
            if (string.IsNullOrWhiteSpace(ThemeId) || !ID.IsID(ThemeId))
                return false;

            var itemThemeId = ruleContext.Item.GetInheritedFieldValue(ThemableFieldNames.Theme, true);
            if (string.IsNullOrWhiteSpace(itemThemeId) || !ID.IsID(itemThemeId))
                return false;

            return ID.Parse(ThemeId) == ID.Parse(itemThemeId);
        }
    }
}
