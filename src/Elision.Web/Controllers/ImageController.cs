using System.Web.Mvc;
using Elision.Entities.RenderingParameters;
using Elision.Entities.RenderingParameters.Content;
using Elision.Web.Models;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class ImageController : SitecoreController
    {
        public ActionResult ResponsiveImage(ResponsiveImageRenderingParameters args, RenderingActionArgs renderingActionArgs)
        {
            var model = new ResponsiveImageViewModel
                {
                    Item = renderingActionArgs.DatasourceItem, 
                    CssClass = args.CssClass
                };
            if (args.ResponsiveImageProfile != null)
                model.Profile = new ResponsiveImageProfile(args.ResponsiveImageProfile);

            return View(model);
        }

        public ActionResult Image()
        {
            return View();
        }
    }
}