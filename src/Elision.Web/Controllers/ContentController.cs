using System.Web.Mvc;
using Elision.Entities.RenderingParameters.Content;
using Elision.Fields;
using Elision.Web.Models;
using Sitecore.Data;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class ContentController : SitecoreController
    {
        public ActionResult Heading(HeadingRenderingParameters args, Database database)
        {
            var model = new HeadingViewModel
                {
                    HeadingLevel = args.HeadingLevel.Or("H1").ToLower(),
                    CssClass = args.CssClass
                };

            if (args.TextAlign != null)
                model.CssClass = string.Join(" ", new[] {model.CssClass, args.TextAlign[CssClassOptionFieldIDs.CssClass]});

            return View(model);
        }

        public ActionResult ContentBlock()
        {
            return View();
        }
    }
}