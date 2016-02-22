using Sitecore.Data;

namespace Elision
{
    public static class PlaceholderIDs
    {
        public const string PageHeadIdString = "{8082C7B0-CEB3-4C8A-8246-C02B3FD7680F}";
        public const string PageBodyIdString = "{C505FAF7-3947-4863-87B9-668E688B3FCF}";
        public const string PageFootIdString = "{FC6E5C2C-F882-4D83-B281-518757F51215}";
        public static ID PageHead = new ID(PageHeadIdString);
        public static ID PageBody = new ID(PageBodyIdString);
        public static ID PageFoot = new ID(PageFootIdString);
    }
}