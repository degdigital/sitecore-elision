---
title: Allow Inherited Templates as Rendering Datasource
---
## What problem does this solve?
1. RenderingA datasource template is EnhancedContentTemplate
1. RenderingB datasource template is ContentTemplate
1. EnhancedContentTemplate inherits from ContentTemplate
1. Add RenderingA to a page, create datasource EnhancedContentTemplate
1. Add RenderingB to another page, try to reference same datasource as RenderingA - this would not be allowed out-of-the-box with Sitecore.

## How do I use it?
There is no special action needed. Inherited templates are automatically allowed to be selected when choosing a rendering datasource.

## How does it work?
The `getRenderingDatasource` pipeline has been extended to include a processor called `AddDerivedTemplatesForSelection`. This pipeline processor adds all derived templates to the allowed "TemplatesForSelection" list.

## Credit
This feature started as an original idea by [Patrick Delancy](http://www.github.com/patrickdelancy), but [this article](http://ggullentops.blogspot.com/2014/10/multiple-datasources-for-associated.html) by Gert Gullentops helped inform the final implementation, even though we didn't solve the problem in the same way.
