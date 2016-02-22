namespace Elision.Web.Pipelines.GetCanonicalUrl
{
    public interface IGetCanonicalUrlProcessor
    {
        void Process(GetCanonicalUrlArgs args);
    }
}