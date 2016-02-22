using Sitecore.Data;

namespace Elision.Web.Models
{
    public class StructureViewModel : IRendering, IRenderingWithColorScheme, IRenderingWithCssClass
    {
		public string CssClass { get; set; }

	    public ID RenderingUniqueId { get; set; }
	    public string BackgroundColor { get; set; }
	    public string ForegroundColor { get; set; }
	}
}