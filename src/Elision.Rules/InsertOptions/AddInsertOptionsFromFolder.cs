using Elision.Data;
using Sitecore.Collections;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Rules.Actions;
using Sitecore.Rules.InsertOptions;

namespace Elision.Rules.InsertOptions
{
    public class AddInsertOptionsFromFolder<T> : RuleAction<T> where T : InsertOptionsRuleContext
    {
        public string TemplateFolderId { get; set; }

        public override void Apply(T ruleContext)
        {
            if (string.IsNullOrWhiteSpace(TemplateFolderId) || !ID.IsID(TemplateFolderId))
                return;

            var templateFolder = ruleContext.Item.Database.GetItem(TemplateFolderId);
            if (templateFolder == null)
                return;

            AddInsertOptions(templateFolder, ruleContext);
        }

        protected virtual void AddInsertOptions(Item item, T ruleContext)
        {
            if (item.InheritsFrom(Sitecore.TemplateIDs.Template) ||
                item.InheritsFrom(Sitecore.TemplateIDs.BranchTemplate))
            {
                if (!ruleContext.InsertOptions.Contains(item))
                    ruleContext.InsertOptions.Add(item);
            }
            else
            {
                foreach (Item child in item.GetChildren(ChildListOptions.IgnoreSecurity | ChildListOptions.SkipSorting))
                {
                    AddInsertOptions(child, ruleContext);
                }
            }
        }
    }
}
