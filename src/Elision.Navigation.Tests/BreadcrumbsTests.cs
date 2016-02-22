//using System.Linq;
//using Elision.Fields;
//using Elision.Web.ModelBuilders;
//using FluentAssertions;
//using NUnit.Framework;
//using Sitecore.Data;
//using Sitecore.FakeDb;

//namespace Elision.Navigation.Tests
//{
//    [TestFixture]
//    public class BreadcrumbsTests
//    {
//        [TestCase("/sitecore/Content/Website/Home", 1)]
//        [TestCase("/sitecore/Content/Website/Home/Section", 2)]
//        [TestCase("/sitecore/Content/Website/Home/Section/SubPage", 3)]
//        public void BreadcrumbsStopAtWebsiteFolder(string itemPath, int expectedCount)
//        {
//            using (GetFakeDb())
//            {
//                var repo = new BreadcrumbsModelBuilder();

//                var thisPage = Sitecore.Context.Database.GetItem(itemPath);

//                var model = repo.BuildModelForPage(thisPage);

//                model.Crumbs.Count.Should().Be(expectedCount);
//            }
//        }

//        [TestCase("/sitecore/Content/Website/Home")]
//        [TestCase("/sitecore/Content/Website/Home/Section")]
//        [TestCase("/sitecore/Content/Website/Home/Section/SubPage")]
//        public void OnlyLastPageIsActive(string itemPath)
//        {
//            using (GetFakeDb())
//            {
//                var repo = new BreadcrumbsModelBuilder();

//                var thisPage = Sitecore.Context.Database.GetItem(itemPath);

//                var model = repo.BuildModelForPage(thisPage);

//                model.Crumbs.Where(x => x.IsActive).Should().HaveCount(1);
//                model.Crumbs.Last().IsActive.Should().BeTrue();
//            }
//        }

//        private static Db GetFakeDb()
//        {
//            return new Db {
//                new DbTemplate("WebsiteFolder", TemplateIDs.WebsiteFolder),
//                new DbItem("Website", ID.NewID, TemplateIDs.WebsiteFolder) {
//                    new DbItem("Home") {
//                        new DbField(NavigableFieldIDs.ShowInPrimaryNavigation){ Value = "1" },
//                        new DbItem("Section") {
//                            new DbField(NavigableFieldIDs.ShowInPrimaryNavigation){ Value = "1" },
//                            new DbItem("SubPage")
//                                {
//                                    new DbField(NavigableFieldIDs.ShowInPrimaryNavigation){ Value = "1" },
//                                }
//                        }}}};
//        }
//    }
//}
