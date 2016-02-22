using Sitecore;
using Sitecore.Caching;
using Sitecore.Configuration;

namespace Elision.Sitemap.Caching
{
    public class SitemapXmlCache : CustomCache
    {
        public SitemapXmlCache(long maxSize) : base("Elision.SitemapXmlCache", maxSize) { }

        public static readonly SitemapXmlCache Current = new SitemapXmlCache(StringUtil.ParseSizeString(Settings.GetSetting("Elision.SitemapXmlCacheMaxSize", "50MB")));

        public SitemapXmlFile Get(string cacheKey)
        {
            return GetObject(cacheKey) as SitemapXmlFile;
        }

        public void Set(string cacheKey, SitemapXmlFile value)
        {
            SetObject(cacheKey, value);
        }

        public void Remove(string cacheKey)
        {
            base.Remove(cacheKey);
        }
    }

    public class SitemapXmlFile : ICacheable
    {
        public readonly string Content;

        public SitemapXmlFile(string content)
        {
            Content = content;

            Cacheable = true;
            Immutable = true;
        }

        public long GetDataLength()
        {
            return (Content ?? "").Length * sizeof(char);
        }

        public bool Cacheable { get; set; }
        public bool Immutable { get; private set; }

        public event DataLengthChangedDelegate DataLengthChanged;
    }
}
