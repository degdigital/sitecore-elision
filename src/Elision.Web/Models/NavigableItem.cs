using System.Collections.Generic;
using System.Linq;
using System.Web;
using Elision.Fields;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Links;

namespace Elision.Web.Models
{
    public class NavigableItem : CustomItem
    {
        protected readonly Item ContextSitePage;
        public NavigableItem(Item innerItem) : this(innerItem, Sitecore.Context.Item) { }
        public NavigableItem(Item innerItem, Item contextSitePage) : base(innerItem)
        {
            ContextSitePage = contextSitePage;
        }

        protected HtmlString _navigationText { get; set; }
        public HtmlString NavigationText
        {
            get
            {
                if (_navigationText == null)
                {
                    var navText = InnerItem.Fields.GetValue(NavigableFieldIDs.NavigationText);
                    if (string.IsNullOrWhiteSpace(navText))
                    {
                        var linkField = (LinkField)InnerItem.Fields[MenuLinkFieldIDs.Link];
                        navText = linkField != null && linkField.HasValue()
                                      ? linkField.Text
                                      : null;
                    }
                    _navigationText = new HtmlString(navText
                                              .Or(InnerItem.DisplayName)
                                              .Or(InnerItem.Name));                    
                }
                return _navigationText;
            }
            set
            {
                if (!string.IsNullOrWhiteSpace(value?.ToString()))
                    _navigationText = value;
            }
        }
        public bool IsActive { get { return InnerItem != null && ContextSitePage != null && InnerItem.ID == ContextSitePage.ID; } }
        public bool IsUnderActiveItem { get { return InnerItem.Axes.IsDescendantOf(ContextSitePage); } }

        public virtual string Url
        {
            get
            {
                return LinkUrl ?? LinkManager.GetItemUrl(InnerItem);
            }
        }

        private string LinkUrl
        {
            get
            {
                var linkField = (LinkField) InnerItem.Fields[MenuLinkFieldIDs.Link];
                return linkField != null && linkField.HasValue()
                           ? linkField.GetFriendlyUrl()
                           : null;
            }
        }

        public bool ShowInPrimaryMenu { get { return InnerItem.Fields.GetValue(NavigableFieldIDs.ShowInPrimaryNavigation) == "1"; } }
        public bool ShowInSectionMenu { get { return InnerItem.Fields.GetValue(NavigableFieldIDs.ShowInInteriorNavigation) == "1"; } }

        public virtual IEnumerable<NavigableItem> PrimaryNavChildren
        {
            get
            {
                return InnerItem.GetChildren()
                                .Where(x => x != null)
                                .Select(x => new NavigableItem(x, ContextSitePage))
                                .Where(x => x.ShowInPrimaryMenu)
                                .Where(x =>
                                       !string.IsNullOrWhiteSpace(x.LinkUrl)
                                       ||
                                       (Sitecore.Context.Device != null && x.InnerItem != null && x.InnerItem.HasLayout(Sitecore.Context.Device)));
            }
        }

        protected virtual IEnumerable<NavigableItem> _sectionNavChildren { get; set; }
        public virtual IEnumerable<NavigableItem> SectionNavChildren
        {
            get
            {
                if (_sectionNavChildren == null)
                {
                    _sectionNavChildren = InnerItem.GetChildren()
                                                   .Where(x => x != null)
                                                   .Select(x => new NavigableItem(x, ContextSitePage))
                                                   .Where(x => x.ShowInSectionMenu);
                }
                return _sectionNavChildren;
            }
            set { _sectionNavChildren = value; }
        }
        public NavigableItem Parent { get { return InnerItem.Parent == null ? null : new NavigableItem(InnerItem.Parent); } }

        public string CssClass
        {
            get
            {
                var linkField = (LinkField) InnerItem.Fields[MenuLinkFieldIDs.Link];
                return linkField != null && linkField.HasValue()
                           ? linkField.Class
                           : null;
            }
        }
        public string Target
        {
            get
            {
                var linkField = (LinkField)InnerItem.Fields[MenuLinkFieldIDs.Link];
                return linkField != null && linkField.HasValue()
                           ? linkField.Target
                           : null;
            }
        }
    }
}
