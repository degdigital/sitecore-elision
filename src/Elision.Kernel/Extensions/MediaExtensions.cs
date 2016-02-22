using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Resources.Media;

namespace Elision
{
    public static class MediaExtensions
    {
        public static string GetMediaUrl(this Item item, string fieldName, int? width = null)
        {
            var imageField = (ImageField) item.Fields[fieldName];
            if (imageField == null) return null;

            var mediaItem = (MediaItem) imageField.MediaItem;
            if (mediaItem == null) return null;

            var options = MediaUrlOptions.Empty;
            if (width.HasValue)
                options.Width = width.Value;

            return MediaManager.GetMediaUrl(mediaItem, options);
        }
    }
}
