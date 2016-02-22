using System;
using System.Linq;
using Elision.Data;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Comparers;
using Sitecore.Data.Items;

namespace Elision.Search.ComputedFields
{
    public class LinkedItemField : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public virtual string RefFieldName { get; set; }

        public virtual string LinkedItemFieldName { get; set; }
        public virtual string Order { get; set; }
        public virtual string OrderReversed { get; set; }
        public virtual string Separator { get; set; }

        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            var sitecoreIndexableItem = ((SitecoreIndexableItem) indexable);
            if (sitecoreIndexableItem == null || sitecoreIndexableItem.Item == null)
                return null;

            var linkedItems = sitecoreIndexableItem
                .Item
                .GetLinkedItems(string.IsNullOrEmpty(RefFieldName) ? FieldName : RefFieldName);

            LinkedItemOrderBy orderBy;
            if (!Enum.TryParse(Order, true, out orderBy))
                orderBy = LinkedItemOrderBy.FieldValueOrder;

            switch (orderBy)
            {
                case LinkedItemOrderBy.CmsOrder:
                    linkedItems = linkedItems.OrderBy(x => x, new ItemComparer());
                    break;
                case LinkedItemOrderBy.NameAlpha:
                    linkedItems = linkedItems.OrderBy(x => string.IsNullOrWhiteSpace(x.DisplayName) ? x.Name : x.DisplayName);
                    break;
            }
            if (OrderReversed == "1" || OrderReversed == "True")
                linkedItems = linkedItems.Reverse();

            Func<Item, string> selector;
            if (string.IsNullOrWhiteSpace(LinkedItemFieldName) || LinkedItemFieldName.ToLowerInvariant() == "displayname")
                selector = x => string.IsNullOrWhiteSpace(x.DisplayName) ? x.Name : x.DisplayName;
            else if (LinkedItemFieldName.ToLowerInvariant() == "name")
                selector = x => x.Name;
            else
                selector = x => x[LinkedItemFieldName];

            return string.Join(Separator ?? "|", linkedItems.Select(selector));
        }
    }

    public enum LinkedItemOrderBy
    {
        FieldValueOrder,
        CmsOrder,
        NameAlpha
    }
}
