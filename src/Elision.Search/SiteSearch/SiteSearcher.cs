using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.SearchTypes;

namespace Elision.Search.SiteSearch
{
    public interface ISiteSearcher
    {
        ContentSearchResults<SearchResultItem> Search(SiteSearchOptions options);
    }

    public class SiteSearcher : ContentSearcher<SearchResultItem, SiteSearchOptions>, ISiteSearcher
    {
        private readonly ISiteSearchQueryableBuilder _queryableBuilder;

        public SiteSearcher(ISiteSearchQueryableBuilder queryableBuilder)
        {
            _queryableBuilder = queryableBuilder;
        }

        protected override IQueryable<SearchResultItem> BuildQueryable(SiteSearchOptions options, IProviderSearchContext searchContext)
        {
            var queryable = base.BuildQueryable(options, searchContext);

            queryable = _queryableBuilder.ApplyWebsiteFilter(queryable, options);
            queryable = _queryableBuilder.ApplyKeywordFilter(queryable, options);

            return queryable;
        }
    }
}
