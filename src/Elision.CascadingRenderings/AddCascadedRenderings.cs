using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;
using Sitecore.Data;
using Sitecore.Data.Items;
using Sitecore.Layouts;
using Sitecore.Mvc.Pipelines.Response.GetXmlBasedLayoutDefinition;

namespace Elision.CascadingRenderings
{
    public class AddCascadedRenderings : GetXmlBasedLayoutDefinitionProcessor
    {
        public override void Process(GetXmlBasedLayoutDefinitionArgs args)
        {
            if (args.Result == null) return;

            var item = Sitecore.Mvc.Presentation.PageContext.Current.Item;
            var device = Sitecore.Mvc.Presentation.PageContext.Current.Device;
            if (item == null || item.Parent == null || device == null) return;

            var deviceId = ID.Parse(string.Format("{{{0}}}", device.Id.ToString().ToUpper()));
            args.Result = MergeWithCascadedRenderings(args.Result, item.Parent, deviceId);
        }

        protected virtual XElement MergeWithCascadedRenderings(XElement self, Item parent, ID deviceId)
        {
            var parentParsed = parent.GetLayoutDefinition();
            var selfParsed = LayoutDefinition.Parse(self.ToString());
            
            var selfDevice = selfParsed.GetDevice(deviceId.ToString());
            var parentDevice = parentParsed.GetDevice(deviceId.ToString());

            if (parentDevice.Renderings == null) return self; // empty layout

            var cascadePlaceholdersField = parent.Fields[LayoutIDs.CascadePlaceholdersField];
            if (cascadePlaceholdersField == null || string.IsNullOrWhiteSpace(cascadePlaceholdersField.Value))
                return self;

            var placeholdersToCascade = cascadePlaceholdersField.Value.Split('|');

            foreach (var placeholderKey in placeholdersToCascade)
            {
                var renderings = parentDevice
                    .Renderings.Cast<RenderingDefinition>()
                    .Where(r => r.Placeholder.Equals(placeholderKey))
                    .ToArray();

                foreach (var rendering in renderings)
                {
                    rendering.Placeholder = GetCascadedPlaceholderKey(rendering, selfDevice);
                    rendering.Parameters +=
                        (string.IsNullOrWhiteSpace(rendering.Parameters) ? "" : "&")
                        + "disablewebedit=1&wascascaded=1&cascadedfrom=" + parent.Name;
                    selfDevice.AddRendering(rendering);
                }
            }

            return XDocument.Parse(selfParsed.ToXml()).Root;
        }

        private string GetCascadedPlaceholderKey(RenderingDefinition rendering, DeviceDefinition targetDevice)
        {
            var placeholderKeyRegex = new Regex(@"(?<key>.+)_(?<uid>[\d\w]{8}\-(?:[\d\w]{4}\-){3}[\d\w]{12})");
            var match = placeholderKeyRegex.Match(rendering.Placeholder);

            if (!match.Success) //not a dynamic placeholder, just pass it down
                return rendering.Placeholder;

            var placeholderKeyPath = match.Groups["key"].Value;
            var targetPlaceholder = GetParentRenderingForPlaceholderKeyPath(targetDevice, placeholderKeyPath);

            return targetPlaceholder.Or(rendering.Placeholder);
        }

        private string GetParentRenderingForPlaceholderKeyPath(DeviceDefinition device, string keyPath)
        {
            var parts = keyPath.Trim('/').Split('/');
            string parentPath;
            if (parts.Length <= 2)
                parentPath = parts[0];
            else if (parts.Length > 2)
                parentPath = "/" + string.Join("/", parts, 0, parts.Length - 1);
            else
                return null;

            var parentRendering = device
                .Renderings.Cast<RenderingDefinition>()
                .FirstOrDefault(x => Regex.IsMatch(x.Placeholder, "^" + parentPath + @"(_|$)", RegexOptions.IgnoreCase));

            if (parentRendering == null)
                return null;

            var parentUidGuid = Guid.Parse(parentRendering.UniqueId);
            return string.Format("{0}_{1}", keyPath, parentUidGuid);
        }
    }
}
