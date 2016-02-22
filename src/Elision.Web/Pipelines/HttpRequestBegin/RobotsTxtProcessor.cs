using Elision.Data;
using Elision.Fields;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Web;

namespace Elision.Web.Pipelines.HttpRequestBegin
{
    public class RobotsTxtProcessor : HttpRequestProcessor, IHttpHandler
    {
        public override void Process(HttpRequestArgs args)
        {
            var context = HttpContext.Current;
            if (context == null)
                return;

            ProcessRequest(context);
        }

        public void ProcessRequest(HttpContext context)
        {
            var requestUrl = context.Request.RawUrl;
            if (string.IsNullOrWhiteSpace(requestUrl) || !requestUrl.ToLower().EndsWith("robots.txt"))
                return;

            var robotsTxtContent = @"User-agent: *" + Environment.NewLine
                                   + "Disallow: /sitecore" + Environment.NewLine
                                   + "Sitemap: /sitemap.xml";

            if (Sitecore.Context.Site != null && Sitecore.Context.Database != null)
            {
                var homeNode = SiteContext.GetHomeItem();
                if (homeNode != null)
                {
                    robotsTxtContent = homeNode.Fields.GetValue(RobotsTxtGenerationFieldIDs.RobotsTxt)
                                               .Or(robotsTxtContent);
                }
            }

            context.Response.ContentType = "text/plain";
            context.Response.Write(robotsTxtContent);
            context.Response.End();
        }

        public bool IsReusable { get { return true; } }
    }
}
