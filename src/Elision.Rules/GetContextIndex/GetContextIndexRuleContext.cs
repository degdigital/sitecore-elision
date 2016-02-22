using Elision.Rules.Services;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Pipelines.GetContextIndex;

namespace Elision.Rules.GetContextIndex
{
    public class GetContextIndexRuleContext : EnhancedRuleContext
    {
        public readonly GetContextIndexArgs Args;

        public GetContextIndexRuleContext(GetContextIndexArgs args)
        {
            Args = args;
            Item = (SitecoreIndexableItem) args.Indexable;
        }
    }
}
