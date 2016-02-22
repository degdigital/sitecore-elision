using Elision.Rules.GetLookupSourceItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetLookupSourceItems;
using Sitecore.Rules;

namespace Elision.Rules.Tests.GetLookupSourceItems
{
    [TestClass]
    public class QueryContainsTokenConditionTests
    {
        [TestMethod]
        public void MatchesToken()
        {
            using (var db = new Db() { new DbItem("page1"), new DbItem("page2") })
            {
                var page1 = db.GetItem("/sitecore/content/page1");
                var page2 = db.GetItem("/sitecore/content/page2");

                var args = new GetLookupSourceItemsArgs() { Item = page1, Source = "{Home}/page1" };

                var context = new GetLookupsourceItemsRuleContext(args);
                var condition = new QueryContainsTokenCondition<GetLookupsourceItemsRuleContext>()
                    {
                        Token = "Home"
                    };

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(true);
            }
        }
    }
}
