using System;
using System.Web;
using Sitecore;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Pipelines.Response.GetPageRendering;

namespace Elision.Web.Caching
{
    public class SetBrowserCachingRenderingProcessor : GetPageRenderingProcessor
    {
        public override void Process(GetPageRenderingArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.PageContext == null
                || args.PageContext.RequestContext == null
                || args.PageContext.RequestContext.HttpContext == null
                || args.PageContext.RequestContext.HttpContext.Response == null)
                return;

            SetCacheHeaders(args.PageContext.RequestContext.HttpContext.Response);
            if (Context.Item != null)
                SetUpdateHeaders(Context.Item, args.PageContext.RequestContext.HttpContext.Response);
        }

        private static void SetCacheHeaders(HttpResponseBase response)
        {
            if ((Context.Site == null && !Settings.DisableBrowserCaching)
                || (Context.Site != null && !Context.Site.DisableBrowserCaching))
                return;

            Tracer.Info("Adding Http headers to disable caching.");
            response.Cache.SetNoStore();
            response.Cache.SetCacheability(HttpCacheability.NoCache);
        }

        private static void SetUpdateHeaders(Item item, HttpResponseBase response)
        {
            var date = item.Statistics.Updated;
            if (date > DateTime.Now)
                date = DateTime.Now;

            Tracer.Info("Adding Http header to indicate last modification.", "Date: " + date + ".");
            response.Cache.SetLastModified(date);
        }
    }
}
