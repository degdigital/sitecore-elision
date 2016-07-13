﻿using System.Collections.Generic;
using Sitecore.Data.Items;
using Sitecore.Pipelines;

namespace Elision.Sitemap.GenerateSitemapXml
{
    public class GenerateSitemapArgs : PipelineArgs
    {
        public Item RootItem { get; set; }
        public IEnumerable<Item> Items { get; set; } = new Item[0];
        public List<string> SitemapFiles { get; set; } = new List<string>();

        public string CacheKeyBase { get; set; }

        public string Content { get; set; }

        public string RequestUrl { get; set; }
    }
}
