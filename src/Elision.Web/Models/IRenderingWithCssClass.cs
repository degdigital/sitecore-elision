namespace Elision.Web.Models
{
    public interface IRenderingWithCssClass : IRendering
    {
        string CssClass { get; set; }
    }
}