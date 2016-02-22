---
title: CMS Managed SynonymAnalyzer for ContentSearch
---
## What problem does this solve?
When using Sitecore's `ContentSearch` namespace, keyword filtering just does simple text matching against the tokens in a content field. It does not consider word variations like pluralization, nor does it consider different words with the same meaning.

To resolve this concern, Sitecore ships with a SynonymAnalyzer. It works well, but the keyword synonyms can only be configured with a static XML file on the server. This causes a couple of problems: first of all, changing the synonym groups requires modifying a file on the server. Even if that were acceptable, this file would need to be maintained across all Content Management and Content Delivery servers.

Elision contains a SynonymEngine that will read synonym groups from items in the Sitecore database, eliminating the need for file deployments to update synonym groups, and giving more flexibility to site admins or content editors.

## How do I use it?
1. Use the "Search Synonyms Folder" branch template to create a folder in Sitecore that will hold the synonym groups for a single search index.
  * This folder can be placed anywhere in the content tree.
  * A search synonyms folder can be shared across multiple indexes.
1. Update the index configuration to use Sitecore's `SynonymAnalyzer`, and Elision's `SitecoreSynonymEngine`.
  * An example config patch snippet can be found in the `Elision.Search.config` file.
1. Rebuild all affected indexes

All of the synonym groups are cached on first request. The cache can be monitored from Sitecore's cache admin tool (/sitecore/admin/cache.aspx). It can also be cleared from that admin screen. Otherwise, the cache is automatically cleared when a change is made to any item that inherits from the `SearchSynonyms` template.

## How does it work?
Elision does not implmenent a full analyzer, it only implements `ISynonymEngine`, which is used by the Lucene SynonymAnalyzer that ships with Sitecore. The analyzer requests all of the known synonyms for a given keyword. Elision's `SitecoreSynonymEngine` will look up the requested term to see if it matches any keywords in any of the configured synonym groups. If found, it will provide the list of matching synonyms.

## How do I disable it?
There is a small amount of overhead to check and clear the synonym cache on item save, and Sitecore's SynonymAnalyzer is slightly slower than the default lucene analyzer. If you will not be using this functionality, then you can completely disable it by following these steps:
* Edit the `Elision.Search.config` file to remove all references to `SearchSynonymCacheClearer` and `SitecoreSynonymEngine`
* Update any configured indexes to remove references to Sitecore's `SynonymAnalyzer` and Elision's `SitecoreSynonymEngine`
* Remove any existing Search Synonym Folders from Sitecore

## Credit
This feature started as a requirement for a client project. The original implementation was done by [Patrick Delancy](http://patrickdelancy.com/) with the assistance of [this article](http://firebreaksice.com/sitecore-synonym-search-with-lucene/) by Mark Ursino.
