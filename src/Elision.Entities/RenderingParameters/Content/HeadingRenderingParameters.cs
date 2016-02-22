using Elision.Entities.ParameterFields.Content;
using Elision.Entities.ParameterFields.Design;
using Sitecore.Data.Items;

namespace Elision.Entities.RenderingParameters.Content
{
    public class HeadingRenderingParameters : ITextAlignParameters, IClassParameters, IHeadingLevelParameters
    {
        public Item TextAlign { get; set; }
        public string CssClass { get; set; }
        public string HeadingLevel { get; set; }
    }
}
