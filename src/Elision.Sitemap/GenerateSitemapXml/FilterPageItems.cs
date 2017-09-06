using System.Linq;
using Elision.Fields;

namespace Elision.Sitemap.GenerateSitemapXml
{
    public class FilterPageItems : IGenerateSitemapProcessor
    {
        public void Process(GenerateSitemapArgs args)
        {
            if (args.Items.Any())
                args.Items = args.Items.Where(x => x.InheritsFrom(ContentPageFieldIDs.TemplateId) && x.Visualization.Layout != null);
        }
    }
}
