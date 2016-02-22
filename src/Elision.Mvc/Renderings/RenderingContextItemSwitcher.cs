using System;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.Mvc.Presentation;

namespace Elision.Mvc
{
    public class RenderingContextItemSwitcher : IDisposable
    {
        private readonly RenderingContext _context;
        private readonly Item _oldItem;

        public RenderingContextItemSwitcher(Item item)
        {
            Assert.IsNotNull(item, "item");
            _context = RenderingContext.CurrentOrNull;
            if (_context == null)
            {
                throw new InvalidOperationException(Sitecore.StringExtensions.StringExtensions.FormatWith("The context item can only be changed inside a {0}", (object)typeof(RenderingContext)));
            }
            _oldItem = _context.ContextItem;
            _context.ContextItem = item;
        }

        public void Dispose()
        {
            _context.ContextItem = _oldItem;
        }
    }
}
