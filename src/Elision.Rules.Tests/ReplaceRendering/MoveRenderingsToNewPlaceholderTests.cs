using System;
using System.Collections;
using System.Linq;
using System.Text.RegularExpressions;
using Elision.Rules.ReplaceRendering;
using FluentAssertions;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.FakeDb;
using Sitecore.Layouts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Elision.Rules.Tests.ReplaceRendering
{
    [TestClass]
    public class MoveRenderingsToNewPlaceholderTests
    {
        [TestMethod]
        public void MovesRenderingWhenPlaceholderIsNested()
        {
            DeviceDefinition device;
            RenderingDefinition sourceRendering;
            Item targetRenderingItem;
            using (var db = GetDatabase(out sourceRendering, out targetRenderingItem, out device))
            {
                var ctx = new ReplaceRenderingRuleContext(sourceRendering, targetRenderingItem, device);
                var action = new MoveRenderingsToNewPlaceholderAction<ReplaceRenderingRuleContext>()
                {
                    SourcePlaceholderKey = _placeholderKeyNestedWith1Rendering,
                    TargetPlaceholderKey = _placeholderKeyWith0Renderings
                };

                action.Apply(ctx);

                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyNestedWith1Rendering)).Should().BeEmpty();
                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyWith0Renderings)).Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void MovesOneRendering()
        {
            DeviceDefinition device;
            RenderingDefinition sourceRendering;
            Item targetRenderingItem;
            using (var db = GetDatabase(out sourceRendering, out targetRenderingItem, out device))
            {
                var ctx = new ReplaceRenderingRuleContext(sourceRendering, targetRenderingItem, device);
                var action = new MoveRenderingsToNewPlaceholderAction<ReplaceRenderingRuleContext>()
                {
                    SourcePlaceholderKey = _placeholderKeyWith1Rendering,
                    TargetPlaceholderKey = _placeholderKeyWith0Renderings
                };

                action.Apply(ctx);

                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyWith1Rendering)).Should().BeEmpty();
                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyWith0Renderings)).Should().HaveCount(1);
            }
        }

        [TestMethod]
        public void Moves4Renderings()
        {
            DeviceDefinition device;
            RenderingDefinition sourceRendering;
            Item targetRenderingItem;
            using (var db = GetDatabase(out sourceRendering, out targetRenderingItem, out device))
            {
                var ctx = new ReplaceRenderingRuleContext(sourceRendering, targetRenderingItem, device);
                var action = new MoveRenderingsToNewPlaceholderAction<ReplaceRenderingRuleContext>()
                {
                    SourcePlaceholderKey = _placeholderKeyWith4Renderings,
                    TargetPlaceholderKey = _placeholderKeyWith0Renderings
                };

                action.Apply(ctx);

                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyWith4Renderings)).Should().BeEmpty();
                ctx.Device.Renderings.Cast<RenderingDefinition>().Where(x => MatchesPlaceholderKey(x.Placeholder, _placeholderKeyWith0Renderings)).Should().HaveCount(4);
            }
        }

        private bool MatchesPlaceholderKey(string keyToLookIn, string keyToFind)
        {
            return Regex.IsMatch(keyToLookIn, string.Format(@"\b{0}_", keyToFind));
        }

        private readonly string _placeholderKeyWith1Rendering = "placeholder1";
        private readonly string _placeholderKeyWith0Renderings = "placeholder0";
        private readonly string _placeholderKeyWith4Renderings = "placeholder4";

        private readonly string _placeholderKeyNestedWith1Rendering = "nestedplaceholder1";

        private readonly ID _sourceRenderingItemId = ID.NewID;
        private readonly ID _targetRenderingItemId = ID.NewID;

        private readonly Guid _renderingUniqueId = Guid.NewGuid();

        private readonly ID _pageId = ID.NewID;

        private Db GetDatabase(out RenderingDefinition sourceRendering, out Item targetRenderingItem, out DeviceDefinition device)
        {
            var db = new Db();

            var page = new DbItem("page", _pageId);
            db.Add(page);

            db.Add(new DbItem("sourceRendering", _sourceRenderingItemId));
            db.Add(new DbItem("targetRendering", _targetRenderingItemId));

            page.Add(new DbField("__Renderings", Sitecore.FieldIDs.LayoutField) {Value = string.Format(@"<r xmlns:xsd=""http://www.w3.org/2001/XMLSchema"" xmlns:s=""s"">
	<d id=""{{E68F244A-399C-4B86-B517-5B116547CECE}}"">
		<r uid=""{1}"" s:id=""{2}"" s:ph=""pagebody"" />
		<r uid=""{{CDFD3A99-7A88-4D72-8E5C-2E0F3565C052}}"" s:ds=""{{7D426874-C107-4A7A-A0C0-D0E041C649C2}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{3}_{0:d}"" />
		<r uid=""{{305AE5CD-9555-4258-91A6-FF939BC0650F}}"" s:ds=""{{E3B3F0F6-CB76-4EA6-A3F6-681490B2345B}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{4}_{0:d}"" />
		<r uid=""{{8F5FE967-B933-4E77-8731-D10EABEEBE9A}}"" s:ds=""{{143B554C-E9BF-414D-81B8-CDCFFD2702F4}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{4}_{0:d}"" />
		<r uid=""{{C4718241-6719-4ED0-A5D5-5875F834D89D}}"" s:ds=""{{E4E35B2C-709E-4547-B217-155604766169}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{4}_{0:d}"" />
		<r uid=""{{E1891090-34EA-4DB3-91E9-D3D312EA2B4F}}"" s:ds=""{{D46A9CBF-F999-440B-B551-91667A6C1FCA}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{4}_{0:d}"" />
		<r uid=""{{F30E85D9-7F9D-4FAE-B70B-DF57DC4CF78C}}"" s:ds=""{{2825111A-5285-48DE-A5F5-F28B7C3181A6}}"" s:id=""{{D97EB6F7-26BF-4347-8988-D6019C3E2E0B}}"" s:ph=""{4}_{0:d}"" />
	</d>
</r>",
                                                                                                                  _renderingUniqueId,
                                                                                                                  ID.Parse(_renderingUniqueId),
                                                                                                                  _sourceRenderingItemId,
                                                                                                                  _placeholderKeyWith1Rendering,
                                                                                                                  _placeholderKeyWith4Renderings
                         )});

            device = new DeviceDefinition
                {
                    ID = ID.NewID.ToString(),
                    Renderings = new ArrayList
                        {
                            new RenderingDefinition{ UniqueId = ID.Parse(_renderingUniqueId).ToString(), ItemID = _sourceRenderingItemId.ToString(), Placeholder = "pagebody"},
                            new RenderingDefinition{ Placeholder = string.Format("{0}_{1}", _placeholderKeyWith1Rendering, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                            new RenderingDefinition{ Placeholder = string.Format("{0}_{1}", _placeholderKeyWith4Renderings, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                            new RenderingDefinition{ Placeholder = string.Format("{0}_{1}", _placeholderKeyWith4Renderings, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                            new RenderingDefinition{ Placeholder = string.Format("{0}_{1}", _placeholderKeyWith4Renderings, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                            new RenderingDefinition{ Placeholder = string.Format("{0}_{1}", _placeholderKeyWith4Renderings, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                            new RenderingDefinition{ Placeholder = string.Format("/pagebody/{0}_{1}", _placeholderKeyNestedWith1Rendering, _renderingUniqueId), UniqueId = ID.NewID.ToString(), ItemID = ID.NewID.ToString()},
                        }
                };
            targetRenderingItem = db.GetItem(_targetRenderingItemId);
            sourceRendering = device.GetRenderingByUniqueId(new ID(_renderingUniqueId).ToString());

            return db;
        }
    }
}
