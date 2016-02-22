using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Elision.Rules.PlaceholderSettings;
using FluentAssertions;
using Sitecore;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Layouts;
using Sitecore.Pipelines.GetPlaceholderRenderings;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Rules.Tests.PlaceholderSettings
{
    [TestClass]
    [Ignore] //("FakeDb does not support listing a template's BaseTemplates, which is necessary for this rule action")]
    public class AddRenderingFolderActionTests
    {
        [TestMethod]
        public void AddsAllRenderingsFromFolder()
        {
            ID[] renderingIds;
            ID[] folderIds;
            var layoutDefinition = new LayoutDefinition() {Devices = new ArrayList() {new DeviceDefinition()}};
            
            using (var db = GetDatabase(out renderingIds, out folderIds))
            {
                var item = db.GetItem("/sitecore/home/page");
                var args = new GetPlaceholderRenderingsArgs("placeholder", layoutDefinition.ToXml(), db.Database)
                    {
                        PlaceholderRenderings = new List<Item>()
                    };
                var ctx = new PlaceholderSettingsRuleContext(args, item);

                var action = new AddRenderingFolderAction<PlaceholderSettingsRuleContext>()
                    {
                        RenderingFolderItemId = folderIds[0].ToString()
                    };

                action.Apply(ctx);

                args.PlaceholderRenderings.Select(x => x.ID)
                    .Should().HaveCount(2)
                    .And.Contain(renderingIds.Take(2));
            }
        }

        [TestMethod]
        public void BaseTemplatesIssue()
        {
            using (var db = new Db() {new DbItem("page")})
            {
                var aFolder = db.GetItem("/sitecore/content/page");
                var baseTemplates = aFolder.Template.BaseTemplates;

                Assert.IsNotNull(baseTemplates);
            }
        }

        private Db GetDatabase(out ID[] renderingIds, out ID[] folderIds)
        {
            var ids = new List<ID>(14);
            while (ids.Count < ids.Capacity)
                ids.Add(ID.NewID);

            renderingIds = ids.ToArray();
            var renderingIdQueue = new Queue<ID>(ids);

            ids = new List<ID>(6);
            while (ids.Count < ids.Capacity)
                ids.Add(ID.NewID);
            folderIds = ids.ToArray();
            var folderIdQueue = new Queue<ID>(ids);

            var renderingTemplateId = ID.NewID;
            var db = new Db()
                {
                    new DbTemplate(SitecoreIDs.RenderingSectionBaseTemplateId),
                    new DbTemplate("RenderingTemplate", renderingTemplateId)
                        {
                            BaseIDs = new []{SitecoreIDs.RenderingSectionBaseTemplateId}
                        },
                    new DbItem("page"),
                    new DbItem("Layouts", ItemIDs.LayoutRoot)
                        {
                            new DbItem("folder a", folderIdQueue.Dequeue()) {
                                new DbItem("Rendering A1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                new DbItem("Rendering A2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                new DbItem("subfolder aa", folderIdQueue.Dequeue())
                                    {
                                        new DbItem("Rendering AA1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                        new DbItem("Rendering AA2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                        new DbItem("subfolder aaa", folderIdQueue.Dequeue())
                                            {
                                                new DbItem("Rendering AAA1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                                new DbItem("Rendering AAA2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                            }
                                    }
                            },
                            new DbItem("folder b", folderIdQueue.Dequeue()) {
                                new DbItem("Rendering B1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                new DbItem("Rendering B2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                new DbItem("subfolder ba", folderIdQueue.Dequeue())
                                    {
                                        new DbItem("Rendering Ba1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                        new DbItem("Rendering Ba2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                    },
                                new DbItem("subfolder bb", folderIdQueue.Dequeue())
                                    {
                                        new DbItem("Rendering BB1", renderingIdQueue.Dequeue(), renderingTemplateId),
                                        new DbItem("Rendering BB2", renderingIdQueue.Dequeue(), renderingTemplateId),
                                    }
                            },
                            new DbItem("Rendering 1", renderingIdQueue.Dequeue(), renderingTemplateId),
                            new DbItem("Rendering 2", renderingIdQueue.Dequeue(), renderingTemplateId)
                        }
                };
            return db;
        }
    }
}
