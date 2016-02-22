using System.Xml;
using Sitecore.ContentSearch;
using Sitecore.ContentSearch.ComputedFields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Elision.Search.ComputedFields
{
    public class ImageUrl : IComputedIndexField
    {
        public string FieldName { get; set; }
        public string ReturnType { get; set; }

        public virtual string RefFieldName { get; set; }
        public virtual int? Width { get; set; }
        public virtual int? Height { get; set; }
        public virtual bool? Thumbnail { get; set; }

        public ImageUrl(){}
        public ImageUrl(XmlNode node)
        {
            if (node == null)
                return;

            var attrib = node.Attributes["refFieldName"];
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value))
                RefFieldName = attrib.Value;

            int parsedInt;
            attrib = node.Attributes["width"];
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value) && int.TryParse(attrib.Value, out parsedInt))
                Width = parsedInt;

            attrib = node.Attributes["height"];
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value) && int.TryParse(attrib.Value, out parsedInt))
                Height = parsedInt;

            bool parsedBool;
            attrib = node.Attributes["thumbnail"];
            if (attrib != null && !string.IsNullOrWhiteSpace(attrib.Value) && bool.TryParse(attrib.Value, out parsedBool))
                Thumbnail = parsedBool;
        }

        public virtual object ComputeFieldValue(IIndexable indexable)
        {
            var item = (Item)(indexable as SitecoreIndexableItem);
            if (item == null)
                return null;

            var imageField = (ImageField)item.Fields[string.IsNullOrWhiteSpace(RefFieldName) ? FieldName : RefFieldName];
            if (imageField == null)
                return null; //string.Format("FieldName={0}, RefFieldName={1}, Width={2}, Height={3}", FieldName, RefFieldName, Width, Height);

            var mediaItem = (MediaItem)imageField.MediaItem;
            if (mediaItem == null)
                return null;

            return mediaItem.FriendlyUrl(Height, Width, Thumbnail);
        }
    }
}
