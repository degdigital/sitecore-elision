using System.Collections.Generic;

namespace Elision.Web.Models
{
    public class MainNavigationViewModel
    {
        public IEnumerable<NavigableItem> MenuItems { get; set; }
        public int Levels { get; set; }

        public string CssClass { get; set; }

        public string TopLevelListClass { get; set; }
        public string TopLevelListItemClass { get; set; }
        public string TopLevelLinkClass { get; set; }
        public string ChildListClass { get; set; }
        public string ChildListItemClass { get; set; }
        public string ChildLinkClass { get; set; }
    }
}
