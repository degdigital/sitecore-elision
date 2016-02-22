using Sitecore;
using Sitecore.Data;
using Sitecore.Diagnostics;

namespace Elision.Rules.GetRenderingDatasource
{
    public class RemoveGlobalDatasourceRootAction<T> : RemoveDatasourceRootAction<T> where T : GetRenderingDatasourceRuleContext
    {
        public override void Apply(T ruleContext)
        {
            var contextItem = ruleContext.Args.RenderingItem.Database.GetItem(ruleContext.Args.ContextItemPath);
            if (contextItem == null)
                return;

            contextItem = contextItem.Database.GetItem(ItemIDs.ContentRoot);
            if (contextItem == null)
                return;

            var folderPath = ruleContext.Settings.GlobalDatasourceFolderPath;
            if (ruleContext.Settings.GlobalDatasourceFolderNesting)
            {
                var dsTemplateId = ruleContext.Args.RenderingItem["Datasource template"];
                var dsTemplate = ID.IsID(dsTemplateId)
                    ? ruleContext.Args.ContentDatabase.GetItem(ID.Parse(dsTemplateId))
                    : ruleContext.Args.ContentDatabase.GetItem(dsTemplateId);
                if (dsTemplate == null)
                    Log.SingleWarn(string.Format("Unable to resolve datasource template when setting datasource roots for rendering {0} ({1}).", ruleContext.Args.RenderingItem.DisplayName, ruleContext.Args.RenderingItem.ID), this);
                else
                    folderPath += "/" + dsTemplate.Name;
            }

            var dsFolder = contextItem.Axes.SelectSingleItem(folderPath);
            if (dsFolder == null) return;

            DatasourceRootId = dsFolder.ID.ToString();
            base.Apply(ruleContext);
        }
    }
}
