using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Mvc.Presentation;

namespace Elision.Entities.RenderingParameters
{
    public class RenderingActionArgs
    {
        public Database Database { get; set; }

        public Rendering Rendering { get; set; }
        public PageContext PageContext { get; set; }

        public string RenderingDataSource { get; set; }
        public Item RenderingContextItem { get; set; }

        private Item _datasourceItem;
        public Item DatasourceItem
        {
            get
            {
                if (_datasourceItem == null && Database != null && !string.IsNullOrWhiteSpace(RenderingDataSource))
                    _datasourceItem = Database.ResolveDatasource(RenderingDataSource, PageContext == null ? null : PageContext.Item);

                return _datasourceItem;
            }
            set { _datasourceItem = value; }
        }
    }
}