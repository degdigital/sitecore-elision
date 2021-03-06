using Elision.Mvc.WebEdit;

namespace Elision.ExperienceEditor
{
    public class AddRenderingWrapper : Sitecore.Mvc.ExperienceEditor.Pipelines.Response.RenderRendering.AddWrapper
    {
        public override void Process(Sitecore.Mvc.Pipelines.Response.RenderRendering.RenderRenderingArgs args)
        {
            if (WebEditStateSwitcher.CurrentValue == WebEditState.Disabled)
                return;

            if (args.Rendering.Parameters["disablewebedit"] == "1")
            {
                args.Disposables.Add(new WebEditDisabler());
                return;
            }

            base.Process(args);
        }
    }
}
