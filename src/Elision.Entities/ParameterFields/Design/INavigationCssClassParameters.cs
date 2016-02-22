namespace Elision.Entities.ParameterFields.Design
{
    public interface INavigationCssClassParameters
    {
        string TopLevelListClass { get; set; }
        string TopLevelListItemClass { get; set; }
        string TopLevelLinkClass { get; set; }
        string ChildListClass { get; set; }
        string ChildListItemClass { get; set; }
        string ChildLinkClass { get; set; }
    }
}
