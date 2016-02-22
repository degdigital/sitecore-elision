using Elision.Rules.GetRenderingDatasource;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Rules.Tests.GetRenderingDatasource
{
    [TestClass]
    public class AddGlobalDatasourceRootActionTests
    {
        [TestMethod]
        public void AddsGlobalDatasourceRoot()
        {
            var newTemplateId = ID.NewID;

            using (var db = new Db()
                {
                    new DbTemplate("ComponentsFolder", ID.Parse("{122AE27A-D84F-4C5E-8367-0F42C764976E}")),
                    new DbTemplate("PageTemplate", newTemplateId),
                    new DbItem("Website", ID.NewID, TemplateIDs.WebsiteFolder)
                        {
                            new DbItem("home"),
                            new DbItem("components")
                        },
                    new DbItem("rendering")
                        {
                            new DbField("Datasource template") { Value = newTemplateId.ToString() }
                        }
                })
            {
                var home = db.GetItem("/sitecore/content/Website/home");
                var rendering = db.GetItem("/sitecore/content/rendering");
                var ctx = new GetRenderingDatasourceRuleContext(new GetRenderingDatasourceArgs(rendering)
                    {
                        ContextItemPath = home.Paths.FullPath
                    });
                var action = new AddGlobalDatasourceRootAction<GetRenderingDatasourceRuleContext>();

                action.Apply(ctx);

                ctx.Args.DatasourceRoots.Should().HaveCount(1).And.ContainSingle(x => x.Name == "PageTemplate");
                db.GetItem("/sitecore/content/_components").Should().NotBeNull();
            }
        }

    }
}
