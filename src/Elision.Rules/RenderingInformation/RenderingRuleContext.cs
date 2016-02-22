using Elision.Rules.Services;
using Sitecore.Data;

namespace Elision.Rules.RenderingInformation
{
    public class RenderingRuleContext : EnhancedRuleContext
    {
        public readonly ID RenderingId;

        public RenderingRuleContext(ID renderingId)
        {
            RenderingId = renderingId;
        }
    }
}
