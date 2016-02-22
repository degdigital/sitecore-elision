using System;
using System.Globalization;
using System.Linq;
using DEG.GoogleMaps.Models;
using DEG.GoogleMaps.Services;
using DEG.ServiceCore.Authentication;
using Elision.Data;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.SecurityModel;

namespace Elision.Location
{
    public class SetGeolocationOnItemSave
    {
        public void OnItemSaving(object sender, EventArgs args)
        {
            var item = Event.ExtractParameter(args, 0) as Item;
            if (item == null || item.Name == "__Standard Values")
                return;
            if (!item.InheritsFrom(TemplateIDs.GeoLocationTemplate) || !item.InheritsFrom(TemplateIDs.PhysicalLocationTemplate))
                return;

            using (new SecurityDisabler())
            {
                if (!string.IsNullOrEmpty(item["Latitude"]) && !string.IsNullOrEmpty(item["Longitude"])) 
                    return;

                var location = GetGeolocation(item);
                SetGeolocation(item, location);
            }
        }

        protected virtual GeocodingGeometryLocation GetGeolocation(Item item)
        {
            var address =
                string.Format(
                    "{0} {1}, {2}, {3}, {4}",
                    item.Fields["Address"],
                    item.Fields["Address2"],
                    item.Fields["City"],
                    item.Fields["State"],
                    item.Fields["PostalCode"]);

            var geocoder = new GeocodingService(new NoAuthentication(), false);
            var result = geocoder.LookupByAddress(address);

            if (result.Status != GeocodingStatusCode.Ok || result.Results == null)
                return null;

            var location = result.Results.FirstOrDefault();
            if (location == null || location.Geometry == null || location.Geometry.Location == null)
                return null;

            return location.Geometry.Location;
        }

        protected virtual void SetGeolocation(Item item, GeocodingGeometryLocation location)
        {
            if (location == null)
                return;

            item.Fields["Latitude"].Value = location.Latitude.ToString(CultureInfo.InvariantCulture);
            item.Fields["Longitude"].Value = location.Longitude.ToString(CultureInfo.InvariantCulture);
        }
    }
}
