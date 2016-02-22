using System;
using System.Collections.Generic;
using System.Linq;
using Elision.Search.Caching;
using Sitecore.ContentSearch.LuceneProvider.Analyzers;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;

namespace Elision.Search.LuceneProvider.Analyzers
{
    public class SitecoreSynonymEngine : ISynonymEngine
    {
        private readonly Item _folder;

        public SitecoreSynonymEngine(string synonymItemFolder, string database)
        {
            Assert.ArgumentNotNullOrEmpty(synonymItemFolder, "synonymItemFolder");
            var db = Database.GetDatabase(database);

            var folder = ID.IsID(synonymItemFolder)
                             ? db.GetItem(ID.Parse(synonymItemFolder))
                             : db.GetItem(synonymItemFolder);
            
            Assert.IsNotNull(folder, "synonym folder not found " + synonymItemFolder);

            _folder = folder;
        }

        public IEnumerable<string> GetSynonyms(string word)
        {
            Assert.ArgumentNotNull(word, "word");
            return SynonymGroups.Terms.FirstOrDefault(readOnlyCollection => readOnlyCollection.Contains(word));
        }

        protected SearchSynonymGroups SynonymGroups
        {
            get
            {
                SearchSynonymGroups synonymGroups;
                lock (SearchSynonymCache.Current)
                {
                    synonymGroups = SearchSynonymCache.Current.Get(_folder.ID);
                }
                if (synonymGroups != null) 
                    return synonymGroups;

                var items = _folder.Axes.GetDescendants();

                synonymGroups = new SearchSynonymGroups(
                    items
                        .Select(x => x.Fields["Synonyms"])
                        .Where(x => x != null)
                        .Select(
                            x =>
                            x.Value.Split(",".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
                             .Select(s => s.Trim())
                             .ToArray())
                        .ToArray());

                lock (SearchSynonymCache.Current)
                {
                    SearchSynonymCache.Current.Set(_folder.ID, synonymGroups);
                }
                return synonymGroups;
            }
        }
    }
}
