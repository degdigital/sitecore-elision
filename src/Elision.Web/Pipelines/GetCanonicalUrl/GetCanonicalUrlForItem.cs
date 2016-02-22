using Elision.Fields;

namespace Elision.Web.Pipelines.GetCanonicalUrl
{
    public class GetCanonicalUrlForItem : IGetCanonicalUrlProcessor
    {
        public void Process(GetCanonicalUrlArgs args)
        {
            if (!string.IsNullOrWhiteSpace(args.CanonicalUrl))
                return;

            if (args.PageItem == null)
                return;

            var itemCanonical = args.PageItem.Fields.GetValue(PageMetaFieldsFieldIDs.CanonicalUrl);
            if (!string.IsNullOrWhiteSpace(itemCanonical))
                args.CanonicalUrl = itemCanonical;
        }
    }
}