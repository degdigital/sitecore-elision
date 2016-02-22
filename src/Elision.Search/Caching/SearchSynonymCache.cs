using Elision.Search.LuceneProvider.Analyzers;
using Sitecore;
using Sitecore.Caching;
using Sitecore.Configuration;
using Sitecore.Data;

namespace Elision.Search.Caching
{
    public class SearchSynonymCache : CustomCache
    {
        public SearchSynonymCache(long maxSize) : base("DEG.SearchSynonymCache", maxSize) {}

        public static readonly SearchSynonymCache Current = new SearchSynonymCache(StringUtil.ParseSizeString(Settings.GetSetting("DEG.SearchSynonymCacheMaxSize", "50MB")));

        public SearchSynonymGroups Get(ID cacheKey)
        {
            return GetObject(cacheKey.ToString()) as SearchSynonymGroups;
        }

        public void Set(ID cacheKey, SearchSynonymGroups value)
        {
            SetObject(cacheKey.ToString(), value);
        }

        public void Remove(ID cacheKey)
        {
            base.Remove(cacheKey.ToString());
        }
    }
}
