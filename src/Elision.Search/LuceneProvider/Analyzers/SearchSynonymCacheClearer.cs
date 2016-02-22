using System;
using Elision.Fields;
using Elision.Search.Caching;
using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Events;

namespace Elision.Search.LuceneProvider.Analyzers
{
    public class SearchSynonymCacheClearer
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null)
                return;

            ClearSearchSynonymCache(item);
        }

        public void OnItemSavedRemote(object sender, EventArgs args)
        {
            var savedRemoteEventArgs = args as ItemSavedRemoteEventArgs;
            if (savedRemoteEventArgs == null)
                return;

            ClearSearchSynonymCache(savedRemoteEventArgs.Item);
        }

        protected virtual void ClearSearchSynonymCache(Item item)
        {
            if (item.InheritsFrom(SearchSynonymsFieldIDs.TemplateId))
                SearchSynonymCache.Current.Remove(item.ParentID);
        }
    }
}
