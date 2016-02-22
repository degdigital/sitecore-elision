---
title: Dynamic relative rendering datasource folders
---
Elision adds the ability to define relative datasource folders for renderings, and will automatically create the relative folder if it does not already exist.

## What problem does this solve?
Sitecore allows you to define a relative datasource folder for a rendering, but that folder must already exist. If the relative folder does not exist, Sitecore would throw an error.

## How do I use it?
When setting the "Datasource folder" for a rendering, use a manually-entered relative path, e.g. "./_components".

If the relative folder that is referenced does not exist, it will be created automatically before the datasource selection dialog is shown.

For additional configurability, Elision adds a new global rules folder where the rendering datasource folders can be controlled. More details can be found on the [Get Rendering Datasource Rules page]({{ "/features/get-rendering-datasource-rules/" | prepend: site.baseurl }}).

## How does it work?
By extending the `getRenderingDatasource` pipeline.

## Credit
Bits and pieces of this implementation have been taken from these places:
* [This answer on StackOverflow](http://stackoverflow.com/questions/7678597/how-to-use-sitecore-query-in-datasource-location-dynamic-datasouce) by techphoria414, aka Nick Wesselman
* [This blog post](https://jermdavis.wordpress.com/2014/02/21/improving-your-sitecore-ia-with-relative-datasource-locations/) by Jeremy Davis
* [This blog post](http://blog.istern.dk/2012/05/15/queries-in-datasource-location-on-sitecore-layouts/) by Thomas Stern