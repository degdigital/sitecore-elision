using Elision.ContentEditor.Pipelines.GetLookupSourceItems;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.Tests.Pipelines
{
    [TestClass]
    public class ReplaceLookupSourceQueryTokensTests
    {
        [TestMethod]
        public void ReplacesWebsiteToken()
        {
            using (var db = GetDb())
            {
                var page = db.GetItem("/sitecore/content/Website/Home/Page1");
                var processor = new ReplaceLookupSourceQueryTokens();

                var args = new GetLookupSourceItemsArgs
                    {
                        Item = page,
                        Source = "{Website}/Home/Page1"
                    };
                processor.Process(args);

                args.Source.Should().Be("/sitecore/content/Website/Home/Page1");
            }
        }

        [TestMethod]
        public void ReplacesHomeToken()
        {
            using (var db = GetDb())
            {
                var page = db.GetItem("/sitecore/content/Website/Home/Page1");
                var processor = new ReplaceLookupSourceQueryTokens();

                var args = new GetLookupSourceItemsArgs
                {
                    Item = page,
                    Source = "{Home}/Page1"
                };
                processor.Process(args);

                args.Source.Should().Be("/sitecore/content/Website/Home/Page1");
            }
        }

        [TestMethod]
        public void ReplacesItemFieldValueToken()
        {
            using (var db = GetDb())
            {
                var page = db.GetItem("/sitecore/content/Website/Home/Page1");
                var processor = new ReplaceLookupSourceQueryTokens();

                page["FieldName"].Should().BeEquivalentTo("FieldValue");

                var args = new GetLookupSourceItemsArgs
                {
                    Item = page,
                    Source = "{ItemField:FieldName}/Page1"
                };
                processor.Process(args);

                args.Source.Should().Be("FieldValue/Page1");
            }
        }

        private static Db GetDb()
        {
            return new Db()
                {
                    new DbTemplate("Website", TemplateIDs.WebsiteFolder),
                    new DbTemplate("HomePage", TemplateIDs.HomePageTemplate),
                    new DbItem("Website", ID.NewID, TemplateIDs.WebsiteFolder)
                        {
                            new DbItem("Home", ID.NewID, TemplateIDs.HomePageTemplate)
                                {
                                    new DbItem("Page1") {{"FieldName", "FieldValue"}}
                                }
                        }
                };
        }
    }
}
