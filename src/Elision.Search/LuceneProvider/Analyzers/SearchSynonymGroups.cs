using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Sitecore.Caching;

namespace Elision.Search.LuceneProvider.Analyzers
{
    public class SearchSynonymGroups : ICacheable
    {
        public readonly ReadOnlyCollection<string>[] Terms;

        public SearchSynonymGroups(IEnumerable<IEnumerable<string>> terms)
        {
            Terms = terms.Select(x => new ReadOnlyCollection<string>(x.ToList())).ToArray();
        }

        public long GetDataLength()
        {
            return Terms.Sum(x => x.Sum(y => y.Length));
        }

        private bool? _cacheable;
        public bool Cacheable
        {
            get { return _cacheable.GetValueOrDefault(true); }
            set { _cacheable = value; }
        }

        public bool Immutable { get { return true; } }
        public event DataLengthChangedDelegate DataLengthChanged;
    }
}