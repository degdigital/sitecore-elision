using System.Web;
using Elision.Data;
using Elision.Themes;
using Elision.Web.Models;
using Sitecore.Data;
using Sitecore.Data.Items;

namespace Elision.Web.ModelBuilders
{
    public interface ISiteResourceModelBuilder
    {
        SiteResourceViewModel Build(Item renderingContextItem, ID resourceLocationId, ID deviceId, string siteScriptFieldName, string pageScriptFieldName);
    }

    public class SiteResourceModelBuilder : ISiteResourceModelBuilder
    {
        private readonly IThemeRetriever _themeRetriever;

        public SiteResourceModelBuilder(IThemeRetriever themeRetriever)
        {
            _themeRetriever = themeRetriever;
        }

        public SiteResourceViewModel Build(Item renderingContextItem, ID resourceLocationId, ID deviceId, string siteScriptFieldName, string pageScriptFieldName)
        {
            var model = new SiteResourceViewModel();

            if (!string.IsNullOrWhiteSpace(siteScriptFieldName)) {
                var home = SiteContext.GetHomeItem();
                if (home != null)
                    model.SiteResources = new HtmlString(home[siteScriptFieldName] ?? "");
            }

            if (renderingContextItem != null)
            {
                if (!string.IsNullOrWhiteSpace(pageScriptFieldName))
                {
                    model.PageResources = new HtmlString(renderingContextItem[pageScriptFieldName] ?? "");
                }

                var theme = _themeRetriever.GetThemeFromContextItem(renderingContextItem);
                model.ThemeResources = new HtmlString(theme == null
                                                          ? ""
                                                          : _themeRetriever.GetThemeResources(theme, deviceId,
                                                                                              resourceLocationId)
                    );
            }

            return model;
        }
    }
}