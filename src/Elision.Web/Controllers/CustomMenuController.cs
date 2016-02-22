using System.Web.Mvc;
using Elision.Entities.RenderingParameters.Navigation;
using Elision.Web.Models;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class CustomMenuController : SitecoreController
    {
        public ActionResult Container(CustomMenuRenderingParameters args)
        {
            return View(args);
        }

        public ActionResult MenuItem(CustomMenuItemRenderingParameters args)
        {
	        var model = new MenuItemViewModel();
	        if (args != null)
		        model.CssClass = args.CssClass;
			return View(model);
        }
    }
}