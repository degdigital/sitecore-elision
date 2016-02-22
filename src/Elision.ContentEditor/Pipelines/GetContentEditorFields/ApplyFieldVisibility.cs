using Sitecore.Data.Fields;
using Sitecore.Data.Templates;
using Sitecore.Shell.Applications.ContentEditor.Pipelines.GetContentEditorFields;

namespace Elision.ContentEditor.Pipelines.GetContentEditorFields
{
    public class ApplyFieldVisibility : GetFields
    {
        protected override bool CanShowField(Field field, TemplateField templateField)
        {
            if (ShowDataFieldsOnly)
            {
                var templateFieldItem = field.Database.GetItem(templateField.ID);
                var hideWithStandardFields =
                    (templateFieldItem != null
                     && templateFieldItem.Fields[TemplateFieldIDs.HideWithStandardFields] != null
                     && templateFieldItem.Fields[TemplateFieldIDs.HideWithStandardFields].Value == "1");

                if (hideWithStandardFields)
                    return false;
            }
            return base.CanShowField(field, templateField);
        }
    }
}