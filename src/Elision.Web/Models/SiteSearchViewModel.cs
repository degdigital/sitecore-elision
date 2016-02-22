using System;
using System.Collections.Generic;
using Elision.Search.SiteSearch;

namespace Elision.Web.Models
{
    public class SiteSearchViewModel
    {
        public IEnumerable<SiteSearchResultViewModel> Results { get; set; }
        public SiteSearchOptions Options { get; set; }

        public int TotalResults { get; set; }
    }

    public class SiteSearchResultViewModel
    {
        public string Name { get; set; }
        public DateTime Updated { get; set; }
        public string Url { get; set; }
    }
}