using System;
using System.Collections.Generic;
using Sitecore.Configuration;
using Sitecore.Data;
using Sitecore.Diagnostics;
using Sitecore.Rules;

namespace Elision.Rules.Services
{
    public class EnhancedRuleList<T> : RuleList<T> where T : EnhancedRuleContext
    {
        public EnhancedRuleList(IEnumerable<Rule<T>> rules) : base (rules) { }

        protected override void Run(T ruleContext, bool stopOnFirstMatching, out int executedRulesCount)
        {
            Assert.ArgumentNotNull(ruleContext, "ruleContext");
            executedRulesCount = 0;
            if (Count == 0)
                return;

            using (new LongRunningOperationWatcher(Settings.Profiling.RenderFieldThreshold, "Long running rule set: {0}", new string[1] { this.Name ?? string.Empty }))
            {
                foreach (var rule in Rules)
                {
                    if (rule.Condition == null)
                        continue;

                    var stack = new RuleStack();
                    try
                    {
                        rule.Condition.Evaluate(ruleContext, stack);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(
                            string.Format(
                                "Evaluation of condition failed. Rule item ID: {0}, condition item ID: {1}",
                                !ID.IsNullOrEmpty(rule.UniqueId) ? rule.UniqueId.ToString() : "Unknown",
                                !string.IsNullOrWhiteSpace(rule.Condition.UniqueId) ? rule.Condition.UniqueId : "Unknown"),
                            ex, this);
                        ruleContext.Abort();
                    }

                    if (ruleContext.IsAborted)
                        break;

                    if (stack.Count == 0) 
                        continue;

                    if (!(bool)stack.Pop() || ruleContext.SkipRule)
                    {
                        ruleContext.SkipRule = false;
                        continue;
                    }

                    foreach (var action in rule.Actions)
                    {
                        try
                        {
                            action.Apply(ruleContext);
                        }
                        catch (Exception ex)
                        {
                            Log.Error(
                                string.Format(
                                    "Execution of action failed. Rule item ID: {0}, action item ID: {1}",
                                    !ID.IsNullOrEmpty(rule.UniqueId) ? rule.UniqueId.ToString() : "Unknown",
                                    !string.IsNullOrWhiteSpace(action.UniqueId) ? action.UniqueId : "Unknown"),
                                ex, this);
                            ruleContext.Abort();
                        }

                        if (ruleContext.IsAborted)
                            return;

                        if (ruleContext.StopProcessingThisRuleset)
                        {
                            ruleContext.StopProcessingThisRuleset = false;
                            return;
                        }
                        if (ruleContext.StopProcessingThisRule)
                        {
                            ruleContext.StopProcessingThisRule = false;
                            break;
                        }
                    }
                    ++executedRulesCount;

                    if (stopOnFirstMatching)
                        break;
                }
            }
        }
    }
}
