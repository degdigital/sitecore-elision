//using System;
//using System.Collections.Generic;
//using System.Linq;
//using Sitecore.ContentSearch.SearchTypes;

//namespace Elision.Search.SiteSearch
//{
//    public class SiteSearchResultItem : SearchResultItem
//    {
//        public string UrlLinks { get; set; }
//        private Dictionary<string, string> UrlLinksParsed { get; set; }

//        public virtual string GetUrl(string sitename = null)
//        {
//            if (string.IsNullOrWhiteSpace(UrlLinks))
//                return Url;

//            if (string.IsNullOrWhiteSpace(sitename) ||
//                Sitecore.Sites.SiteContextFactory.GetSiteNames().Any(x => x == sitename))
//                sitename = Sitecore.Context.GetSiteName();

//            if (UrlLinksParsed == null)
//                UrlLinksParsed = UrlLinks
//                    .Split("|".ToCharArray(), StringSplitOptions.RemoveEmptyEntries)
//                    .Select(x => x.Split(":".ToCharArray(), 2))
//                    .Where(x => x != null && x.Length == 2)
//                    .ToDictionary(x => x[0], x => x[1]);

//            if (UrlLinksParsed.ContainsKey(sitename))
//                return UrlLinksParsed[sitename];
//            return Url;
//        }
//    }
//}
