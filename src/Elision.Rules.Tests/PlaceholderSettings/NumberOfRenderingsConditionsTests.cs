//using System.Collections;
//using System.Collections.Generic;
//using Elision.Rules.PlaceholderSettings;
//using FluentAssertions;
//using Sitecore;
//using Sitecore.Data;
//using Sitecore.Data.Items;
//using Sitecore.FakeDb;
//using Sitecore.Layouts;
//using Sitecore.Pipelines.GetPlaceholderRenderings;
//using Sitecore.Rules;

//namespace Elision.Rules.Tests.PlaceholderSettings
//{
//    [TestClass]
//    public class NumberOfRenderingsConditionsTests
//    {
//        [TestCase("placeholder1", 3)]
//        [TestCase("placeholder2", 1)]
//        public void CountsAllRenderingsOnlyInRequestedPlaceholderRenderings(string placeholderKey, int expectedCount)
//        {
//            LayoutDefinition layoutDefinition;
//            ID[] renderingIds;
//            using (var db = GetDatabase(out layoutDefinition, out renderingIds))
//            {
//                var item = db.GetItem("/sitecore/content/page");
//                var args = new GetPlaceholderRenderingsArgs(placeholderKey, layoutDefinition.ToXml(), db.Database, ID.Parse(((DeviceDefinition)layoutDefinition.Devices[0]).ID))
//                {
//                    PlaceholderRenderings = new List<Item>()
//                };

//                var ctx = new PlaceholderSettingsRuleContext(args, item);

//                var condition = new NumberOfRenderingsInPlaceholderCondition<PlaceholderSettingsRuleContext>()
//                {
//                    OperatorId = "{066602E2-ED1D-44C2-A698-7ED27FD3A2CC}", //equals
//                    Value = expectedCount
//                };

//                var ruleStack = new RuleStack();

//                condition.Evaluate(ctx, ruleStack);

//                ruleStack.Should().HaveCount(1);
//                ruleStack.Pop().Should().Be(true);
//            }
//        }

//        [TestCase(0, 1)]
//        [TestCase(1, 1)]
//        [TestCase(2, 2)]
//        public void CountsAllRenderingsOnPage(int renderingIdIndex, int expectedCount)
//        {
//            LayoutDefinition layoutDefinition;
//            ID[] renderingIds;
//            using (var db = GetDatabase(out layoutDefinition, out renderingIds))
//            {
//                var item = db.GetItem("/sitecore/content/page");
//                var args = new GetPlaceholderRenderingsArgs("placeholder1", layoutDefinition.ToXml(), db.Database, ID.Parse(((DeviceDefinition)layoutDefinition.Devices[0]).ID))
//                {
//                    PlaceholderRenderings = new List<Item>()
//                };

//                var ctx = new PlaceholderSettingsRuleContext(args, item);

//                var condition = new NumberOfSpecificRenderingOnPageCondition<PlaceholderSettingsRuleContext>()
//                {
//                    RenderingItemId = renderingIds[renderingIdIndex].ToString(),
//                    OperatorId = "{066602E2-ED1D-44C2-A698-7ED27FD3A2CC}", //equals
//                    Value = expectedCount
//                };

//                var ruleStack = new RuleStack();

//                condition.Evaluate(ctx, ruleStack);

//                ruleStack.Should().HaveCount(1);
//                ruleStack.Pop().Should().Be(true);
//            }
//        }

//        private Db GetDatabase(out LayoutDefinition layoutDefinition, out ID[] renderingIds)
//        {
//            var ph1 = new PlaceholderDefinition { Key = "placeholder1", UniqueId = ID.NewID.ToString() };
//            var ph2 = new PlaceholderDefinition { Key = "placeholder2", UniqueId = ID.NewID.ToString() };
//            var deviceId = ID.NewID;

//            renderingIds = new[] {ID.NewID, ID.NewID, ID.NewID};
//            var deviceDefinition = new DeviceDefinition()
//                {
//                    ID = deviceId.ToString(),
//                    //Placeholders = new ArrayList() {ph1, ph2},
//                    Renderings = new ArrayList()
//                        {
//                            new RenderingDefinition() { Placeholder = ph1.Key, UniqueId = ID.NewID.ToString(), ItemID = renderingIds[0].ToString() },
//                            new RenderingDefinition() { Placeholder = ph1.Key, UniqueId = ID.NewID.ToString(), ItemID = renderingIds[1].ToString() },
//                            new RenderingDefinition() { Placeholder = ph1.Key, UniqueId = ID.NewID.ToString(), ItemID = renderingIds[2].ToString() },
//                            new RenderingDefinition() { Placeholder = ph2.Key, UniqueId = ID.NewID.ToString(), ItemID = renderingIds[2].ToString() }
//                        }
//                };

//            layoutDefinition = new LayoutDefinition()
//                {
//                    Devices = new ArrayList() {deviceDefinition}
//                };
//            return new Db()
//                {
//                    new DbItem("page"),
//                    new DbItem("Layout", ItemIDs.LayoutRoot)
//                        {
//                            new DbItem("DefaultDevice", deviceId),
//                            new DbItem("Rendering A", renderingIds[0]),
//                            new DbItem("Rendering B", renderingIds[1]),
//                            new DbItem("Rendering C", renderingIds[2])
//                        }
//                };

//        }
//    }
//}
