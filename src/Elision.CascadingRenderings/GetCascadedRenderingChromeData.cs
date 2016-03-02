using System;
using Sitecore;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetChromeData;

namespace Elision.CascadingRenderings
{
    public class GetCascadedRenderingChromeData : GetChromeDataProcessor
    {
        public override void Process(GetChromeDataArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.ChromeData, "Chrome Data");
            if (!"rendering".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
                return;

            if (!Sitecore.Configuration.Settings.GetBoolSetting("Elision.CascadingRenderings.Enabled", false))
                return;

            var renderingReference = args.CustomData["renderingReference"] as Sitecore.Layouts.RenderingReference;
            if (renderingReference == null || renderingReference.Settings == null)
                return;

            if (StringUtil.ExtractParameter("wascascaded", renderingReference.Settings.Parameters) == "1")
            {
                var cascadedFrom = StringUtil.ExtractParameter("cascadedfrom", renderingReference.Settings.Parameters);
                args.ChromeData.DisplayName += string.IsNullOrWhiteSpace(cascadedFrom)
                                                   ? " (Cascaded)"
                                                   : " (Cascaded from " + cascadedFrom + ")";
                args.ChromeData.Commands.RemoveAll(x =>
                                                   x.Click.StartsWith("chrome:rendering:sort")
                                                   || x.Click.Contains("webedit:setdatasource")
                                                   || x.Click.StartsWith("chrome:rendering:delete")
                                                   || x.Click.StartsWith("chrome:rendering:properties"));
            }
        }
    }
}
