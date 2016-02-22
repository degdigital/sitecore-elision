using System;
using System.Linq;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data.Events;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Publishing;

namespace Elision.Publishing
{
    public class RemoteHtmlCacheClearer : Sitecore.Publishing.HtmlCacheClearer
    {
        public void ClearRemoteCache(object sender, EventArgs args)
        {
            Assert.ArgumentNotNull(sender, "sender");
            Assert.ArgumentNotNull(args, "args");

            var siteNames = Sites != null && Sites.Count > 0
                                     ? Sites.Cast<string>().ToArray()
                                     : Factory.GetSiteNames();

            var targetDb = GetTargetDatabase(args);
            var dbString = targetDb == null ? string.Empty : " for sites associated with " + targetDb;
            Log.Info(this + " : clearing HTML caches" + dbString + "; " + siteNames.Length + " possible sites.", this);

            foreach (var site in siteNames.Select(Factory.GetSite).Where(x => x != null))
            {
                if (!site.CacheHtml)
                {
                    Log.Info(this + " : output caching not enabled for " + site.Name, this);
                    continue;
                }

                if (targetDb != null && site.Database != null && targetDb != site.Database.Name)
                {
                    Log.Info(this + " : " + targetDb + " not relevenat to " + site.Name, this);
                    continue;
                }

                var htmlCache = CacheManager.GetHtmlCache(site);
                Assert.IsNotNull(htmlCache, "htmlCache for " + site.Name);

                if (htmlCache.InnerCache.Count < 1)
                {
                    Log.Info(this + " : no entries in output cache for " + site.Name, this);
                    continue;
                }

                Log.Info(this + " clearing output cache for " + site.Name, this);
                htmlCache.Clear();
            }

            Log.Info(this + " done.", this);
        }

        private string GetTargetDatabase(EventArgs args)
        {
            Assert.IsNotNull(args, "args");
            var scArgs = args as SitecoreEventArgs;

            if (scArgs != null)
            {
                var publisher = scArgs.Parameters[0] as Publisher;

                if (publisher != null
                    && publisher.Options != null
                    && publisher.Options.TargetDatabase != null
                    && !string.IsNullOrEmpty(publisher.Options.TargetDatabase.Name))
                {
                    return publisher.Options.TargetDatabase.Name;
                }
            }
            else
            {
                var pubArgs = args as PublishEndRemoteEventArgs;
                if (pubArgs != null && !string.IsNullOrEmpty(pubArgs.TargetDatabaseName))
                    return pubArgs.TargetDatabaseName;
            }

            return null;
        }
    }
    
    //public class HtmlCacheClearer : Sitecore.Publishing.HtmlCacheClearer
    //{
    //    public new void ClearCache(object sender, EventArgs args)
    //    {
    //        var scArgs = args as Sitecore.Events.SitecoreEventArgs;
    //        var publisher = scArgs.Parameters[0] as Publisher;

    //        Log.Info(string.Format("CacheClearer root: {0}",
    //                               publisher.Options.RootItem == null
    //                                   ? "null"
    //                                   : publisher.Options.RootItem.ID.ToString()
    //                     ), this);
    //        Log.Info(string.Format("CacheClearer return values: {0}", string.Join(", ", scArgs.Result.ReturnValues.Cast<object>())), this);
    //        Log.Info(string.Format("CacheClearer result messages: {0}", string.Join(", ", scArgs.Result.Messages.Cast<string>())), this);

    //        base.ClearCache(sender, args);

    //        //Assert.ArgumentNotNull(sender, "sender");
    //        //Assert.ArgumentNotNull(args, "args");

    //        var pargs = args as PublishEndRemoteEventArgs;
    //        if (pargs == null) return;

    //        var did = new ID(pargs.RootItemId);
    //        Assert.IsNotNull(did, "publish root item id");

    //        var db = Factory.GetDatabase(pargs.TargetDatabaseName);
    //        if (db == null) return;

    //        var rootItem = db.GetItem(did);
    //        if (rootItem == null) return;

    //        var siteCaches =
    //            Factory.GetSiteInfoList()
    //                   .Where(siteInfo => !Sites.Contains(siteInfo.Name))
    //                   .Where(siteInfo => rootItem.Paths.ContentPath.Contains(siteInfo.StartItem))
    //                   .Select(si => Factory.GetSite(si.Name))
    //                   .Where(sc => sc != null)
    //                   .Select(CacheManager.GetHtmlCache)
    //                   .Where(htmlCache => htmlCache != null)
    //                   .ToArray();

    //        foreach (var htmlCache in siteCaches)
    //        {
    //            htmlCache.Clear();
    //        }
    //    }
    //}
}
