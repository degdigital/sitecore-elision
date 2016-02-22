using System;
using System.Linq;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Web;

namespace Elision
{
    public static class ItemInformationExtensions
    {
        public static bool InheritsFrom(this Item item, ID templateId)
        {
            return item.TemplateID == templateId
                   || item.Template.InheritsFrom(templateId);
        }

        public static bool InheritsFrom(this TemplateItem template, ID templateId)
        {
            return template.ID == templateId
                   || template.BaseTemplates
                              .Where(x => x.ID != template.ID)
                              .Any(x => x.InheritsFrom(templateId));
        }

        public static string GetValue(this FieldCollection fields, ID fieldId)
        {
            var field = fields[fieldId];
            return field == null ? String.Empty : field.Value;
        }

        public static Field GetInheritedField(this Item item, string fieldName, bool skipEmptyFields = false)
        {
            if (item == null)
                return null;

            var field = item.Fields[fieldName];
            if (field != null && skipEmptyFields && string.IsNullOrWhiteSpace(field.Value))
                field = null;

            return field ?? item.Parent.GetInheritedField(fieldName, skipEmptyFields);
        }

        public static string GetInheritedFieldValue(this Item item, string fieldName, bool skipEmptyValues = false)
        {
            var field = item.GetInheritedField(fieldName, skipEmptyValues);
            return field == null
                       ? null
                       : field.Value;
        }

        public static SiteInfo GetSite(this Item item)
        {
            var siteInfoList = Sitecore.Sites.SiteContextFactory.Sites
                                       .OrderByDescending(x => (x.RootPath + x.StartItem).Length);

            var itemPath = item.Paths.FullPath;
            return siteInfoList
                .FirstOrDefault(site => itemPath.StartsWith(site.RootPath + site.StartItem, StringComparison.InvariantCultureIgnoreCase));
        }
    }
}
