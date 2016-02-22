using Elision.Rules.GetRenderingDatasource;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.Rules.Tests.GetRenderingDatasource
{
    [TestClass]
    public class AddDatasourceRootActionTests
    {
        [TestMethod]
        public void AddsDatasourceRoot()
        {
            using (var db = new Db() {new DbItem("home"), new DbItem("components")})
            {
                var home = db.GetItem("/sitecore/content/home");
                var ctx = new GetRenderingDatasourceRuleContext(new GetRenderingDatasourceArgs(home));
                var action = new AddDatasourceRootAction<GetRenderingDatasourceRuleContext>()
                    {
                        NewDatasourceRoot = home
                    };

                action.Apply(ctx);

                ctx.Args.DatasourceRoots.Should().HaveCount(1).And.ContainSingle(x => x.ID == home.ID);
            }
        }

        [TestMethod]
        public void IgnoresAddingMissingItem()
        {
            using (var db = new Db() {new DbItem("home"), new DbItem("components")})
            {
                var home = db.GetItem("/sitecore/content/home");
                var ctx = new GetRenderingDatasourceRuleContext(new GetRenderingDatasourceArgs(home));
                var action = new AddDatasourceRootAction<GetRenderingDatasourceRuleContext>()
                    {
                        NewDatasourceRoot = null
                    };

                action.Apply(ctx);

                ctx.Args.DatasourceRoots.Should().BeEmpty();
            }
        }

    }
}
