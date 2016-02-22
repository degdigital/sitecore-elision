using Sitecore.Data;

namespace Elision
{
    public class SitecoreIDs
    {
        public static readonly ID RenderingSectionBaseTemplateId = new ID("{D1592226-3898-4CE2-B190-090FD5F84A4C}");
        public static readonly ID StandardRenderingParametersTemplateId = new ID("{8CA06D6A-B353-44E8-BC31-B528C7306971}");

        public static readonly ID RenderingsFieldId = new ID("{F1A1FE9E-A60C-4DDB-A3A0-BB5B29FE732E}");
        public static readonly ID FinalLayoutField = new ID("{04BF00DB-F5FB-41F7-8AB7-22408372A981}");

#if SITECORE8
        public static readonly ID LayoutFieldId = Sitecore.FieldIDs.FinalLayoutField;
#else
        public static readonly ID LayoutFieldId = Sitecore.FieldIDs.LayoutField;
#endif
    }
}
