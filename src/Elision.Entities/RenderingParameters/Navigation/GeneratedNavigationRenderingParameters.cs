using Elision.Entities.ParameterFields.Design;
using Elision.Entities.ParameterFields.Navigation;

namespace Elision.Entities.RenderingParameters.Navigation
{
    public class GeneratedNavigationRenderingParameters : INavigationLevelsParameters, INavigationCssClassParameters, IClassParameters
    {
        public int NavigationLevels { get; set; }
        public bool IncludeTopLevel { get; set; }

        public string CssClass { get; set; }

        public string TopLevelListClass { get; set; }
        public string TopLevelListItemClass { get; set; }
        public string TopLevelLinkClass { get; set; }
        public string ChildListClass { get; set; }
        public string ChildListItemClass { get; set; }
        public string ChildLinkClass { get; set; }
    }
}
