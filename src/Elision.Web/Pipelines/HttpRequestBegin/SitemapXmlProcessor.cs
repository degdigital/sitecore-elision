using Elision.Sitemap.GenerateSitemapXml;
using System.Web;
using Sitecore.Pipelines;

namespace Elision.Web.Pipelines.HttpRequestBegin
{
    public class SitemapXmlProcessor : IHttpHandler
    {
        public void ProcessRequest(HttpContext context)
        {
            var args = new GenerateSitemapArgs()
                {
                    RequestUrl = context.Request.RawUrl
                };
            CorePipeline.Run("getSitemapXml", args);
            
            if (string.IsNullOrWhiteSpace(args.Content))
                return;

            context.Response.ContentType = "application/xml";
            context.Response.Write(args.Content);
            context.Response.End();
        }

        public bool IsReusable { get { return true; } }
    }
}
