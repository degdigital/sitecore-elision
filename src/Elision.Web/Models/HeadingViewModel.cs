using Sitecore.Data;

namespace Elision.Web.Models
{
    public class HeadingViewModel : IRenderingWithCssClass
    {
        public string HeadingLevel { get; set; }
        public string CssClass { get; set; }
        public ID RenderingUniqueId { get; set; }
    }
}