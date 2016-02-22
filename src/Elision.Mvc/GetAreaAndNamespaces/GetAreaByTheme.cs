using Elision.Fields;
using Elision.Themes;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Elision.Mvc.GetAreaAndNamespaces
{
    public class GetAreaByTheme : IAreaResolveStrategy
    {
        private readonly IThemeRetriever _themeRetriever;

        public GetAreaByTheme(IThemeRetriever themeRetriever)
        {
            _themeRetriever = themeRetriever;
        }

        public string Resolve(RenderRenderingArgs args)
        {
            return FindAreaByFolder(args);
        }

        public virtual string FindAreaByFolder(RenderRenderingArgs args)
        {
            var theme = _themeRetriever.GetThemeFromContextItem(args.PageContext.Item);
            if (theme == null)
                return null;

            var areaName = theme[ThemeFieldNames.MvcAreaName];

            if (string.IsNullOrWhiteSpace(areaName))
                return null;

            return areaName;
        }
    }
}