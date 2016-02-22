using Elision.Search.Tests.Fixtures;
using FluentAssertions;
using Sitecore.ContentSearch;
using Sitecore.FakeDb;
using Sitecore.ContentSearch.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Search.Tests
{
    [TestClass]
    public class ContentSearchTests
    {
        [TestMethod]
        [Ignore] // there's a new pipeline in contentsearch in 8.1 that seems to be interfering with this unit test
        public void CanSearchTestIndex()
        {
            using (var db = new Db
                {
                    new DbItem("Home")
                        {
                            new DbItem("Page")
                        }
                })
            {
                var items = Sitecore.Context.Database.SelectItems("//*");

                var index = TestIndexBuilder.CreateIndex(items);

                using (var ctx = index.CreateSearchContext())
                {
                    var queryable = ctx.GetQueryable<ISearchResult>();
                    var results = queryable.GetResults();

                    results.TotalSearchResults.Should().BeGreaterThan(0);
                }

            }
        }
    }
}
