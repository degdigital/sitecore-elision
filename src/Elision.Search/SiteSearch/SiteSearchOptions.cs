using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Elision.Search.SiteSearch
{
    public class SiteSearchOptions : ContentSearchOptions
    {
        public readonly string Query;

        public SiteSearchOptions(string query, Item contextPage, ContentSearchPagingOptions paging)
            : base(paging, contextPage)
        {
            Query = query;
            Assert.ArgumentNotNull(contextPage, "contextPage");
        }
    }
}
