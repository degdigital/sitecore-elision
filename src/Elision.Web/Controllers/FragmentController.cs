using System.Web;
using System.Web.Mvc;
using Elision.Entities.RenderingParameters;
using Elision.Web.Models;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class FragmentController : SitecoreController
    {
        public ActionResult Fragment(RenderingActionArgs args)
        {
            var model = new FragmentModel
                {
                    WebEditUrl = string.Format("/?sc_mode=edit&sc_itemid={0}&sc_lang={1}",
                                               HttpUtility.UrlEncode(args.DatasourceItem.ID.ToString()), args.DatasourceItem.Language)
                };

            return View(model);
        }

        public ActionResult FragmentContainer()
        {
            return View();
        }
    }
}
