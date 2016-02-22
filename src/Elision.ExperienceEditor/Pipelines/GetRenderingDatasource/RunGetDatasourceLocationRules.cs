using Elision.Diagnostics;
using Elision.Rules.GetRenderingDatasource;
using Elision.Rules.Services;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.ExperienceEditor.Pipelines.GetRenderingDatasource
{
    public class RunGetDatasourceLocationRules
    {        
        private readonly IRulesRunner _rulesRunner;
        public RunGetDatasourceLocationRules(IRulesRunner rulesRunner)
        {
            _rulesRunner = rulesRunner;
        }

        public void Process(GetRenderingDatasourceArgs args)
        {
            SetDatasourceRoots(args);
        }

        private void SetDatasourceRoots(GetRenderingDatasourceArgs args)
        {
            using (new TraceOperation("Run GetDatasourceLocation rules"))
            {
                _rulesRunner.RunGlobalRules(RuleIDs.GetRenderingDatasourceGlobalRulesRootId,
                                            args.RenderingItem.Database,
                                            new GetRenderingDatasourceRuleContext(args));
            }
        }
    }
}
