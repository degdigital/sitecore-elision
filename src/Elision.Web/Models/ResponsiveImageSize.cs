using Elision.Fields;
using Sitecore.Data.Items;

namespace Elision.Web.Models
{
    public class ResponsiveImageSize : CustomItem
    {
        public ResponsiveImageSize(Item innerItem) : base(innerItem) { }

        public int ImageWidth
        {
            get
            {
                int parsed;
                return int.TryParse(InnerItem[ResponsiveImageSizeFieldIDs.ImageWidth], out parsed) ? parsed : 0;
            }
        }

        public string WindowMinWidth { get { return InnerItem[ResponsiveImageSizeFieldIDs.WindowMinWidth]; } }
    }
}
