using System;
using System.Collections.Generic;
using System.Linq;
using Elision.Diagnostics;
using Elision.Rules.CompatibleRenderings;
using Elision.Rules.Services;
using Newtonsoft.Json;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Layouts;
using Sitecore.Pipelines.GetChromeData;

namespace Elision.ExperienceEditor.Pipelines.GetChromeData
{
    public class RunCompatibleRenderingRules : GetChromeDataProcessor
    {
        private readonly IRulesRunner _rulesRunner;

        public RunCompatibleRenderingRules(IRulesRunner rulesRunner)
        {
            _rulesRunner = rulesRunner;
        }

        public override void Process(GetChromeDataArgs args)
        {
            using (new TraceOperation("Run GetCompatibleRendering rules."))
            {
                var renderingReference = args.CustomData["renderingReference"] as RenderingReference;
                if (renderingReference == null) return;

                var startingCompatibleRenderings = GetCompatibleRenderingsList(args);
                var context = new GetCompatibleRenderingsRuleContext(renderingReference, startingCompatibleRenderings, args.Item);

                _rulesRunner.RunGlobalRules(RuleIDs.GetCompatibleRenderingsGlobalRulesRootId, renderingReference.Database, context);

                UpdateChromeCompatibleRenderings(args, context.CompatibleRenderings);
            }
        }

        private void UpdateChromeCompatibleRenderings(GetChromeDataArgs args, List<ID> compatibleRenderings)
        {
            var morphButton = args.ChromeData.Commands.FirstOrDefault(x => x.Click.StartsWith("chrome:rendering:morph"));
            if (compatibleRenderings.Any())
            {
                if (morphButton == null)
                {
                    morphButton = GetDefaultMorphButton();
                    if (morphButton == null) return;

                    //add the morph button to the chrome data
                    this.AddButtonToChromeData(morphButton, args);
                }

                //set the morph button ids
                var jsonIds = JsonConvert.SerializeObject(
                    compatibleRenderings.Select(x => x.ToShortID().ToString()).ToList())
                                     .Replace("\"", "'");
                morphButton.Click = string.Format("{0}({1})", "chrome:rendering:morph", jsonIds);
            }
            else if (morphButton != null)
            {
                args.ChromeData.Commands.Remove(morphButton);
            }
        }

        private WebEditButton GetDefaultMorphButton()
        {
            var buttons = this.GetButtons("/sitecore/content/Applications/WebEdit/Default Rendering Buttons");
            return buttons.FirstOrDefault(b => "chrome:rendering:morph".Equals(b.Click, StringComparison.InvariantCultureIgnoreCase));
        }

        private static IEnumerable<ID> GetCompatibleRenderingsList(GetChromeDataArgs args)
        {
            var morphButton = args.ChromeData.Commands.FirstOrDefault(x => x.Click.StartsWith("chrome:rendering:morph"));
            if (morphButton == null) return new ID[0];

            var ids = morphButton.Click.Split(',', '\'', '"', '(', ')');
            return ids.Where(ShortID.IsShortID).Select(x => ShortID.Parse(x).ToID());
        }
    }
}
