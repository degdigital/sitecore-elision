using Elision.Web.ModelBuilders;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace Elision.Web.Controllers
{
    public class MetadataController : SitecoreController
    {
        private readonly IPageMetadataModelBuilder _modelBuilder;

        public MetadataController(IPageMetadataModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder ?? new PageMetadataModelBuilder();
        }

        public System.Web.Mvc.ActionResult PageMetadata()
        {
            var model = _modelBuilder.Build(RenderingContext.Current.ContextItem);
            return View(model);
        }
    }
}
