using System.Linq;
using System.Text.RegularExpressions;
using Sitecore.Collections;
using Sitecore.Rules;

namespace Elision.Rules.ItemNaming
{
    public class EnsureItemNameIsUnique<T> : RenameItemAction<T> where T : RuleContext
    {
        public override void Apply(T ruleContext)
        {
            var item = ruleContext.Item;
            var name = ruleContext.Item.Name;

            var existingItemNames = ruleContext
                .Item.Parent
                .GetChildren(ChildListOptions.SkipSorting | ChildListOptions.IgnoreSecurity)
                .Where(x => x.ID != item.ID && x.Key != item.Key)
                .Select(x => x.Name)
                .ToArray();

            var regex = new Regex(@"(?<name>.*\D)(?<num>\d+)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);

            while (existingItemNames.Contains(name))
            {
                var match = regex.Match(name);
                if (match.Success)
                    name = string.Format("{0}{1}", match.Groups["name"].Value, int.Parse(match.Groups["num"].Value) + 1);
                else
                    name = name + "1";
            }
            RenameItem(item, name);
        }
    }
}
