using System;
using System.Linq;
using Sitecore.Diagnostics;
using Sitecore.Pipelines.GetChromeData;

namespace Elision.CascadingRenderings
{
    public class GetCascadedPlaceholderChromeData : GetChromeDataProcessor
    {
        public override void Process(GetChromeDataArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            Assert.IsNotNull(args.ChromeData, "Chrome Data");
            if (!"placeholder".Equals(args.ChromeType, StringComparison.OrdinalIgnoreCase))
                return;

            if (args.Item == null)
                return;

            if (!Sitecore.Configuration.Settings.GetBoolSetting("Elision.CascadingRenderings.Enabled", false))
                return;

            var cascadable = true;
            var cascaded = false;

            var placeholderKey = args.CustomData["placeHolderKey"] as string;
            if (string.IsNullOrWhiteSpace(placeholderKey))
                cascadable = false;

            var cascadedPhField = args.Item.Fields[LayoutIDs.CascadePlaceholdersField];
            if (cascadedPhField == null)
            {
                cascadable = false;
            }
            else
            {
                cascaded = (cascadedPhField.Value ?? "").Split('|').Contains(placeholderKey);
                cascadable = !cascaded;
            }

            var cascadeCommand = args.ChromeData.Commands.FirstOrDefault(x => x.Click.Contains("deg:placeholder:cascade()"));
            if (cascadeCommand != null)
            {
                if (cascadable)
                {
                    cascadeCommand.Click = cascadeCommand
                        .Click.Replace("deg:placeholder:cascade()",
                                       string.Format("deg:placeholder:cascade(placeholder={0})", placeholderKey));
                }
                else
                {
                    args.ChromeData.Commands.Remove(cascadeCommand);
                }
            }

            var uncascadeCommand = args.ChromeData.Commands.FirstOrDefault(x => x.Click.Contains("deg:placeholder:uncascade()"));
            if (uncascadeCommand != null)
            {
                if (cascaded)
                {
                    uncascadeCommand.Click = uncascadeCommand
                        .Click.Replace("deg:placeholder:uncascade()",
                                       string.Format("deg:placeholder:uncascade(placeholder={0})", placeholderKey));
                }
                else
                {
                    args.ChromeData.Commands.Remove(uncascadeCommand);
                }
            }
        }
    }
}
