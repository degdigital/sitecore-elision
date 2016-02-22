using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class LinkController : SitecoreController
    {
        public System.Web.Mvc.ActionResult Link()
        {
            return View();
        }

        public System.Web.Mvc.ActionResult LinkContainer()
        {
            return View();
        }
    }
}