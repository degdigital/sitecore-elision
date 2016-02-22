---
title: Controller Rendering Parameter Values
---
Elision contains an MVC ValueProvider and ValueProviderFactory that use a Sitecore pipeline to build a collection of objects that can be considered as candidates for controller actions. Objects set in the pipeline are eligible to be passed as parameters to a controller rendering's action.

By default, Elision will include some common items, like renderingDatasource, pageContextItem, context database, etc., but the `mvc.getControllerRenderingValueParameters` pipeline can be extended to include other items.

## What problem does this solve?
There are two major issues that are improved with this feature:
1. Reduces the need to reference static context objects, which helps with testability.
1. Cuts down on duplicated code to retrieve common data, like rendering parameters.

## How do I use it?
To use this feature, you just need to add a parameter to your controller action. If the parameter is the expected type, and named correctly, the rest happens automatically.

{% highlight csharp %}
public class ContentController : SitecoreController {
	public ActionResult Index(Sitecore.Data.Database db) {
		Assert.AreEqual(Sitecore.Context.Database, db);
		return View();
	}
}
{% endhighlight %}

You can also create a class to be used as a parameter. The properties will be set using the same rules as normal method parameters.

{% highlight csharp %}
public class MyRenderingParameters
{
  public Item PageContextItem { get; set; }
  public ID RenderingUniqueId { get; set; }
}

public class ContentController : SitecoreController {
  public ActionResult Index(MyRenderingParameters args) {
    Assert.AreEqual(RenderingContext.Current.PageContext.Item, args.PageContextItem);
    Assert.AreEqual(RenderingContext.Current.Rendering.UniqueId, args.RenderingUniqueId);
    return View();
  }
}
{% endhighlight %}

### Using rendering parameters
Elision comes with the pipeline processor `AddRenderingParameters` that will parse through all of the saved rendering parameters so that they can be easily used in your controller action.

### Adding your own custom values
To include your own values, you simply need to add a new processor to the `mvc.getControllerRenderingValueParameters` pipeline. Your handler should implement the `IGetControllerRenderingValueParametersProcessor` interface.

## How does it work?
This is a simple extension of the Model Binding that ASP.NET MVC already does. 

The bulk of the work is done in Elision's `PipelineValueProviderFactory`. This factory is what calls the `mvc.getControllerRenderingValueParameters` pipeline.

This provider factory is initialized and registered in the `initialize` pipeline by the `InitializeValueProviderFactories` processor.

## How do I disable it?
We think this is valuable functionality, and it is used by the core Elision renderings. If you are using any of the standard renderings that come with Elision, you should not disable this functionality.

If you are absolutely certain that you are not using this functionality, you can save a tiny bit of time on initialize, and shave a few milliseconds off of each web request by removing the `InitializeValueProviderFactories` processor from the `initialize` pipeline.

## Credit
The implementation for this feature is based heavily on [this post](http://geekswithblogs.net/KyleBurns/archive/2012/10/18/using-sitecore-renderingcontext-parameters-as-mvc-controller-action-arguments.aspx) from Kyle Burns a few years ago.