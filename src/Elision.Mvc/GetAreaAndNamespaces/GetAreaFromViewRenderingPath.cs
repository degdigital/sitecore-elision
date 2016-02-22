using System.Text.RegularExpressions;
using Sitecore.Mvc.Pipelines.Response.RenderRendering;

namespace Elision.Mvc.GetAreaAndNamespaces
{
    public class GetAreaFromViewRenderingPath : IAreaResolveStrategy
    {
        public string Resolve(RenderRenderingArgs args)
        {
            if (SkipProcessor(args))
                return null;

            return FindAreaByPath(args);
        }

        public virtual string FindAreaByPath(RenderRenderingArgs args)
        {
            var path = args.Rendering["ViewPath"];
            var areaName = GetAreaName(path);

            return string.IsNullOrWhiteSpace(areaName) 
                ? null 
                : areaName;
        }

        public virtual string GetAreaName(string renderingPath)
        {
            var m = Regex.Match(renderingPath,
                                @"\/areas\/(?<areaname>[^\s\/]*)\/views\/(\/[\w\s\-\+]+\/)*(.*\.(cshtml|ascx)$)",
                                RegexOptions.IgnoreCase);

            return m.Success ? m.Groups["areaname"].Value : null;
        }

        public bool SkipProcessor(RenderRenderingArgs args)
        {
            if (args.Rendering.RenderingType != "ViewRenderer")
                return true;
            return string.IsNullOrWhiteSpace(args.Rendering["ViewPath"]);
        }
    }
}