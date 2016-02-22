---
title: Automatic Item Reference Updating
---
## What problem does this solve?

### Copy or clone an item with descendants
1. Have a page with references to child items (rendering datasource, internal link, etc.)
1. Clone or copy the item (which copies descendants)
1. The new copies now reference the _old_ child items.

### Create an item from a branch template
1. Have a branch template with references to child items (rendering datasource, internal link, etc.)
1. Create a new item from the branch template
1. The new item now references the child items _in the branch template_, not the newly created child items.

## How do I use it?
There is no special action necessary. Simply copy items or create items from branch templates like normal, and all references within the copied/created items will be automatically updated to reference the new items.

## How does it work?
There are multiple extension points to implement this functionality...

* `item:added` event handler added to update references when creating an item from a branch template
* `ItemDuplicate` processor for the `uiDuplicateItem` pipeline has been replaced and extended to update references when duplicating an item
* Processor added to the `uiCopyItems` and `uiCloneItems` pipelines

## Credit
Implementation is based heavily on [this article](http://reasoncodeexample.com/2013/01/13/changing-sitecore-item-references-when-creating-copying-duplicating-and-cloning/) by Uli Weltersbach.
