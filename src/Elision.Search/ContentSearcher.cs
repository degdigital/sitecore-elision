using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.Linq;
using Sitecore.Data.Items;

namespace Elision.Search
{
    public abstract class ContentSearcher<TResult, TOptions>
        where TResult : ISearchResult
        where TOptions : ContentSearchOptions
    {
        public virtual ContentSearchResults<TResult> Search(TOptions options)
        {
            var index = GetIndex(options);

            using (var searchContext = index.CreateSearchContext())
            {
                var queryable = BuildQueryable(options, searchContext);

                return BuildResults(queryable, options);
            }
        }

        protected virtual ContentSearchResults<TResult> BuildResults(IQueryable<TResult> queryable, TOptions options)
        {
            var results = queryable.GetResults();

            var totalResults = results.TotalSearchResults;

            return new ContentSearchResults<TResult>()
            {
                Documents = GetReturnDocuments(options, results),
                FacetCategories = results.Facets == null ? new List<FacetCategory>() : results.Facets.Categories,
                TotalSearchResults = totalResults
            };
        }

        protected virtual TResult[] GetReturnDocuments(TOptions options, SearchResults<TResult> results)
        {
            return results.Hits.Skip(options.Paging.Offset).Take(options.Paging.Limit).Select(x => x.Document).ToArray();
        }

        protected virtual IQueryable<TResult> BuildQueryable(TOptions options, IProviderSearchContext searchContext)
        {
            return searchContext.GetQueryable<TResult>();
        }

        protected virtual ISearchIndex GetIndex(TOptions options)
        {
            return ContentSearchManager.GetIndex(new SitecoreIndexableItem(options.ContextPage));
        }
    }

    public class ContentSearchResults<T> where T : ISearchResult
    {
        public T[] Documents { get; set; }
        public int TotalSearchResults { get; set; }
        public List<FacetCategory> FacetCategories { get; set; }
    }

    public class ContentSearchOptions
    {
        public readonly ContentSearchPagingOptions Paging;
        public readonly Item ContextPage;

        public ContentSearchOptions() : this(null, null) { }
        public ContentSearchOptions(ContentSearchPagingOptions pagingOptions, Item contextPage)
        {
            ContextPage = contextPage ?? Sitecore.Context.Item;
            Paging = pagingOptions ?? ContentSearchPagingOptions.None;
        }
    }

    public class ContentSearchPagingOptions
    {
        public readonly int Offset;
        public readonly int Limit;

        private ContentSearchPagingOptions(int offset, int limit)
        {
            Limit = limit;
            Offset = offset;
        }



        public static readonly ContentSearchPagingOptions None = new ContentSearchPagingOptions(0, int.MaxValue);
        public static ContentSearchPagingOptions FromOffset(int offset, int limit)
        {
            return new ContentSearchPagingOptions(offset, limit);
        }
        public static ContentSearchPagingOptions FromPage(int pageNumber, int pageSize)
        {
            return new ContentSearchPagingOptions((pageNumber - 1) * pageSize, pageSize);
        }
    }
}
