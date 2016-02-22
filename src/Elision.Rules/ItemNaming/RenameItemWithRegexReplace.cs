using System.Text.RegularExpressions;
using Sitecore.Rules;

namespace Elision.Rules.ItemNaming
{
    public class RenameItemWithRegexReplace<T> : RenameItemAction<T> where T : RuleContext
    {
        public string Pattern { get; set; }
        public string Replacement { get; set; }

        public override void Apply(T ruleContext)
        {
            var item = ruleContext.Item;

            var regex = new Regex(Pattern);
            var newName = regex.Replace(item.Name, Replacement);

            RenameItem(item, newName);
        }
    }
}
