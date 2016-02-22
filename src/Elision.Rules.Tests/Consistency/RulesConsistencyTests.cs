using System.Collections.Generic;
using System.Linq;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Rules.Tests.Consistency
{
    [TestClass]
    public class RulesConsistencyTests
    {
        [TestMethod]
        [Ignore] //"FakeDB serialization is letting me down again. It doesn't seem to load the field values for the rule actions."
        public void AllReferencedRuleActionClassesExist()
        {
            using (GetSerializedData())
            {
                var ruleActions = Sitecore.Context.Database.SelectItems("//*[@@TemplateID='{F90052A5-B4E6-4E6D-9812-1E1B88A6FCEA}']"); 
                var issues = new List<string>();

                if (ruleActions.Length == 0)
                    Assert.Inconclusive("There were no rule actions found. Check the tds projects to make sure this is correct.");

                foreach (var ruleActionItem in ruleActions)
                {
                    var ruleActionTypeName = ruleActionItem.Fields["Type"].Value;
                    var ruleActionType = Sitecore.Reflection.ReflectionUtil.GetTypeInfo(ruleActionTypeName);
                    
                    if (ruleActionType == null)
                    {
                        issues.Add(string.Format("Unable to find type '{0}' for rule action '{1}' {2}",
                                                 ruleActionTypeName,
                                                 ruleActionItem.Paths.Path, ruleActionItem.ID));
                        continue;
                    }
                }

                if (issues.Any())
                    Assert.Fail(string.Join("\r\n", issues));
            }
        }

        private static Db GetSerializedData()
        {
            return new Db
                {
                    new DsDbItem("/sitecore/system/Settings", true, false) {ParentID = Sitecore.ItemIDs.SystemRoot}
                };
        }
    }
}
