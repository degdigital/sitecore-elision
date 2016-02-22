using Elision.Web.Models;
using Sitecore.Data.Items;

namespace Elision.Web.ModelBuilders
{
    public interface IBreadcrumbsModelBuilder
    {
        BreadcrumbsModel BuildModelForPage(Item page);
    }

    public class BreadcrumbsModelBuilder : IBreadcrumbsModelBuilder
    {
        public BreadcrumbsModel BuildModelForPage(Item page)
        {
            var model = new BreadcrumbsModel();

            var current = new NavigableItem(page, page);
            while (current != null && current.InnerItem.TemplateID != TemplateIDs.WebsiteFolder)
            {
                if (current.ShowInPrimaryMenu || current.ShowInSectionMenu)
                    model.Crumbs.Push(current);

                current = current.Parent;
            }

            return model;
        }
    }
}