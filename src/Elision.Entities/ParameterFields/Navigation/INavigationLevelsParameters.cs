namespace Elision.Entities.ParameterFields.Navigation
{
    public interface INavigationLevelsParameters
    {
        int NavigationLevels { get; set; }
        bool IncludeTopLevel { get; set; }
    }
}
