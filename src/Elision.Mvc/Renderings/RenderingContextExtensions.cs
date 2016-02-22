using System.Linq;
using Elision.Data;
using Sitecore.Collections;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Elision.Mvc
{
    public static class RenderingContextExtensions
    {
        public static Item GetHomeItem(this RenderingContext context)
        {
            return SiteContext.GetHomeItem();
        }

        public static Item GetSectionLandingPage(this RenderingContext context, Item contextItem)
        {
            var homeItem = GetHomeItem(context);
            if (homeItem == null) return null;

            return homeItem
                .GetChildren(ChildListOptions.SkipSorting)
                .FirstOrDefault(x => x.Axes.IsAncestorOf(contextItem));
        }

        public static Item ResolveDatasource(this RenderingContext context)
        {
            return context.ContextItem.Database.ResolveDatasource(context.Rendering.DataSource, context.PageContext.Item);
        }
    }
}
