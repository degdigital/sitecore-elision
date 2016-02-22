using System.Web;
using Elision.Diagnostics;
using Sitecore.Links;

namespace Elision.Web.Pipelines.GetCanonicalUrl
{
    public class GetCanonicalUrlForAlias : IGetCanonicalUrlProcessor
    {
        public void Process(GetCanonicalUrlArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.CanonicalUrl))
                return;

            using (var trace = new TraceOperation("GetCanonicalUrlForAlias"))
            {
                if (!Sitecore.Configuration.Settings.AliasesActive)
                {
                    trace.Debug("Skipping because AliasesActive = false");
                    return;
                }
                if (!Sitecore.Context.Database.Aliases.Exists(args.RawUrl))
                {
                    trace.Debug(string.Format("Skipping because alias not found in database '{0}' for raw url '{1}'", Sitecore.Context.Database.Name, args.RawUrl));
                    return;
                }

                var targetId = Sitecore.Context.Database.Aliases.GetTargetID(args.RawUrl);
                var targetItem = Sitecore.Context.Database.GetItem(targetId);
                if (targetItem == null)
                {
                    trace.Warning(string.Format("Unable to find the item '{0}' defined in the alias.", targetId));
                    return;
                }

                if (HttpContext.Current != null)
                {
                    const string href = "{0}://{1}{2}";
                    args.CanonicalUrl =
                        string.Format(href,
                                      HttpContext.Current.Request.Url.Scheme,
                                      HttpContext.Current.Request.Url.Host,
                                      LinkManager.GetItemUrl(targetItem));
                }
                else
                {
                    args.CanonicalUrl = LinkManager.GetItemUrl(targetItem);
                }
                trace.Info(string.Format("Setting canonical url to '{0}'", args.CanonicalUrl));
            }
        }
    }
}