using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;

namespace Elision.Search
{
    public static class SearchIndexExtensions
    {
        public static IEnumerable<string> GetSearchTerms(this ISearchIndex index, string prefix = "")
        {
            using (var context = index.CreateSearchContext())
            {
                var termsByFieldName = context.GetTermsByFieldName("_content", prefix)
                    .Union(context.GetTermsByFieldName("tagname", prefix))
                    .Union(context.GetTermsByFieldName("displayname", prefix));
                return termsByFieldName
                              .OrderByDescending(x => x.DocumentFrequency)
                              .Select(x => x.Term)
                              .Distinct()
                              .ToArray();
            }
        }

        public static IEnumerable<string> GetAllFieldValues(this ISearchIndex index, string fieldName)
        {
            using (var context = index.CreateSearchContext())
            {
                var termsByFieldName = context.GetTermsByFieldName((fieldName ?? "").ToLowerInvariant(), null);
                return termsByFieldName
                              .Select(x => x.Term)
                              .ToArray();
            }
        }
    }
}
