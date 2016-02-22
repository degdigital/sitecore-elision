using System;
using Elision.Diagnostics;
using Elision.Rules.GetLookupSourceItems;
using Elision.Rules.Services;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.ContentEditor.Pipelines.GetLookupSourceItems
{
    public class RunGetLookupSourceItemsRules
    {
        private readonly IRulesRunner _runner;

        public RunGetLookupSourceItemsRules(IRulesRunner runner)
        {
            _runner = runner;
        }

        public void Process(GetLookupSourceItemsArgs args)
        {
            try
            {
                using (new TraceOperation("Run GetLookupSourceItems rules."))
                {
                    _runner.RunGlobalRules(RuleIDs.GetLookupSourceItemsGlobalRulesRootId,
                                           args.Item.Database,
                                           new GetLookupsourceItemsRuleContext(args));
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message, ex, this);
            }
        }
    }
}
