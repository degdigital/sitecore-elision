using System.Linq;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.ItemInformation
{
    public class HasLayoutDetailsForAnyDevice<T> : OperatorCondition<T> where T : RuleContext
    {
        protected override bool Execute(T ruleContext)
        {
            return ruleContext
                .Item.Database.Resources
                .Devices.GetAll()
                .Any(device => ruleContext.Item.Visualization.GetLayout(device) != null);
        }
    }
}
