using System.Linq;
using System.Web.Mvc;
using Elision.Search;
using Elision.Search.SiteSearch;
using Elision.Web.Models;
using Sitecore.Mvc.Controllers;
using Sitecore.Mvc.Presentation;

namespace Elision.Web.Controllers
{
    public class SearchController : SitecoreController
    {
        private readonly ISiteSearcher _siteSearcher;
        public SearchController(ISiteSearcher siteSearcher)
        {
            _siteSearcher = siteSearcher;
        }

        public ActionResult SiteSearch(string q = null, int p = 1, int s = 10)
        {
            var searchOptions = new SiteSearchOptions(q,
                                                      PageContext.Current.Item,
                                                      ContentSearchPagingOptions.FromPage(p, s));

            var model = new SiteSearchViewModel
                {
                    Options = searchOptions
                };

            if (!string.IsNullOrWhiteSpace(q))
            {
                var results = _siteSearcher.Search(searchOptions);

                model.TotalResults = results.TotalSearchResults;
                model.Results = results.Documents.Select(x => new SiteSearchResultViewModel
                    {
                        Name = x.Name,
                        Updated = x.Updated,
                        Url = x.Url
                    });
            }
            return View(model);
        }
    }
}
