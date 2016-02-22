using Sitecore.Data.Items;

namespace Elision.Web.Models
{
    public class ResponsiveImageViewModel
    {
		public string CssClass { get; set; }
        public ResponsiveImageProfile Profile { get; set; }
        public Item Item { get; set; }
    }
}