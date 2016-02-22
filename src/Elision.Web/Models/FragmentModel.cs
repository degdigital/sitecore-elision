using System.Web;

namespace Elision.Web.Models
{
    public class FragmentModel
    {
        public HtmlString RenderedContents { get; set; }

        public string WebEditUrl { get; set; }
    }
}