using Sitecore.Rules.Conditions;

namespace Elision.Rules.GetLookupSourceItems
{
    public class QueryContainsTokenCondition<T> : WhenCondition<T> where T : GetLookupsourceItemsRuleContext
    {
        public string Token { get; set; }

        protected override bool Execute(T ruleContext)
        {
            return ruleContext.Args.Source
                              .Contains(
                                  string.Concat(
                                      ruleContext.Settings.QueryTokenPrefix,
                                      Token,
                                      ruleContext.Settings.QueryTokenSuffix
                                      ));
        }
    }
}