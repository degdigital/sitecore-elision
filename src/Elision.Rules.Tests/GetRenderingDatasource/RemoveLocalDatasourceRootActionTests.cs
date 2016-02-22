using Elision.Rules.GetRenderingDatasource;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Rules.Tests.GetRenderingDatasource
{
    [TestClass]
    public class RemoveLocalDatasourceRootActionTests
    {
        [TestMethod]
        public void RemovesDatasourceRoot()
        {
            var newTemplateId = ID.NewID;
            var componentsFolderTemplateId = ID.Parse("{122AE27A-D84F-4C5E-8367-0F42C764976E}");
            var dsFolderId = ID.NewID;
            using (var db = new Db()
                {
                    new DbTemplate("ComponentsFolder", componentsFolderTemplateId),
                    new DbTemplate("PageTemplate", newTemplateId),
                    new DbItem("home")
                        {
                            new DbItem("_components", ID.NewID, componentsFolderTemplateId)
                                {
                                    new DbItem("PageTemplate", dsFolderId, componentsFolderTemplateId)
                                }
                        },
                    new DbItem("rendering")
                        {
                            new DbField("Datasource template") {Value = newTemplateId.ToString()}
                        }
                })
            {
                var home = db.GetItem("/sitecore/content/home");
                var rendering = db.GetItem("/sitecore/content/rendering");
                var args = new GetRenderingDatasourceArgs(rendering) { ContextItemPath = home.Paths.FullPath };
                args.DatasourceRoots.Add(db.GetItem(dsFolderId));

                var ctx = new GetRenderingDatasourceRuleContext(args);
                var action = new RemoveLocalDatasourceRootAction<GetRenderingDatasourceRuleContext>();

                ctx.Args.DatasourceRoots.Should().HaveCount(1);

                action.Apply(ctx);

                ctx.Args.DatasourceRoots.Should().HaveCount(0);
            }
        }
    }
}
