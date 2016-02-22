using System;
using System.Linq;
using Elision.Fields;
using Sitecore.ContentSearch.Linq;
using Sitecore.ContentSearch.Linq.Utilities;
using Sitecore.ContentSearch.SearchTypes;

namespace Elision.Search.SiteSearch
{
    public interface ISiteSearchQueryableBuilder
    {
        IQueryable<TResult> ApplyKeywordFilter<TResult, TOptions>(IQueryable<TResult> queryable, TOptions options)
            where TResult : SearchResultItem
            where TOptions : SiteSearchOptions;

        IQueryable<TResult> ApplyWebsiteFilter<TResult, TOptions>(IQueryable<TResult> queryable, TOptions options)
            where TResult : SearchResultItem
            where TOptions : SiteSearchOptions;
    }

    public class SiteSearchQueryableBuilder : ISiteSearchQueryableBuilder
    {
        public virtual IQueryable<TResult> ApplyKeywordFilter<TResult, TOptions>(IQueryable<TResult> queryable, TOptions options)
            where TResult : SearchResultItem
            where TOptions : SiteSearchOptions
        {
            if (string.IsNullOrWhiteSpace(options.Query))
                return queryable;

            var keywords = options.Query.ToLowerInvariant().Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            var contentPredicate = PredicateBuilder.True<TResult>();
            contentPredicate = keywords.Aggregate(contentPredicate, (current, keyword) => current.And(x => x.Content.Like(keyword)));

            var namePredicate = PredicateBuilder.True<TResult>();
            namePredicate = keywords.Aggregate(namePredicate, (current, keyword) => current.And(x => x.Name.Like(keyword)));

            return queryable.Where(contentPredicate.Or(namePredicate));
        }

        public virtual IQueryable<TResult> ApplyWebsiteFilter<TResult, TOptions>(IQueryable<TResult> queryable, TOptions options)
            where TResult : SearchResultItem
            where TOptions : SiteSearchOptions
        {
            if (options.ContextPage == null)
                return queryable;

            var homeItem = options.ContextPage.Axes.GetAncestors()
                                  .Reverse()
                                  .FirstOrDefault(x => x.InheritsFrom(TemplateIDs.WebsiteFolder));
            if (homeItem == null)
                return queryable;

            var websitePredicate = PredicateBuilder.True<TResult>()
                                                   .And(x => x.Paths.Contains(homeItem.ID))
                                                   .And(x => !"1".Equals(x[SearchableFieldNames.HideFromSiteSearch]));

            return queryable.Where(websitePredicate);
        }
    }
}
