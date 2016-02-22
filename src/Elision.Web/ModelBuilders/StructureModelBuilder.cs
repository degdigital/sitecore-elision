using Elision.Entities.RenderingParameters.Structure;
using Elision.Fields;
using Elision.Web.Models;

namespace Elision.Web.ModelBuilders
{
	public interface IStructureModelBuilder
	{
	    StructureViewModel Build(StructureRenderingParameters args);
	}

	public class StructureModelBuilder : IStructureModelBuilder
	{
        public StructureViewModel Build(StructureRenderingParameters args)
        {
            var model = new StructureViewModel
                {
                    CssClass = args.CssClass,
                    RenderingUniqueId = args.RenderingUniqueId
                };

            if (args.ColorScheme != null)
			{
                model.BackgroundColor = args.ColorScheme[ColorSchemeFieldNames.BackgroundColor];
                model.ForegroundColor = args.ColorScheme[ColorSchemeFieldNames.ForegroundColor];
			}

			return model;
		}
	}
}