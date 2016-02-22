using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Items;

namespace Elision.Search.ComputedFields
{
    public class TemplateDisplayName: IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            var indexItem = (Item)(indexable as SitecoreIndexableItem);
            if (indexItem == null) return null;

            return indexItem.Template.DisplayName;
        }

        public string FieldName { get; set; }
        public string ReturnType { get; set; }
    }
}
