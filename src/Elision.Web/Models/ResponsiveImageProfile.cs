using System.Collections.Generic;
using System.Linq;
using Elision.Fields;
using Sitecore.Data.Items;

namespace Elision.Web.Models
{
    public class ResponsiveImageProfile : CustomItem
    {
        public string DefaultImageSizeId
        {
            get { return (InnerItem.Fields[ResponsiveImageProfileFieldIDs.DefaultImageSize] == null ? null : InnerItem.Fields[ResponsiveImageProfileFieldIDs.DefaultImageSize].Value); }
        }

        public ResponsiveImageSize DefaultImageSize
        {
            get
            {
                return ImageSizes.ContainsKey(DefaultImageSizeId)
                           ? ImageSizes[DefaultImageSizeId]
                           : ImageSizes.Values.FirstOrDefault();
            }
        }

        private Dictionary<string, ResponsiveImageSize> _imageSizes;
        public Dictionary<string, ResponsiveImageSize> ImageSizes
        {
            get
            {
                if (_imageSizes == null)
                {
                    _imageSizes = InnerItem
                        .GetChildren()
                        .ToDictionary(x => x.ID.ToString(), x => new ResponsiveImageSize(x));
                }
                return _imageSizes;
            }
        }

        public ResponsiveImageProfile(Item innerItem) : base(innerItem) { }
    }
}