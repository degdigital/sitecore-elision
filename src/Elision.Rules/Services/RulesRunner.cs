using System;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace Elision.Rules.Services
{
    public interface IRulesRunner
    {
        void RunGlobalRules<T>(Item rulesFolder, T context) where T : EnhancedRuleContext;
        void RunGlobalRules<T>(ID rulesFolderId, Database db, T context) where T : EnhancedRuleContext;
    }

    public class RulesRunner : IRulesRunner
    {
        public void RunGlobalRules<T>(Item rulesFolder, T context) where T : EnhancedRuleContext
        {
            if (rulesFolder == null)
            {
                Log.SingleError("Rules folder not found:\r\n" + Environment.StackTrace, this);
                return;
            }

            foreach (var ruleItem in rulesFolder.Axes.GetDescendants())
            {
                var ruleXml = ruleItem["Rule"];
                if (string.IsNullOrWhiteSpace(ruleXml) || ruleItem["Disabled"] == "1")
                    continue;

                var rules = new EnhancedRuleList<T>(RuleFactory.ParseRules<T>(ruleItem.Database, ruleXml).Rules);

                rules.Run(context);

                if (context.IsAborted)
                    break;
                if (context.StopProcessingAfterThisRuleset)
                {
                    context.StopProcessingAfterThisRuleset = false;
                    break;
                }
            }
        }

        public void RunGlobalRules<T>(ID rulesFolderId, Database db, T context) where T : EnhancedRuleContext
        {
            Assert.ArgumentNotNull(db, "db");
            Assert.IsFalse(ID.IsNullOrEmpty(rulesFolderId), "rulesFolderId not specified");

            var rulesFolder = db.GetItem(rulesFolderId);
            if (rulesFolder == null)
            {
                Log.SingleError(string.Format("Rules folder for id '{0}' not found in '{1}' database", rulesFolderId, db.Name), this);
                return;
            }

            RunGlobalRules(rulesFolder, context);
        }
    }
}
