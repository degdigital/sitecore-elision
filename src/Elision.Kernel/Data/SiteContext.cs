using Sitecore;
using Sitecore.Data.Items;

namespace Elision.Data
{
    public static class SiteContext
    {
        public static Item GetHomeItem()
        {
            return Context.Database.GetItem(Context.Site.StartPath);
        }
    }
}
