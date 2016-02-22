using System.Collections.Generic;

namespace Elision.Web.Models
{
    public class BreadcrumbsModel
    {
        public Stack<NavigableItem> Crumbs { get; protected set; }

        public BreadcrumbsModel()
        {
            Crumbs = new Stack<NavigableItem>();
        }
    }
}