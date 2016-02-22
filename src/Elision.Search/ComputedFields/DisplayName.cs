using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace Elision.Search.ComputedFields
{
    public class DisplayName : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public object ComputeFieldValue(IIndexable indexable)
        {
            var indexItem = (Item)(indexable as SitecoreIndexableItem);
            if (indexItem == null) return string.Empty;

            return string.IsNullOrWhiteSpace(indexItem.DisplayName) ? indexItem.Name : indexItem.DisplayName;
        }
    }
}
