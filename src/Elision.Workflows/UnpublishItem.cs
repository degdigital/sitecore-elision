using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Workflows.Simple;

namespace Elision.Workflows
{
    public class UnpublishItem : Sitecore.Workflows.Simple.PublishAction
    {
        public new void Process(WorkflowPipelineArgs args)
        {
            Assert.ArgumentNotNull(args, "args");
            if (args.DataItem == null)
                return;

            using (new EditContext(args.DataItem))
            {
                args.DataItem.Publishing.NeverPublish = true;
            }
            base.Process(args);
        }
    }
}
