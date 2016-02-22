using Sitecore.Data;

namespace Elision
{
    public static class RenderingIDs
    {
        public const string TwoColumnWideRightPageLayoutIdString = "{C8E28128-2955-4C4B-B61C-6613EFAAAAFC}";
        public const string ThreeColumnWideMiddlePageLayoutIdString = "{93BF63F0-E2D4-45BD-8EA4-85DAA244D979}";
        public const string ThreeColumnEqualPageLayoutIdString = "{843B03C1-A206-4D32-AF88-CB47DA1E7BC7}";
        public const string TwoColumnWideLeftPageLayoutIdString = "{C9C5FD33-A47F-40B0-8E52-582940CF7E79}";
        public const string TwoColumnEqualPageLayoutIdString = "{9B37ED94-6A81-46DC-A155-92D8A4704806}";
        public const string OneColumnPageLayoutIdString = "{EA7A7183-84E1-46DA-972F-ED53E11A98D1}";
        public const string FourColumnEqualPageLayoutIdString = "{E2F7D6F6-F55E-4077-AF00-2CB2826D0E2F}";
        public const string NavigationBreadcrumbsIdString = "{A0E20C17-B66E-415A-A990-ED75D189EF5F}";

        public static ID TwoColumnWideRightPageLayout = new ID(TwoColumnWideRightPageLayoutIdString);
        public static ID ThreeColumnWideMiddlePageLayout = new ID(ThreeColumnWideMiddlePageLayoutIdString);
        public static ID ThreeColumnEqualPageLayout = new ID(ThreeColumnEqualPageLayoutIdString);
        public static ID TwoColumnWideLeftPageLayout = new ID(TwoColumnWideLeftPageLayoutIdString);
        public static ID TwoColumnEqualPageLayout = new ID(TwoColumnEqualPageLayoutIdString);
        public static ID OneColumnPageLayout = new ID(OneColumnPageLayoutIdString);
        public static ID FourColumnEqualPageLayout = new ID(FourColumnEqualPageLayoutIdString);

        public static ID NavigationBreadcrumbs = new ID(NavigationBreadcrumbsIdString);
        public static ID NavigationPrimary = new ID("{2246F5D8-178F-4A36-B3A8-E4C4658163D0}");
        public static ID NavigationSection = new ID("{85986073-631A-4B99-AAC4-E96093EA724F}");

        public static ID PageMetadata = new ID("{459D9770-0CDE-4865-9E46-6A73836A9BB1}");

        public static ID HeadResources = new ID("{A1F97BCC-0F5A-4A9B-AA67-0F22E23268AA}");
        public static ID BodyTopResources = new ID("{176AC3B8-2AA5-4811-8BD3-FAA212881824}");
        public static ID BodyBottomResources = new ID("{081E0D0C-D55F-42A1-A5D7-A9F3A975153C}");

        public static ID FragmentRendering = new ID("{29CE8EA8-26B3-4638-B7E7-31A76E99DAE4}");
        public static ID ResponsiveImageRendering = new ID("{40CEB3F8-E496-48F1-9F58-DA9D4F7C2E5D}");

        public static ID TwitterCardMetadata = new ID("{422BF485-564D-460D-8F7A-E708C4DA9118}");
    }
}