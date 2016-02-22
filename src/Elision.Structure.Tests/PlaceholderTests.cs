//using System;
//using NUnit.Framework;
//using FluentAssertions;
//using Sitecore.Data;
//using Sitecore.FakeDb;
//using Sitecore.FakeDb.Serialization;

//namespace Elision.Structure.Tests
//{
//    [TestFixture]
//    [Ignore("There is something wrong with FakeDb deserializing the placeholders. It doesn't seem to get the actual field values correctly.")]
//    public class PlaceholderTests
//    {
//        [TestCase(PlaceholderIDs.PageHeadIdString, "pagehead")]
//        [TestCase(PlaceholderIDs.PageFootIdString, "pagefoot")]
//        [TestCase(PlaceholderIDs.PageBodyIdString, "pagebody")]
//        public void PlaceholderKeySetCorrectly(string placeholderId, string key)
//        {
//            using (BuildFakeDb())
//            {
//                Sitecore.Context.Database.GetItem(new ID(placeholderId))
//                        .Fields["Placeholder Key"].Value.Should().Be(key);
//            }
//        }

//        [TestCase("PageBody", PlaceholderIDs.PageBodyIdString, new[]
//            {
//                RenderingIDs.OneColumnPageLayoutIdString,
//                RenderingIDs.TwoColumnEqualPageLayoutIdString,
//                RenderingIDs.TwoColumnWideLeftPageLayoutIdString,
//                RenderingIDs.TwoColumnWideRightPageLayoutIdString,
//                RenderingIDs.ThreeColumnEqualPageLayoutIdString,
//                RenderingIDs.ThreeColumnWideMiddlePageLayoutIdString,
//                RenderingIDs.FourColumnEqualPageLayoutIdString,
//                RenderingIDs.NavigationBreadcrumbsIdString
//            })]
//        [TestCase("PageHead", PlaceholderIDs.PageHeadIdString)]
//        [TestCase("PageFoot", PlaceholderIDs.PageFootIdString)]
//        public void PlaceholderAllowesCorrectRenderings(string placeholderName, string placeholderId, params string[] allowedControls)
//        {
//            using (BuildFakeDb())
//            {
//                var placeholderItem = Sitecore.Context.Database.GetItem(ID.Parse(placeholderId));

//                placeholderItem.Fields["Allowed Controls"]
//                    .Value.Split(new[] {'|'}, StringSplitOptions.RemoveEmptyEntries)
//                    .Should().BeEquivalentTo(allowedControls);
//            }
//        }

//        private static Db BuildFakeDb()
//        {
//            return new Db()
//                {
//                    new DsDbItem("/sitecore/layout", true) {ParentID = Sitecore.ItemIDs.RootID}
//                };
//        }
//    }
//}
