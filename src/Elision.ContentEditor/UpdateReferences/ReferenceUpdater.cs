using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Sitecore.Data;
using Sitecore.Data.Fields;
using Sitecore.Data.Items;
using Sitecore.Diagnostics;
using Sitecore.SecurityModel;
using Sitecore.Xml.Patch;
using Elision.Diagnostics;

namespace Elision.ContentEditor.UpdateReferences
{
    public class ReferenceUpdater
    {
        private Database _masterDb;
        private readonly Item _startItem;

        //List Items to process and can be replacable.
        private readonly Dictionary<string, EqualItems> _result = new Dictionary<string, EqualItems>();
        private readonly Dictionary<string, string> _roots = new Dictionary<string, string>();

        //List of ignored/excluded item e.g. device id or any id which is not required for replacement process. Also gains performance.
        private readonly List<string> _ignoredItemIds = new List<string>();
        private readonly bool _deep;

        public int Count = 0;

        private Func<Field, bool> ShouldProcessField { get; set; }

        public ReferenceUpdater(Item start, Dictionary<string, string> roots, bool deep = false)
        {
            _startItem = start;
            _roots = roots;
            _deep = deep;
            Count = 0;
        }

        private bool ExcludeStandardSitecoreFieldsExceptLayout(Field field)
        {
            Assert.ArgumentNotNull(field, "field");
            return field.ID == SitecoreIDs.RenderingsFieldId
                   || field.ID == SitecoreIDs.FinalLayoutField
                   || !field.Name.StartsWith("__");
        }

        public void Start()
        {
            using (var trace = new TraceOperation("Update item references"))
            {
                if (ShouldProcessField == null)
                {
                    //Default if not supplied from outside.
                    ShouldProcessField = ExcludeStandardSitecoreFieldsExceptLayout;
                }

                if (_startItem != null && _roots != null)
                {
                    _masterDb = _startItem.Database;
                    FixReferences(_startItem, _roots, _deep);
                }
            }
        }

        private void FixReferences(Item start, Dictionary<string, string> roots, bool deep = false)
        {
            Count++;
            ProcessItem(start);
            if (!deep || !start.HasChildren)
                return;

            var childrens = start.Children;
            foreach (Item c in childrens)
                FixReferences(c, roots, deep);
        }

        private void ProcessItem(Item contextItem)
        {
            if (contextItem == null)
                return;
            var fields = GetFieldsToProcess(contextItem);
            foreach (var field in fields)
            {
                foreach (var itemVersion in GetVersionsToProcess(contextItem))
                {
                    var itemVersionField = itemVersion.Fields[field.ID];
                    ProcessField(itemVersionField);
                }
            }
        }

        private string GetInitialFieldValue(Field field)
        {
            return field.GetValue(true, true);
        }

        private static string GetLayoutFieldValue(Field field)
        {
            return LayoutField.GetFieldValue(field);

            var value = field.GetValue(true, true);
            var func = XmlDeltas.WithEmptyValue("<r />");
            if (string.IsNullOrEmpty(value))
                return value;

            if (!XmlPatchUtils.IsXmlPatch(value))
                return value;

            return XmlDeltas.ApplyDelta(func(field), value);
        }

