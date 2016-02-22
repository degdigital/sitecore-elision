using System.Text.RegularExpressions;
using Elision.ContentEditor.Pipelines.GetFieldReplacementTokens;
using Sitecore;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Pipelines;
using Sitecore.Pipelines.RenderField;
using Sitecore.Sites;
using Sitecore.Web;

namespace Elision.ContentEditor.Pipelines.RenderField
{
    public class ReplaceTokens
    {
        public void Process(RenderFieldArgs args)
        {
            Assert.ArgumentNotNull(args, "args");

            if (!ShouldReplaceTokens(args)) return;

            var getTokensArgs = new GetFieldReplacementTokensArgs(args);

            CorePipeline.Run("getFieldReplacementTokens", getTokensArgs);

            foreach (var replaceValue in getTokensArgs.ReplacementValues)
            {
                var tokenRegex = new Regex(@"{" + replaceValue.Key + "(?<fmt>:.*?)?}", RegexOptions.IgnorePatternWhitespace | RegexOptions.IgnoreCase);
                args.Result.FirstPart = tokenRegex.Replace(args.Result.FirstPart, m => string.Format("{0" + m.Groups["fmt"].Value + "}", replaceValue.Value));				
            }
        }

        private bool ShouldReplaceTokens(RenderFieldArgs args)
        {
            return args.Item != null && (!CanWebEdit(args) || !CanEditItem(args.Item));
        }

        private bool CanWebEdit(RenderFieldArgs args)
        {
            if (args.DisableWebEdit)
                return false;
            var site = Context.Site;
            return site != null && site.DisplayMode == DisplayMode.Edit && (WebUtil.GetQueryString("sc_duration") != "temporary" && Context.PageMode.IsExperienceEditorEditing);
        }

        private bool CanEditItem(Item item)
        {
            Assert.ArgumentNotNull(item, "item");
            return (Context.IsAdministrator || !item.Locking.IsLocked() || item.Locking.HasLock()) && (item.Access.CanWrite() && item.Access.CanWriteLanguage() && !item.Appearance.ReadOnly);
        }
    }
}