using System.Web;

namespace Elision.Web.Models
{
    public class SiteResourceViewModel
    {
        public HtmlString SiteResources { get; set; }

        public HtmlString PageResources { get; set; }

        public HtmlString ThemeResources { get; set; }
    }
}