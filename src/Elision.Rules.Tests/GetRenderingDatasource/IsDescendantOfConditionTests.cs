using Elision.Rules.GetRenderingDatasource;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Sitecore.FakeDb;
using Sitecore.Rules;
using System;

namespace Elision.Rules.Tests.GetRenderingDatasource
{
    //[TestClass]
    public class IsDescendantOfConditionTests
    {
        public TestContext TestContext { get; set; }

        //[TestCase("/sitecore/content/parent1", "/sitecore/content/parent1", true, TestName = "Detects self")]
        //[TestCase("/sitecore/content/parent1/child1a/child1a1", "/sitecore/content/parent1/child1a/child1a2", false, TestName = "Fails on siblings")]
        //[TestCase("/sitecore/content/parent1/child1a/child1a2", "/sitecore/content/parent1", false, TestName = "Fails on parent")]
        //[TestCase("/sitecore/content/parent-missing", "/sitecore/content/parent1/child1a", false, TestName = "Fails when parent not found")]
        //[TestCase("/sitecore/content/parent1", "/sitecore/content/parent1/child-missing", false, TestName = "Fails when child not found")]
        //[TestCase("/sitecore/content/parent-missing", "/sitecore/content/parent1/child-missing", false, TestName = "Fails when both not found")]

        //[TestMethod]
        //[DeploymentItem("\\Data\\CorrectlyDetectsChild.xml")]
        //[DataSource("Microsoft.VisualStudio.TestTools.DataSource.XML", "|DataDirectory|\\CorrectlyDetectsChild.xml", "Row", DataAccessMethod.Sequential)]
        public void CorrectlyDetectsChild()
        {
            var parentPath = (string)TestContext.DataRow["parentPath"];
            var childPath = (string)TestContext.DataRow["childPath"];
            var result = Boolean.Parse((string)TestContext.DataRow["result"]);
            var message = (string)TestContext.DataRow["message"];
            ExecCorrectlyDetectsChild(parentPath, childPath, result, message);
        }

        public void ExecCorrectlyDetectsChild(string parentPath, string childPath, bool expectedResult, string message)
        {
            using (var db = GetFakeDb())
            {
                var item = db.GetItem(childPath);
                var root = db.GetItem(parentPath);
                var context = new RuleContext {Item = item};

                var condition = new IsDescendentOfCondition<RuleContext>
                    {
                        PotentialParent = root
                    };

                var ruleStack = new RuleStack();

                condition.Evaluate(context, ruleStack);

                ruleStack.Should().HaveCount(1);
                ruleStack.Pop().Should().Be(expectedResult, message);
            }
        }

        private static Db GetFakeDb()
        {
            return new Db
                {
                    new DbItem("parent1")
                        {
                            new DbItem("child1a")
                                {
                                    new DbItem("child1a1"),
                                    new DbItem("child1a2")
                                }
                        }
                };
        }
    }
}
