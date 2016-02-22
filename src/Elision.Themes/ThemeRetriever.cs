using System.Linq;
using Elision.Fields;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;

namespace Elision.Themes
{
    public interface IThemeRetriever
    {
        Item GetThemeFromContextItem(Item contextItem);
        string GetThemeScripts(Item theme, ID deviceId, ID resourceLocationId);
        string GetThemeStyles(Item theme, ID deviceId, ID resourceLocationId);
        string GetThemeResources(Item theme, ID deviceId, ID resourceLocationId);
    }

    public class ThemeRetriever : IThemeRetriever
    {
        public Item GetThemeFromContextItem(Item contextItem)
        {
            var themableAncestor = new[] {contextItem}
                .Union(contextItem.Axes.GetAncestors())
                .FirstOrDefault(x => x.InheritsFrom(ThemableFieldIDs.TemplateId) && !string.IsNullOrWhiteSpace(x[ThemableFieldIDs.Theme]));

            if (themableAncestor == null) return null;

            return contextItem.Database.ResolveDatasource(themableAncestor[ThemableFieldIDs.Theme], themableAncestor);
        }

        public string GetThemeResources(Item theme, ID deviceId, ID resourceLocationId)
        {
            return GetThemeStyles(theme, deviceId, resourceLocationId)
                   + "\r\n"
                   + GetThemeScripts(theme, deviceId, resourceLocationId);
        }

        public string GetThemeScripts(Item theme, ID deviceId, ID resourceLocationId)
        {
            var device = deviceId.ToString();
            var location = resourceLocationId.ToString();

            var themeScriptsFolder = theme.Axes.SelectSingleItem("./Scripts");

            return GetThemeResourceCode(themeScriptsFolder, device, location,
                                        "<script type=\"text/javascript\" src=\"{0}\"></script>");
        }

        public string GetThemeStyles(Item theme, ID deviceId, ID resourceLocationId)
        {
            var device = deviceId.ToString();
            var location = resourceLocationId.ToString();

            var themeStylesFolder = theme.Axes.SelectSingleItem("./Stylesheets");
            if (themeStylesFolder == null)
                return "";

            return GetThemeResourceCode(themeStylesFolder, device, location,
                                        "<link rel=\"stylesheet\" type=\"text/css\" href=\"{0}\" />");
        }

        protected virtual string GetThemeResourceCode(Item resourceFolder, string device, string location, string linkFormat)
        {
            if (resourceFolder == null)
                return "";

            var scriptsByDevice = resourceFolder
                .GetChildren(ChildListOptions.IgnoreSecurity)
                .Where(x => string.IsNullOrWhiteSpace(x[ThemeResourceFieldIDs.SupportedDevices])
                            || x[ThemeResourceFieldIDs.SupportedDevices].Contains(device))
                .Where(MatchesCurrentPageMode);

            var scriptsForOutputLocation = scriptsByDevice.Where(x => x[ThemeResourceFieldIDs.ResourceLocation] == location);

            var scriptCodes = scriptsForOutputLocation
                .Select(x => x.InheritsFrom(ThemeLinkedResourceFieldIDs.TemplateId)
                                 ? string.Format(linkFormat, GetResourceUrl(x.Fields[ThemeLinkedResourceFieldIDs.ResourceLink]))
                                 : x[ThemeEmbeddedResourceFieldIDs.ResourceCode]
                );

            return string.Join("\r\n", scriptCodes);
        }

        protected virtual bool MatchesCurrentPageMode(Item resourceItem)
        {
            var selectedModes = resourceItem.GetLinkedItems(ThemeResourceFieldNames.PageModes).ToArray();
            if (!selectedModes.Any()) return true;

            foreach (var selectedMode in selectedModes.Select(x => x.Name))
            {
                if (selectedMode == "IsNormal" && Sitecore.Context.PageMode.IsNormal)
                    return true;
                if (selectedMode == "IsPreview" && Sitecore.Context.PageMode.IsPreview)
                    return true;
                if ((selectedMode == "IsPageEditor" || selectedMode == "IsExperienceEditor") && Sitecore.Context.PageMode.IsExperienceEditor)
                    return true;
                if ((selectedMode == "IsPageEditorEditing" || selectedMode == "IsExperienceEditorEditing") && Sitecore.Context.PageMode.IsExperienceEditorEditing)
                    return true;
            }
            return false;
        }

        protected virtual string GetResourceUrl(LinkField linkField)
        {
            if (linkField == null || string.IsNullOrWhiteSpace(linkField.Value))
                return null;

            return linkField.GetFriendlyUrl();
        }
    }
}
