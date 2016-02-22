using Elision.Diagnostics;
using Elision.Rules.GetContextIndex;
using Elision.Rules.Services;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Pipelines.GetContextIndex;
using Sitecore.Data.Items;

namespace Elision.Search.Pipelines.GetContextIndex
{
    public class RunGetContextIndexRules : GetContextIndexProcessor
    {
        private readonly IRulesRunner _runner;

        public RunGetContextIndexRules(IRulesRunner runner)
        {
            _runner = runner;
        }

        public override void Process(GetContextIndexArgs args)
        {
            using (new TraceOperation("Run GetContextIndex rules"))
            {
                var indexItem = (Item)(SitecoreIndexableItem) args.Indexable;
                _runner.RunGlobalRules(RuleIDs.GetContextIndexGlobalRulesRootId,
                                       indexItem == null ? Sitecore.Context.Database : indexItem.Database,
                                       new GetContextIndexRuleContext(args));
            }
        }
    }
}