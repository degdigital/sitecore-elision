using System;

namespace Elision.ContentEditor.Pipelines.GetFieldReplacementTokens
{
    public class GetTokens : IGetFieldReplacementTokensProcessor
    {
        public void Process(GetFieldReplacementTokensArgs args)
        {
            var dateToUse = Sitecore.Configuration.State.Previewing
                                     ? Sitecore.Configuration.State.PreviewDate
                                     : DateTime.Now;

            args.ReplacementValues.Add("Now", dateToUse);
            args.ReplacementValues.Add("DisplayName", args.RenderFieldArgs.Item.DisplayName);
        }
    }
}