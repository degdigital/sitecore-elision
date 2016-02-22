using Elision.Rules.Services;
using Sitecore.Pipelines.GetLookupSourceItems;
using Sitecore.Rules;

namespace Elision.Rules.GetLookupSourceItems
{
    public class GetLookupsourceItemsRuleContext : EnhancedRuleContext
    {
        public RulesSettings Settings;
        public GetLookupSourceItemsArgs Args { get; set; }

        public GetLookupsourceItemsRuleContext(GetLookupSourceItemsArgs args)
        {
            Settings = new RulesSettings();

            Args = args;
            Item = args.Item;
        }
    }
}