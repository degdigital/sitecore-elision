namespace Elision.Web.Models
{
    public class SectionNavigationViewModel
    {
        public NavigableItem RootItem { get; set; }

        public int Levels { get; set; }

        public bool IncludeTopLevel { get; set; }
    }
}