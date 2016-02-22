using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.FieldReaders;
using Sitecore.ContentSearch.LuceneProvider;
using Sitecore.ContentSearch.LuceneProvider.Analyzers;
using Sitecore.ContentSearch.LuceneProvider.Converters;
using Sitecore.ContentSearch.Maintenance;
using Sitecore.Data.Items;

namespace Elision.Search.Tests.Fixtures
{
    public static class TestIndexBuilder
    {

        public static ISearchIndex CreateIndex(IEnumerable<Item> items)
        {
            var index = new InMemoryLuceneIndex(Guid.NewGuid().ToString("N"))
                {
                    PropertyStore = new NullPropertyStore(),
                    Configuration = new LuceneIndexConfiguration()
                        {
                            IndexDocumentPropertyMapper = new DefaultLuceneDocumentTypeMapper(),
                            IndexFieldStorageValueFormatter = new LuceneIndexFieldStorageValueFormatter(),
                            FieldReaders = new FieldReaderMap(),
                            Analyzer = new LowerCaseKeywordAnalyzer()
                        }
                };

            index.AddCrawler(new SitecoreFlatItemCrawler(items.Select(x => new SitecoreIndexableItem(x))));

            index.Initialize();

            index.Rebuild(IndexingOptions.ForcedIndexing);

            return index;
        }
    }

    public class SitecoreFlatItemCrawler : FlatDataCrawler<SitecoreIndexableItem>
    {
        private IEnumerable<SitecoreIndexableItem> Indexables { get; set; }

        public SitecoreFlatItemCrawler(IEnumerable<SitecoreIndexableItem> indexables)
        {
            Indexables = indexables;
        }

        protected override IEnumerable<SitecoreIndexableItem> GetItemsToIndex()
        {
            return Indexables;
        }

        protected override SitecoreIndexableItem GetIndexableAndCheckDeletes(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }

        protected override SitecoreIndexableItem GetIndexable(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }

        protected override bool IndexUpdateNeedDelete(SitecoreIndexableItem indexable)
        {
            throw new NotImplementedException();
        }

        protected override IEnumerable<IIndexableUniqueId> GetIndexablesToUpdateOnDelete(IIndexableUniqueId indexableUniqueId)
        {
            throw new NotImplementedException();
        }
    }
}
