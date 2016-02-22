using System.Collections.Generic;
using System.Web;
using Elision.Fields;
using Elision.Web.Models;
using Elision.Web.Pipelines.GetCanonicalUrl;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;
using Sitecore.Pipelines;
using Sitecore.Resources.Media;

namespace Elision.Web.ModelBuilders
{
    public interface IPageMetadataModelBuilder
    {
        PageMetadataModel Build(Item item);
    }

    public class PageMetadataModelBuilder : IPageMetadataModelBuilder
    {
        public PageMetadataModel Build(Item item)
        {
            var urlOptions = (UrlOptions)UrlOptions.DefaultOptions.Clone();
            urlOptions.AlwaysIncludeServerUrl = true;
            urlOptions.LanguageEmbedding = LanguageEmbedding.Never;

            var model = new PageMetadataModel
                {
                    BrowserTitle = item.Fields.GetValue(PageMetaFieldsFieldIDs.BrowserTitle)
                                       .Or(item.DisplayName).Or(item.Name),

                    OgSiteName = item.Fields.GetValue(OpenGraphMetaFieldIDs.OgSiteName),
                    OgType = item.Fields.GetValue(OpenGraphMetaFieldIDs.OgType),
                    OgTitle = item.Fields.GetValue(OpenGraphMetaFieldIDs.OgTitle),
                    OgDescription = item.Fields.GetValue(OpenGraphMetaFieldIDs.OgDescription),

                    MetaKeywords = item.Fields.GetValue(PageMetaFieldsFieldIDs.MetaKeywords),
                    MetaDescription = item.Fields.GetValue(PageMetaFieldsFieldIDs.MetaDescription),

                    Language = item.Language.CultureInfo.Name,
                    LastModified = item.Statistics.Updated,
                    Url = LinkManager.GetItemUrl(item, urlOptions)
                };

            model.CanonicalUrl = GetCanonicalUrl(item);
            model.RobotsMeta = GetRobotsMeta(item);
            model.OgImage = GetOgImage(item);

            return model;
        }

        private static OgImageModel GetOgImage(Item item)
        {
            var ogImageField = (ImageField) item.Fields[OpenGraphMetaFieldIDs.OgImage];
            if (ogImageField == null) 
                return null;

            var mediaItem = (MediaItem) ogImageField.MediaItem;
            if (mediaItem == null)
                return null;

            int size;
            var mediaUrlOptions = MediaUrlOptions.Empty;
            mediaUrlOptions.AbsolutePath = true;
            mediaUrlOptions.AlwaysIncludeServerUrl = true;

            return new OgImageModel
                {
                    Url = MediaManager.GetMediaUrl(mediaItem, mediaUrlOptions),
                    MimeType = mediaItem.MimeType,
                    Width = int.TryParse(ogImageField.Width, out size) ? size : 0,
                    Height = int.TryParse(ogImageField.Height, out size) ? size : 0
                };
        }

        private string GetRobotsMeta(Item pageItem)
        {
            var flags = new List<string>();

            if (pageItem.Fields.GetValue(PageMetaFieldsFieldIDs.BlockSearchEngineIndexing) == "1")
                flags.Add("noindex");
            if (pageItem.Fields.GetValue(PageMetaFieldsFieldIDs.BlockSearchEngineLinkFollowing) == "1")
                flags.Add("nofollow");

            return string.Join(",", flags);
        }

        private string GetCanonicalUrl(Item pageItem)
        {
            var rawUrl = "";
            if (HttpContext.Current != null)
                rawUrl = HttpContext.Current.Request.RawUrl;

            var getCanonicalUrlArgs = new GetCanonicalUrlArgs(pageItem, rawUrl);
            CorePipeline.Run("getCanonicalUrl", getCanonicalUrlArgs);
            return getCanonicalUrlArgs.CanonicalUrl;
        }
    }
}