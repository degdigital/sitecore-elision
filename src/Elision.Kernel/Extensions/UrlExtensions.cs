using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Resources.Media;

namespace Elision
{
    public static class UrlExtensions
    {
        public static string AbsoluteUrl(this Item item)
        {
            if (item == null) return string.Empty;

            var options = (UrlOptions)UrlOptions.DefaultOptions.Clone();
            options.AlwaysIncludeServerUrl = true;
            options.SiteResolving = true;
            return LinkManager.GetItemUrl(item, options);
        }

        public static string FriendlyUrl(this MediaItem mediaItem, int? height = null, int? width = null,
                                         bool? thumbnail = null, bool? includeServerUrl = null)
        {
            if (mediaItem == null) return string.Empty;

            var options = new MediaUrlOptions
            {
                AlwaysIncludeServerUrl = includeServerUrl.GetValueOrDefault(false),
                VirtualFolder = "/",
                LowercaseUrls = true,
                IncludeExtension = true,
                UseItemPath = true
            };
            if (height.HasValue) options.Height = height.Value;
            if (width.HasValue) options.Width = width.Value;
            if (thumbnail.HasValue) options.Thumbnail = thumbnail.Value;

            var url = MediaManager.GetMediaUrl(mediaItem, options);
            return url;
        }

        public static string AbsoluteMediaUrl(this MediaItem mediaItem, int? height = null, int? width = null, bool? thumbnail = null)
        {
            return mediaItem.FriendlyUrl(height, width, thumbnail, true);
        }

    }
}
