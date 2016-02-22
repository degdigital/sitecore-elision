using System.Collections;
using System.Linq;
using Elision.Rules.PlaceholderSettings;
using FluentAssertions;
using Sitecore;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Layouts;
using Sitecore.Pipelines.GetPlaceholderRenderings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Rules.Tests.PlaceholderSettings
{
    [TestClass]
    public class ClearRenderingActionTests
    {
        [TestMethod]
        public void ClearsAllRenderings()
        {
            ID[] ids;
            var layoutDefinition = new LayoutDefinition() {Devices = new ArrayList() {new DeviceDefinition()}};
            
            using (var db = GetDatabase(out ids))
            {
                var item = db.GetItem("/sitecore/home/page");
                var args = new GetPlaceholderRenderingsArgs("placeholder", layoutDefinition.ToXml(), db.Database)
                    {
                        PlaceholderRenderings = db.Database.SelectItems("//Layouts/*").ToList()
                    };

                args.PlaceholderRenderings.Should().NotBeEmpty();

                var ctx = new PlaceholderSettingsRuleContext(args, item);

                var action = new ClearRenderingsAction<PlaceholderSettingsRuleContext>();

                action.Apply(ctx);

                args.PlaceholderRenderings.Should().BeEmpty();
            }
        }

        private Db GetDatabase(out ID[] renderingIds)
        {
            renderingIds = new[] {ID.NewID, ID.NewID, ID.NewID, ID.NewID};
            var db = new Db()
                {
                    new DbItem("page"),
                    new DbItem("Layouts", ItemIDs.LayoutRoot)
                        {
                            new DbItem("Rendering A", renderingIds[0]),
                            new DbItem("Rendering B", renderingIds[1]),
                            new DbItem("Rendering C", renderingIds[2]),
                            new DbItem("Rendering D", renderingIds[3])
                        }
                };
            return db;
        }
    }
}
