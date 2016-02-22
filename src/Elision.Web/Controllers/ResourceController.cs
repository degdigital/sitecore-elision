using System.Web.Mvc;
using Elision.Entities.RenderingParameters;
using Elision.Fields;
using Elision.Web.ModelBuilders;
using Sitecore.Data;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class ResourceController : SitecoreController
    {
        private readonly ISiteResourceModelBuilder _modelBuilder;

        public ResourceController(ISiteResourceModelBuilder modelBuilder)
        {
            _modelBuilder = modelBuilder;
        }

        public ActionResult Head(RenderingActionArgs args)
        {
            var model = _modelBuilder.Build(args.RenderingContextItem,
                                            ThemeResourceLocationIDs.Head,
                                            new ID(args.PageContext.Device.Id),
                                            SiteScriptsFieldNames.SiteHeadScript,
                                            PageScriptsFieldNames.PageHeadScript);

            return View(model);
        }

        public ActionResult BodyTop(RenderingActionArgs args)
        {
            var model = _modelBuilder.Build(args.RenderingContextItem,
                                            ThemeResourceLocationIDs.BodyTop,
                                            new ID(args.PageContext.Device.Id),
                                            null,
                                            null);

            return View(model);
        }

        public ActionResult BodyBottom(RenderingActionArgs args)
        {
            var model = _modelBuilder.Build(args.RenderingContextItem,
                                            ThemeResourceLocationIDs.BodyBottom,
                                            new ID(args.PageContext.Device.Id),
                                            SiteScriptsFieldNames.SiteFootScript,
                                            PageScriptsFieldNames.PageFootScript);

            return View(model);
        }
    }
}