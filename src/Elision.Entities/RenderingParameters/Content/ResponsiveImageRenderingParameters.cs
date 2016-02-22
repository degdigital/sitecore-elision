using Elision.Entities.ParameterFields.Design;
using Elision.Entities.ParameterFields.Image;
using Sitecore.Data.Items;

namespace Elision.Entities.RenderingParameters.Content
{
    public class ResponsiveImageRenderingParameters : IResponsiveImageProfileParameters, IClassParameters
    {
        public Item ResponsiveImageProfile { get; set; }
        public string CssClass { get; set; }
    }
}
