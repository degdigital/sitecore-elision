using Elision.Entities.RenderingParameters;
using Elision.Entities.RenderingParameters.Navigation;
using Elision.Mvc;
using Elision.Web.ModelBuilders;
using Elision.Web.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace Elision.Web.Controllers
{
    public class NavigationController : SitecoreController
    {
        private readonly IBreadcrumbsModelBuilder _breadcrumbsModelBuilder;

        public NavigationController(IBreadcrumbsModelBuilder breadcrumbsModelBuilder)
        {
            _breadcrumbsModelBuilder = breadcrumbsModelBuilder;
        }

        public System.Web.Mvc.ActionResult Breadcrumbs(Item pageContextItem)
        {
            var model = _breadcrumbsModelBuilder.BuildModelForPage(pageContextItem);
            return View(model);
        }

        public System.Web.Mvc.ActionResult Primary(GeneratedNavigationRenderingParameters args, RenderingActionArgs renderingArgs)
        {
            var rootItem = renderingArgs.DatasourceItem ?? RenderingContext.Current.GetHomeItem();
            var model = new MainNavigationViewModel
                {
                    MenuItems = rootItem == null ? new NavigableItem[0] : new NavigableItem(rootItem).PrimaryNavChildren,
                    Levels = args.NavigationLevels <= 0 ? 3 : args.NavigationLevels,

                    CssClass = args.CssClass,
                    TopLevelListClass = args.TopLevelListClass,
                    TopLevelListItemClass = args.TopLevelListItemClass,
                    TopLevelLinkClass = args.TopLevelLinkClass,
                    ChildListClass = args.ChildListClass,
                    ChildListItemClass = args.ChildListItemClass,
                    ChildLinkClass = args.ChildLinkClass
                };
            return View(model);
        }

        public System.Web.Mvc.ActionResult Section(GeneratedNavigationRenderingParameters args, RenderingActionArgs renderingArgs)
        {
            var rootItem = renderingArgs.DatasourceItem
                           ?? RenderingContext.Current.GetSectionLandingPage(RenderingContext.Current.PageContext.Item)
                           ?? RenderingContext.Current.PageContext.Item;

            var model = new SectionNavigationViewModel
                {
                    RootItem = new NavigableItem(rootItem),
                    Levels = args.NavigationLevels <= 0 ? 1 : args.NavigationLevels,
                    IncludeTopLevel = args.IncludeTopLevel
                };
            return View(model);
        }
    }
}