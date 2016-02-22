using Elision.Rules.GetLookupSourceItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.Rules.Tests.GetLookupSourceItems
{
    [TestClass]
    public class ReplaceTokenWithItemPathActionTests
    {
        [TestMethod]
        public void ReplacesToken()
        {
            using (var db = new Db()
                {
                    new DbItem("Home"){
                        new DbItem("page1"),
                        new DbItem("page2")
                    }
                })
            {
                var home = db.GetItem("/sitecore/content/home");
                var page1 = db.GetItem("/sitecore/content/home/page1");
                var page2 = db.GetItem("/sitecore/content/home/page2");

                var args = new GetLookupSourceItemsArgs() {Item = page1, Source = "{ThisPage}/afolder"};

                var ctx = new GetLookupsourceItemsRuleContext(args);
                var action = new ReplaceTokenWithItemPathAction<GetLookupsourceItemsRuleContext>()
                    {
                        Token = "ThisPage",
                        NewItemId = page2.ID.ToString()
                    };

                action.Apply(ctx);

                ctx.Args.Source.Should().BeEquivalentTo("/sitecore/content/home/page2/afolder");
            }
        }
    }
}
