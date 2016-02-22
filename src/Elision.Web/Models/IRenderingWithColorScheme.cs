namespace Elision.Web.Models
{
	public interface IRenderingWithColorScheme : IRendering
	{
		string BackgroundColor { get; set; }
		string ForegroundColor { get; set; }
	}
}