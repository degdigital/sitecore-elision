using System.Collections.Generic;
using System.Linq;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Elision.Search.ComputedFields
{
    public class BaseTemplates : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            var indexItem = (Item)(indexable as SitecoreIndexableItem);
            if (indexItem == null) return null;

            var baseTemplates = new List<ID>();

            baseTemplates.Add(indexItem.TemplateID);
            baseTemplates.AddRange(GetBaseTemplateIds(indexItem.Template));

            return baseTemplates.Distinct().ToArray();
        }

        protected virtual IEnumerable<ID> GetBaseTemplateIds(TemplateItem template)
        {
            foreach (var baseTemplate in template.BaseTemplates)
            {
                yield return baseTemplate.ID;
                foreach (var subTemplate in GetBaseTemplateIds(baseTemplate))
                {
                    yield return subTemplate;
                }
            }
        } 

        public string FieldName { get; set; }
        public string ReturnType { get; set; }
    }
}
