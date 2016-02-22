using Elision.Rules.GetLookupSourceItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.Rules.Tests.GetLookupSourceItems
{
    [TestClass]
    public class SetQueryContextItemActionTests
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
                var action = new SetQueryContextItemAction<GetLookupsourceItemsRuleContext>()
                    {
                        NewContextItemId = page2.ID.ToString()
                    };

                action.Apply(ctx);

                ctx.Args.Item.ID.Should().Be(page2.ID);
            }
        }
    }
}
