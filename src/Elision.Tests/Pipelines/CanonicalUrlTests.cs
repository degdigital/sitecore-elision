using Elision.Web.Pipelines.GetCanonicalUrl;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines;

namespace Elision.Tests.Pipelines
{
    [TestClass]
    public class CanonicalUrlTests
    {
        [TestMethod]
        public void LowerCasesRawUrl()
        {
            var rawUrl = "/Page Url";
            var expectedResult = "/page url";

            using (GetFakeDb())
            {
                var args = new GetCanonicalUrlArgs(null, rawUrl);

                CorePipeline.Run("getCanonicalUrl", args);

                args.CanonicalUrl.Should().Be(expectedResult);
            }
        }

        [TestMethod]
        public void GetsCorrectAliasedCanonical()
        {
            var rawUrl = "/aliased";
            var resolvedUrl = "/page";

            using (GetFakeDb())
            {
                var args = new GetCanonicalUrlArgs(null, rawUrl);

                CorePipeline.Run("getCanonicalUrl", args);

                args.CanonicalUrl.Replace(".aspx", "").Should().EndWith(resolvedUrl);
            }
        }

        [TestMethod]
        public void EmptyForUnchangedUrls()
        {
            var rawUrl = "/page-url";

            using (GetFakeDb())
            {
                var args = new GetCanonicalUrlArgs(null, rawUrl);

                CorePipeline.Run("getCanonicalUrl", args);

                args.CanonicalUrl.Should().BeNull();
            }
        }

        private static Db GetFakeDb()
        {
            var pageId = new ID();
            var pageItem = new DbItem("page", pageId);
            var aliases = new DbItem("Aliases")
                {
                    new DbItem("aliased")
                        {
                            new DbLinkField("Linked item") { LinkType = "internal", TargetID = pageId }
                        }
                };
            aliases.ParentID = Sitecore.ItemIDs.SystemRoot;

            var db = new Db()
                {
                    aliases,
                    pageItem
                };
            return db;
        }
    }
}