        private void ProcessField(Field field)
        {
            string initialValue;
            var fieldValue = string.Empty;
            if (field.ID != Sitecore.FieldIDs.LayoutField && field.ID != Sitecore.FieldIDs.FinalLayoutField)
            {
                initialValue = GetInitialFieldValue(field);
            }
            else
            {
                //Special Case.
                //Full Starndard + Delta.
                fieldValue = field.GetValue(true, true);
                //Patched Value
                initialValue = GetLayoutFieldValue(field);
            }

            if (string.IsNullOrEmpty(initialValue))
                return;

            //Get Keys : Ids and Paths
            var keys = GetKeys(initialValue);

            //Checks keys needs to be ignored otherwise equivalent item exist, if yes prepare list of equal items other wise add in ignore.
            GetReplaceableList(keys);

            //Replace with equivaent item list.
            if (!_result.OrAny())
                return;

            var value = new StringBuilder(initialValue);
            foreach (var r in _result)
            {
                value = value.Replace(r.Value.Source.ID.Guid.ToString("D").ToUpper(), r.Value.Dest.ID.Guid.ToString("D").ToUpper());
                value = value.Replace(r.Value.Source.ID.Guid.ToString("D").ToLower(), r.Value.Dest.ID.Guid.ToString("D").ToLower());
                value = value.Replace(r.Value.Source.ID.Guid.ToString("N").ToUpper(), r.Value.Dest.ID.Guid.ToString("N").ToUpper());
                value = value.Replace(r.Value.Source.ID.Guid.ToString("N").ToLower(), r.Value.Dest.ID.Guid.ToString("N").ToLower());
                value = value.Replace(r.Value.Source.Paths.Path, r.Value.Dest.Paths.Path, true);
                value = value.Replace(r.Value.Source.Paths.Path.ToLower(), r.Value.Dest.Paths.Path.ToLower(), true);
                if (!r.Value.Source.Paths.IsContentItem)
                    continue;

                value.Replace(r.Value.Source.Paths.ContentPath, r.Value.Dest.Paths.ContentPath);
                value.Replace(r.Value.Source.Paths.ContentPath.ToLower(), r.Value.Dest.Paths.ContentPath.ToLower());
            }

            //Special care taken to handler Standard Value and Delta of layout , thus to maintain inheritance otherwise it will break inhertiance
            // and any change to template will not reflect in derived pages.
            if (field.ID == Sitecore.FieldIDs.LayoutField || field.ID == Sitecore.FieldIDs.FinalLayoutField)
            {
                using (new Sitecore.Data.Events.EventDisabler())
                using (new EditContext(field.Item, SecurityCheck.Disable))
                {
                    LayoutField.SetFieldValue(field, value.ToString());
                }
                //Difference of old value and new updated value, thus inheritance remain in place.
                //Try to get new patch.
                //initialValue = XmlDeltas.ApplyDelta(fieldValue, initialValue);
            }
            else
            {
                UpdateFieldValue(field, initialValue, value);
            }
        }

        private void UpdateFieldValue(Field field, string initialValue, StringBuilder value)
        {
            if (initialValue.Equals(value.ToString()))
                return;

            using (new Sitecore.Data.Events.EventDisabler())
            using (new EditContext(field.Item, SecurityCheck.Disable))
            {
                field.Value = value.ToString();
            }
        }

        private Dictionary<string, EqualItems> GetReplaceableList(List<string> keys)
        {
            foreach (var key in keys)
            {
                if (_ignoredItemIds.Contains(key)) continue;

                if (_result.ContainsKey(key)) continue;

                var kItem = GetItemFromString(key);
                if (kItem != null)
                {
                    if (IsReplaceable(kItem.Paths.Path))
                    {
                        var equalItem = GetEquivalentItem(kItem.Paths.Path);

                        if (equalItem != null)
                        {
                            _result.Add(key, new EqualItems() { Source = kItem, Dest = equalItem });
                            continue;
                        }
                    }
                }
                _ignoredItemIds.Add(key);
            }

            return _result;
        }

        private Item GetItemFromString(string idOrPath)
        {
            if (!string.IsNullOrEmpty(idOrPath))
            {
                ID itemId = idOrPath.OrID();
                if (!itemId.IsNull)
                    return itemId.GetItem(_masterDb);

                return _masterDb.GetItem(idOrPath.Trim('"'));

            }
            return null;
        }

        private IEnumerable<Item> GetVersionsToProcess(Item item)
        {
            return item.Versions.GetVersions(true);
        }

        //Reads all field of given item including Clone items.
        private IEnumerable<Field> GetFieldsToProcess(Item item)
        {
            item.Fields.ReadAll();
            return item.Fields.Where(ShouldProcessField).ToArray();
        }

        private List<string> GetKeys(string fieldValue)
        {
            //Regex for Guid/Path
            const string pattern = "([a-zA-Z0-9]{8}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{4}-[a-zA-Z0-9]{12})|([a-zA-Z0-9]{32})|(\"/.*?\")";
            var keys = new List<string>();
            var keyRegex = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Multiline);
            var foundKeys = keyRegex.Matches(fieldValue);
            if (foundKeys.Count > 0)
            {
                foreach (Match m in foundKeys)
                {
                    if (!keys.Contains(m.Value))
                        keys.Add(m.Value);
                }
            }
            return keys.Where(k => !string.IsNullOrEmpty(k)).Distinct().ToList();
        }

        //Check each root item if any matchs then is replacable otherwise ignored.
        private bool IsReplaceable(string path)
        {
            return _roots.OrAny() && _roots.Any(root => path.ToLower().Contains(root.Key.ToLower()));
        }

        private Item GetEquivalentItem(string path)
        {
            if (_roots.OrAny())
            {
                foreach (var root in _roots)
                {
                    if (!path.ToLower().Contains(root.Key.ToLower())) continue;

                    var equalPath = path.Replace(root.Key.Replace("$", "\\$"), root.Value, true);
                    return _masterDb.GetItem(equalPath);
                }
            }
            return null;
        }
    }
}