using System;
using System.Linq;
using Elision.DynamicContentFolders;
using Elision.IDs;
using FluentAssertions;
using NUnit.Framework;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Tests.ReplaceRendering
{
    [TestFixture]
    public class ReplaceRenderingTests
    {
        [Test]
        public void CreatesMissingFolders()
        {
            using (GetFakeDb())
            {
                var processor = new CreateDatasourceFolders();
                var rendering = Sitecore.Context.Database.GetItem("/sitecore/content/rendering");
                var item1 = Sitecore.Context.Database.GetItem("/sitecore/content/page");
				var item2 = Sitecore.Context.Database.GetItem("/sitecore/content/page 2");

                item1.Children.Should().BeEmpty();
				item2.Children.Should().BeEmpty();

                processor.Process(new GetRenderingDatasourceArgs(rendering) { ContextItemPath = item1.Paths.FullPath });
				
                item1.Children.Should().NotBeEmpty();
				item2.Children.Should().NotBeEmpty();
            }
        }

        private static Db GetFakeDb()
        {
            return new Db()
                {
                    new DbItem("rendering", new ID(Guid.NewGuid()), Sitecore.Mvc.Names.TemplateIds.ViewRendering)
                        {
                            new DbField("Datasource Location") {Value = "./DsFolder" +
                                                                        "|/sitecore/content/page 2/DsFolder" +
                                                                        "|/sitecore/content/page 3/DsFolder"}
                        },
                    new DbItem("rendering with params", new ID(Guid.NewGuid()), Sitecore.Mvc.Names.TemplateIds.ViewRendering)
                        {
                            new DbField("Datasource Location") {Value = "./DsFolder"},
                            new DbField("Parameters") {Value = "contentFolderTemplate=" + Guid.NewGuid()}
                        }
                };
        }
    }
}
