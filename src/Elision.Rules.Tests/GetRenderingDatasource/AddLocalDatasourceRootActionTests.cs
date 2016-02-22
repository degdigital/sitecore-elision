using Elision.Rules.GetRenderingDatasource;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Rules.Tests.GetRenderingDatasource
{
    [TestClass]
    public class AddLocalDatasourceRootActionTests
    {
        [TestMethod]
        public void AddsDatasourceRoot()
        {
            using (var db = GetFakeDb())
            {
                var home = db.GetItem("/sitecore/content/home");
                var rendering = db.GetItem("/sitecore/content/rendering");
                var ctx = new GetRenderingDatasourceRuleContext(new GetRenderingDatasourceArgs(rendering)
                    {
                        ContextItemPath = home.Paths.FullPath
                    });
                var action = new AddLocalDatasourceRootAction<GetRenderingDatasourceRuleContext>();

                action.Apply(ctx);

                ctx.Args.DatasourceRoots.Should().HaveCount(1).And.ContainSingle(x => x.Name == "ContentBlock");
            }
        }

        private static Db GetFakeDb()
        {
            var newTemplateId = ID.NewID;
            return new Db()
                {
                    new DbTemplate("ComponentsFolder", ID.Parse("{122AE27A-D84F-4C5E-8367-0F42C764976E}")),
                    new DbTemplate("ContentBlock", newTemplateId)
                        {
                            new DbField(Sitecore.FieldIDs.DisplayName) { Value = "Content Block" }
                        },
                    new DbItem("home"), new DbItem("components"),
                    new DbItem("rendering")
                        {
                            new DbField("Datasource template")
                                {
                                    Value = newTemplateId.ToString()
                                }
                        }
                };
        }
    }
}
