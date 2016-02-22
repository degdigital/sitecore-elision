using System.Web.Mvc;
using Elision.Entities.RenderingParameters.Structure;
using Elision.Web.ModelBuilders;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class StructureController : SitecoreController
    {
	    private readonly IStructureModelBuilder _structureModelBuilder;

	    public StructureController(IStructureModelBuilder structureModelBuilder)
	    {
		    _structureModelBuilder = structureModelBuilder;
	    }

        public ActionResult OneColumn(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult TwoColumn(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult TwoColumnWideLeft(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult TwoColumnWideRight(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult ThreeColumn(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult ThreeColumnWideCenter(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }

        public ActionResult FourColumn(StructureRenderingParameters parms)
        {
            var model = _structureModelBuilder.Build(parms);
            return View(model);
        }
    }
}