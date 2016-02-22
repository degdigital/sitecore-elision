using Elision.Rules.ReplaceRendering;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.Data;
using Sitecore.FakeDb;
using Sitecore.Layouts;
using Sitecore.Rules;

namespace Elision.Rules.Tests.ReplaceRendering
{
    [TestClass]
    public class TargetRenderingIsConditionTests
    {
        [TestMethod]
        public void MatchesTargetId()
        {
            var sourceRenderingId = ID.NewID;
            var targetRenderingId = ID.NewID;
            using (var db = new Db{ new DbItem("rendering", targetRenderingId)})
            {
                var targetRenderingItem = db.GetItem(targetRenderingId);

                var context = new ReplaceRenderingRuleContext(new RenderingDefinition(){ ItemID = sourceRenderingId.ToString() }, targetRenderingItem, new DeviceDefinition());

                var condition = new TargetRenderingIsAnyOfCondition<ReplaceRenderingRuleContext>
                {
                     CompareRenderingItemIds = targetRenderingId.ToString()
                };

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(true);
            }
        }

        [TestMethod]
        public void DoesNotMatchSourceId()
        {
            var sourceRenderingId = ID.NewID;
            var targetRenderingId = ID.NewID;
            using (var db = new Db { new DbItem("rendering", targetRenderingId) })
            {
                var targetRenderingItem = db.GetItem(targetRenderingId);

                var context = new ReplaceRenderingRuleContext(new RenderingDefinition() { ItemID = sourceRenderingId.ToString() }, targetRenderingItem, new DeviceDefinition());

                var condition = new TargetRenderingIsAnyOfCondition<ReplaceRenderingRuleContext>
                {
                    CompareRenderingItemIds = sourceRenderingId.ToString()
                };

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(false);
            }
        }

        [TestMethod]
        public void FailsButDoesNotErrorWhenCompareIdNotSet()
        {
            var sourceRenderingId = ID.NewID;
            var targetRenderingId = ID.NewID;
            using (var db = new Db { new DbItem("rendering", targetRenderingId) })
            {
                var targetRenderingItem = db.GetItem(targetRenderingId);

                var context = new ReplaceRenderingRuleContext(new RenderingDefinition() { ItemID = sourceRenderingId.ToString() }, targetRenderingItem, new DeviceDefinition());

                var condition = new TargetRenderingIsAnyOfCondition<ReplaceRenderingRuleContext>();

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(false);
            }
        }

        [TestMethod]
        public void MatchesWhenComparingMultipleTargetIds()
        {
            var sourceRenderingId = ID.NewID;
            var targetRenderingId = ID.NewID;
            using (var db = new Db { new DbItem("rendering", targetRenderingId) })
            {
                var targetRenderingItem = db.GetItem(targetRenderingId);

                var context = new ReplaceRenderingRuleContext(new RenderingDefinition() { ItemID = sourceRenderingId.ToString() }, targetRenderingItem, new DeviceDefinition());

                var condition = new TargetRenderingIsAnyOfCondition<ReplaceRenderingRuleContext>
                {
                    CompareRenderingItemIds = string.Format("{0}|{1}|{2}", ID.NewID, ID.NewID, targetRenderingId)
                };

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(true);
            }
        }
    }
}
