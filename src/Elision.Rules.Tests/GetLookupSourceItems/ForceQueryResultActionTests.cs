using Elision.Rules.GetLookupSourceItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.Rules.Tests.GetLookupSourceItems
{
    [TestClass]
    public class ForceQueryResultActionTests
    {
        [TestMethod]
        public void SetsContextItem()
        {
            using (var db = new Db()
                {
                    new DbItem("page1"),
                    new DbItem("page2")
                })
            {
                var page1 = db.GetItem("/sitecore/content/page1");
                var page2 = db.GetItem("/sitecore/content/page2");

                var args = new GetLookupSourceItemsArgs() {Item = page1, Source = "./*"};

                var ctx = new GetLookupsourceItemsRuleContext(args);
                var action = new ForceQueryResultAction<GetLookupsourceItemsRuleContext>()
                    {
                        ResultItemId = page2.ID.ToString()
                    };

                action.Apply(ctx);

                ctx.Args.Result.Count.Should().Be(1);
                ctx.Args.Result.ContainsID(page2.ID).Should().BeTrue();
            }
        }

        [TestMethod]
        public void AbortsPipeline()
        {
            using (var db = new Db()
                {
                    new DbItem("page1"),
                    new DbItem("page2")
                })
            {
                var page1 = db.GetItem("/sitecore/content/page1");
                var page2 = db.GetItem("/sitecore/content/page2");

                var args = new GetLookupSourceItemsArgs() {Item = page1, Source = "./*"};

                var ctx = new GetLookupsourceItemsRuleContext(args);
                var action = new ForceQueryResultAction<GetLookupsourceItemsRuleContext>()
                    {
                        ResultItemId = page2.ID.ToString()
                    };

                action.Apply(ctx);

                ctx.Args.Aborted.Should().BeTrue();
            }
        }

        [TestMethod]
        public void AbortsRuleProcessing()
        {
            using (var db = new Db()
                {
                    new DbItem("page1"),
                    new DbItem("page2")
                })
            {
                var page1 = db.GetItem("/sitecore/content/page1");
                var page2 = db.GetItem("/sitecore/content/page2");

                var args = new GetLookupSourceItemsArgs() {Item = page1, Source = "./*"};

                var ctx = new GetLookupsourceItemsRuleContext(args);
                var action = new ForceQueryResultAction<GetLookupsourceItemsRuleContext>()
                    {
                        ResultItemId = page2.ID.ToString()
                    };

                action.Apply(ctx);

                ctx.IsAborted.Should().BeTrue();
            }
        }
    }
}
