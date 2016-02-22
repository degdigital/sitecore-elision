using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Layouts;

namespace Elision
{
    public static class LayoutExtensions
    {
        public static LayoutDefinition GetLayoutDefinition(this Item item)
        {
            var layoutField = item.Fields[SitecoreIDs.LayoutFieldId];
            return LayoutDefinition.Parse(LayoutField.GetFieldValue(layoutField));
        }

        public static void UpdateLayoutDefinition(this Item item, LayoutDefinition layoutDefinition)
        {
            var layoutField = item.Fields[SitecoreIDs.LayoutFieldId];
            LayoutField.SetFieldValue(layoutField, layoutDefinition.ToXml());
        }

        public static bool HasLayout(this Item item, DeviceItem device)
        {
            if (item == null || device == null) return false;
            if (item.Visualization.Layout == null) return false;

            var layout = item.Visualization.GetLayout(device);
            if (layout == null) return false;

            return true;
        }
    }
}
