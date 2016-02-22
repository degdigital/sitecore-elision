using System;
using System.Collections.Generic;
using System.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Elision
{
    public static class LinkedItemExtensions
    {
        public static IEnumerable<Item> GetLinkedItems(this Item item, string fieldName)
        {
            var rawValue = item[fieldName];
            return rawValue.GetItemsFromRawValue(item.Database);
        }

        public static IEnumerable<Item> GetItemsFromRawValue(this string rawValue, Database db)
        {
            if (string.IsNullOrEmpty(rawValue))
                return new Item[0];

            var values = rawValue.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);

            ID linkedId;
            var linkedIds = values
                .Select(x => ID.TryParse(x, out linkedId) ? linkedId : ID.Null)
                .Where(x => x != ID.Null);

            var linkedItems = linkedIds
                .Select(db.GetItem)
                .Where(x => x != null);

            return linkedItems.ToArray();
        }
    }
}
