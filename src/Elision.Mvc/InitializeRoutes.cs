using System.Web.Mvc;
using System.Web.Routing;
using Sitecore.Pipelines;

namespace Elision.Mvc
{
    public class InitializeRoutes : Sitecore.Mvc.Pipelines.Loader.InitializeRoutes
    {
        public override void Process(PipelineArgs args)
        {
            RouteTable.Routes.MapMvcAttributeRoutes();
            base.Process(args);
        }
    }
}
