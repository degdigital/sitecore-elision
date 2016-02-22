using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Sitecore.FakeDb;
using Sitecore.FakeDb.Serialization;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Tests.Components
{
    [TestClass]
    public class ConsistencyTests
    {
        [TestMethod]
        public void NoItemsAreLocked()
        {
            using (GetSerializedData())
            {
                var allItems = Sitecore.Context.Database.GetItem(Sitecore.ItemIDs.RootID)
                                       .Axes.GetDescendants();

                //FakeDb scaffolds 4 starting items to hang fake content on. Make sure we have loaded more than just that.
                allItems.Count().Should().BeGreaterThan(4);

                var lockedItems = allItems.Where(x => x.Locking.HasLock());

                lockedItems.Should().BeEmpty();
            }
        }

        [TestMethod]
        [Ignore]//("FakeDB serialization is letting me down again. It doesn't seem to load the values for the controller and controller action.")]
        public void AllControllerRenderingsReferencesExist()
        {
            using (GetSerializedData())
            {
                var controllerRenderings = Sitecore.Context.Database.SelectItems("//*[@@TemplateID='" + Sitecore.Mvc.Names.TemplateIds.ControllerRendering + "']");
                var issues = new List<string>();

                if (controllerRenderings.Length == 0)
                    Assert.Inconclusive("There were no controller renderings found. Check the tds projects to make sure this is correct.");

                foreach (var rendering in controllerRenderings)
                {
                    var controllerName = rendering.Fields["Controller"].Value; //Sitecore.Mvc.Names.PropertyNames.Controller
                    var controllerType = Sitecore.Reflection.ReflectionUtil.GetTypeInfo(controllerName);
                    
                    if (controllerType == null)
                    {
                        issues.Add(string.Format("Unable to find type '{0}' for controller rendering {1} {2}",
                                                 controllerName,
                                                 rendering.Paths.Path, rendering.ID));
                        continue;
                    }

                    var controllerActionName = rendering.Fields["Controller Action"].Value; //Sitecore.Mvc.Names.PropertyNames.ControllerAction
                    if (controllerType.Methods().Any(x => x.Name == controllerActionName))
                        continue;

                    issues.Add(string.Format("Unable to find action '{0}' on type '{1}' for controller rendering {2} {3}",
                                             controllerActionName, controllerType.Name,
                                             rendering.Paths.Path, rendering.ID));
                }

                if (issues.Any())
                    Assert.Fail(string.Join("\r\n", issues));
            }
        }

        private static Db GetSerializedData()
        {
            return new Db
                {
                    new DsDbItem("/sitecore/layout", true) {ParentID = Sitecore.ItemIDs.RootID},
                    //new DsDbItem("/sitecore/content/home", true) {ParentID = Sitecore.ItemIDs.ContentRoot},
                    //new DsDbItem("/sitecore/templates/deg", true, false) {ParentID = Sitecore.ItemIDs.TemplateRoot}
                };
        }
    }
}
