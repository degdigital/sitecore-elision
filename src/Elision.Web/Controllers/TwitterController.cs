using System.Web.Mvc;
using Elision.Fields;
using Elision.Web.Models;
using Sitecore.Data.Items;
using Sitecore.Mvc.Controllers;

namespace Elision.Web.Controllers
{
    public class TwitterController : SitecoreController
    {
        public ActionResult CardMeta(Item pageContextItem)
        {
            var model = new TwitterCardMetaViewModel
                {
                    CardType = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardType],
                    Site = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardSite],
                    SiteId = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardSiteId],
                    Title = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardTitle],
                    Description = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardDescription],
                    ContentCreator = pageContextItem[TwitterCardsMetaFieldIDs.TwitterCardContentCreator],
                    ImageUrl = pageContextItem.GetMediaUrl(TwitterCardsMetaFieldNames.TwitterCardImage)
                };

            return View(model);
        }
    }
}