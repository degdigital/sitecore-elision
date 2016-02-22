using Elision.Rules.RenderingInformation;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Rules.GetRenderingDatasource
{
    public class GetRenderingDatasourceRuleContext : RenderingRuleContext
    {
        public GetRenderingDatasourceArgs Args { get; set; }

        public RulesSettings Settings { get; set; }

        public GetRenderingDatasourceRuleContext(GetRenderingDatasourceArgs args) 
            : base(args.RenderingItem.ID)
        {
            Settings = new RulesSettings();

            Args = args;
            Item = args.RenderingItem;
        }
    }
}