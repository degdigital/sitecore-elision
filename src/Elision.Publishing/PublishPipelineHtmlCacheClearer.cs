using System;
using System.Linq;
using Sitecore.Configuration;
using Sitecore.Diagnostics;
using Sitecore.Publishing.Pipelines.Publish;
using Sitecore.Web;

namespace Elision.Publishing
{
    public class PublishPipelineHtmlCacheClearer : PublishProcessor
    {
        public override void Process(PublishContext context)
        {
            if (!PublishChangedSiteContent(context))
            {
                Log.Info("No items were updated in publish operation. Skipping cache clearing step.", this);
                return;
            }

            var sites = Factory.GetSiteInfoList()
                               .Where(site => SiteCacheNeedsCleared(site, context))
                               .ToArray();
            if (!sites.Any())
                Log.Info(this + " : No sites found that need cache cleared.", this);

            foreach (var site in sites)
            {
                Log.Info(this + " : Clearing HtmlCache for site " + site.Name, this);
                site.HtmlCache.Clear();
            }
        }

        private bool SiteCacheNeedsCleared(SiteInfo site, PublishContext context)
        {
            if (!site.CacheHtml || site.HtmlCache == null)
                return false;

            if (!SiteUsesPublishTargetDatabase(site, context)) 
                return false;

            return PublishRootIsUnderSiteRoot(site, context);
        }

        private static bool PublishRootIsUnderSiteRoot(SiteInfo site, PublishContext context)
        {
            if (context.PublishOptions.RootItem == null) 
                return true;

            return context.PublishOptions.RootItem.Paths
                           .ContentPath.StartsWith(site.RootPath, StringComparison.InvariantCultureIgnoreCase);
        }

        private static bool SiteUsesPublishTargetDatabase(SiteInfo site, PublishContext context)
        {
            if (string.IsNullOrWhiteSpace(site.Database) || context.PublishOptions.TargetDatabase == null)
                return true;

            if (site.Database.Equals(context.PublishOptions.TargetDatabase.Name, StringComparison.InvariantCultureIgnoreCase))
                return true;

            return false;
        }

        private bool PublishChangedSiteContent(PublishContext context)
        {
            return context.Statistics.Created > 0
                   || context.Statistics.Deleted > 0
                   || context.Statistics.Updated > 0;
        }
    }
}
