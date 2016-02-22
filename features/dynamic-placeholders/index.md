---
title: Dynamic Placeholders
---
Elision has a built-in implementation of dynamic placeholders.

## What problem does this solve?
Sitecore does not support multiple placeholders with the same name on a single page. This becomes a bigger problem when you consider adding multiple instances of a rendering to a page. If that rendering has a placeholder in it, every one of them will show an identical contents, no matter which one you modify.

## How do I use it?
If you have a placeholder that _might_ appear multiple times on a single page, use Elision's `@Html.Sitecore().DynamicPlaceholder()` helper instead of the default Sitecore `@Html.Sitecore().Placeholder()` helper. Elision will handle the rest.

## How does it work?
For Sitecore to distinguish one placeholder from another on the same page, the name must be unique. To work around this, Elision dynamically adds a unique id to the placeholder name. 

The unique ID that is used, is the rendering's unique id, so the contents of the placeholder will follow the rendering around the page, even if they are arranged on the page.

One thing that Elision's DynamicPlaceholder implementation does, that many others do not, is automatically moving nested renderings when the parent rendering is moved to a new placeholder, instead of leaving the nested renderings to be orphaned.

## Credit
This implementation is loosly based on [this post by John Newcombe](http://johnnewcombeuk.blogspot.com/2012/06/sitecore-part-3-dynamic-placeholders.html) from a few years ago, but has been heavily extended for Elision.