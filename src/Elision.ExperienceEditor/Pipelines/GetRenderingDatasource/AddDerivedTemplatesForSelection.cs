using System.Linq;
using Elision.Diagnostics;
using Sitecore.Data.Managers;
using Sitecore.Pipelines.GetRenderingDatasource;

namespace Elision.ExperienceEditor.Pipelines.GetRenderingDatasource
{
    public class AddDerivedTemplatesForSelection
    {
        public void Process(GetRenderingDatasourceArgs args)
        {
            var db = args.Prototype == null ? args.ContentDatabase : args.Prototype.Database;

            using (new TraceOperation(string.Format("Add derived templates for selection in {0} rendering datasource dialog.", args.RenderingItem == null ? "rendering" : args.RenderingItem.Name)))
            {
                var index = 0;
                while (index < args.TemplatesForSelection.Count)
                {
                    var template = args.TemplatesForSelection[index];

                    var allDerived = db.SelectItems("/sitecore/templates//*[contains(@#__Base template#, '" + template.ID + "')]");

                    foreach (var newTemplate in allDerived.Select(TemplateManager.GetTemplate))
                    {
                        if (!args.TemplatesForSelection.Contains(newTemplate))
                            args.TemplatesForSelection.Add(newTemplate);
                    }

                    index++;
                }
            }
        }
    }
}
