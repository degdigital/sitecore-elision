using System.Linq;

namespace Elision.Sitemap.GenerateSitemapXml
{
    public class GetPageItems : IGenerateSitemapProcessor
    {
        public void Process(GenerateSitemapArgs args)
        {
            args.Items = new[] {args.RootItem}.Union(args.RootItem.Axes.GetDescendants());
        }
    }
}
