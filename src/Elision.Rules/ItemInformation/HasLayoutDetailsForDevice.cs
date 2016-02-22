using System.Linq;
using Sitecore.Data;
using Sitecore.Rules;
using Sitecore.Rules.Conditions;

namespace Elision.Rules.ItemInformation
{
    public class HasLayoutDetailsForDevice<T> : OperatorCondition<T> where T : RuleContext
    {
        public string Device { get; set; }

        protected override bool Execute(T ruleContext)
        {
            return ruleContext
                .Item.Database.Resources
                .Devices.GetAll()
                .Where(x => ID.IsID(Device) ? x.ID.ToString() == Device : x.Name == Device)
                .Any(device => ruleContext.Item.Visualization.GetLayout(device) != null);
        }
    }
}
