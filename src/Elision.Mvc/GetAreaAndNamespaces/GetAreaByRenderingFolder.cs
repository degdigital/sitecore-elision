using System;
using Elision.Fields;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Elision.Mvc.GetAreaAndNamespaces
{
    public class GetAreaByRenderingFolder : IAreaResolveStrategy
    {
        public string Resolve(RenderRenderingArgs args)
        {
            return FindAreaByFolder(args);
        }

        public virtual string FindAreaByFolder(RenderRenderingArgs args)
        {           
            var renderingItem = args.Rendering.RenderingItem;

            if (renderingItem == null)
                return null;

            var current = renderingItem.InnerItem;
            if (current == null)
                return null;

            var areaName = current.GetInheritedFieldValue(RenderingFolderWithAreaFieldNames.AreaName, true);

            return String.IsNullOrWhiteSpace(areaName)
                       ? null
                       : areaName;
        }
    }
}