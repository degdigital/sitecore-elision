using System.Collections.Generic;
using System.Linq;
using Sitecore;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;

namespace Elision.Search.ComputedFields
{
    public class ComponentsContentField : IComputedIndexField
    {
        public object ComputeFieldValue(IIndexable indexable)
        {
            var sitecoreIndexable = indexable as SitecoreIndexableItem;

            if (sitecoreIndexable == null) return null;

            var customDataSources = ExtractRenderingDataSourceItems(sitecoreIndexable.Item);

            var contentToAdd = customDataSources.SelectMany(GetItemContent).ToList();

            if (contentToAdd.Count == 0) return null;

            return string.Join(" ", contentToAdd);
        }

        protected virtual IEnumerable<Item> ExtractRenderingDataSourceItems(Item baseItem)
        {
            var layout = baseItem.GetLayoutDefinition();

            for (var deviceIndex = layout.Devices.Count - 1; deviceIndex >= 0; deviceIndex--)
            {
                var device = layout.Devices[deviceIndex] as DeviceDefinition;

                if (device == null) continue;

                for (int renderingIndex = device.Renderings.Count - 1; renderingIndex >= 0; renderingIndex--)
                {
                    var rendering = device.Renderings[renderingIndex] as RenderingDefinition;

                    if (rendering == null) continue;
                    if (string.IsNullOrWhiteSpace(rendering.Datasource))  continue;

                    var dataSource = baseItem.Database.ResolveDatasource(rendering.Datasource, baseItem);
                    if (dataSource != baseItem)
                        yield return dataSource;
                }
            }

        }

        protected virtual IEnumerable<string> GetItemContent(Item dataSource)
        {
            foreach (Field field in dataSource.Fields)
            {
                // this check is what Sitecore uses to determine if a field belongs in _content (see LuceneDocumentBuilder.AddField())
                if (!IndexOperationsHelper.IsTextField(new SitecoreItemDataField(field))) continue;

                var fieldValue = StringUtil.RemoveTags(field.Value ?? string.Empty);

                if (!string.IsNullOrWhiteSpace(fieldValue)) yield return fieldValue;
            }
        }

        public string FieldName { get; set; }
        public string ReturnType { get; set; }
    }
}
