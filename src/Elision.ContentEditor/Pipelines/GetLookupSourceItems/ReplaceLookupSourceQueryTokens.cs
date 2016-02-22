using System.Linq;
using System.Text.RegularExpressions;
using Elision.Diagnostics;
using Elision.Fields;
using Elision.Rules;
using Sitecore.Data.Items;
using Sitecore.Pipelines.GetLookupSourceItems;

namespace Elision.ContentEditor.Pipelines.GetLookupSourceItems
{
    public class ReplaceLookupSourceQueryTokens
    {
        private readonly string _tokenPrefix;
        private readonly string _tokenSuffix;
        public ReplaceLookupSourceQueryTokens()
        {
            var rulesSettings = new RulesSettings();
            _tokenPrefix = rulesSettings.QueryTokenPrefix;
            _tokenSuffix = rulesSettings.QueryTokenSuffix;
        }

        public virtual void Process(GetLookupSourceItemsArgs args)
        {
            using (new TraceOperation("Run ReplaceLookupSourceQueryTokens."))
            {
                var query = args.Source;

                query = ReplaceWebsiteToken(query, args.Item);
                query = ReplaceHomeItemToken(query, args.Item);
                query = ReplaceThemeItemToken(query, args.Item);
                query = ReplaceItemFieldValueToken(query, args.Item);

                args.Source = query;
            }
        }

        protected virtual string ReplaceItemFieldValueToken(string query, Item contextItem)
        {
            var tokenPattern = string.Concat(_tokenPrefix, "ItemField:(?<fieldName>[^\\", _tokenSuffix, "]+)", _tokenSuffix);
            return Regex.Replace(query, tokenPattern, x => contextItem[x.Groups["fieldName"].Value], RegexOptions.IgnoreCase);
        }

        protected virtual string ReplaceHomeItemToken(string query, Item contextItem)
        {
            var token = string.Concat(_tokenPrefix, "Home", _tokenSuffix);
            if (!query.Contains(token)) return query;

            var homeItem = contextItem.InheritsFrom(TemplateIDs.HomePageTemplate)
                               ? contextItem
                               : contextItem.Axes.GetAncestors().FirstOrDefault(x => x.InheritsFrom(TemplateIDs.HomePageTemplate));
            if (homeItem == null) return query;

            return query.Replace(token, homeItem.Paths.FullPath);
        }

        protected virtual string ReplaceThemeItemToken(string query, Item contextItem)
        {
            var token = string.Concat(_tokenPrefix, "Theme", _tokenSuffix);
            if (!query.Contains(token)) return query;

            var themeField = contextItem.GetInheritedField(ThemableFieldNames.Theme, true);
            if (themeField == null) return query;

            var themeItem = themeField.Item.Database.ResolveDatasource(themeField.Value, themeField.Item);
            if (themeItem == null) return query;

            return query.Replace(token, themeItem.Paths.FullPath);
        }

        protected virtual string ReplaceWebsiteToken(string query, Item contextItem)
        {
            var token = string.Concat(_tokenPrefix, "Website", _tokenSuffix);
            if (!query.Contains(token)) return query;

            var websiteItem = contextItem.Axes.GetAncestors().Reverse().FirstOrDefault(x => x.InheritsFrom(TemplateIDs.WebsiteFolder));
            if (websiteItem == null) return query;

            return query.Replace(token, websiteItem.Paths.FullPath);
        }
    }
}
