using Elision.Data;

namespace Elision.Sitemap.GenerateSitemapXml
{
    public class GetSitemapRoot : IGenerateSitemapProcessor
    {
        public void Process(GenerateSitemapArgs args)
        {
            if (args.RootItem == null)
                args.RootItem = SiteContext.GetHomeItem();
        }
    }
}
