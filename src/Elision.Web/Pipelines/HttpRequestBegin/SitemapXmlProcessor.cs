using Elision.Sitemap.GenerateSitemapXml;
using System.Web;
using Sitecore.Pipelines;
using Sitecore.Pipelines.HttpRequest;

namespace Elision.Web.Pipelines.HttpRequestBegin
{
    public class SitemapXmlProcessor : HttpRequestProcessor, IHttpHandler
    {

        public override void Process(HttpRequestArgs args)
        {
            ProcessRequest(args.Context);
        }

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
