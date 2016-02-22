using Sitecore;
using Sitecore.Buckets.Extensions;
using Sitecore.Buckets.Managers;
using Sitecore.Buckets.Pipelines.UI;
using Sitecore.Configuration;
using Sitecore.Data.Items;
using Sitecore.Data.Managers;
using Sitecore.Diagnostics;
using Sitecore.Events;
using Sitecore.Globalization;
using Sitecore.Web.UI.Sheer;
using System;
using System.Collections.Generic;

namespace Elision.ContentEditor.UpdateReferences
{
    public class DuplicateItem : ItemDuplicate
    {
        private Item _itemToDuplicate;

        public new void Execute(ClientPipelineArgs args)
        {
            var targetItem = Duplicate(args);
            if (targetItem == null)
                return;

            if (_itemToDuplicate == null)
                return;

            var roots = new Dictionary<string, string>
                {
                    {_itemToDuplicate.Paths.Path, targetItem.Paths.Path}
                };

            if (roots.Count <= 0)
                return;

            var refUpdater = new ReferenceUpdater(targetItem, roots, true);
            refUpdater.Start();
        }

        private Item Duplicate(ClientPipelineArgs args)
        {
            Item result = null;
            Assert.ArgumentNotNull(args, "args");
            var database = Factory.GetDatabase(args.Parameters["database"]);
            Assert.IsNotNull(database, args.Parameters["database"]);

            var itemId = args.Parameters["id"];
            var sourceItem = database.Items[itemId];
            if (sourceItem != null)
            {
                var parent = sourceItem.Parent;
                if (parent == null)
                {
                    SheerResponse.Alert(Translate.Text("Cannot duplicate the root item."), new string[0]);
                    args.AbortPipeline();
                }
                else if (!parent.Access.CanCreate())
                {
                    var displayName = new object[] { sourceItem.DisplayName };
                    SheerResponse.Alert(Translate.Text("You do not have permission to duplicate \"{0}\".", displayName), new string[0]);
                    args.AbortPipeline();
                }
                else
                {
                    var strArrays = new[] { AuditFormatter.FormatItem(sourceItem) };
                    Log.Audit(this, "Duplicate item: {0}", strArrays);

                    var parentBucketItemOrSiteRoot = sourceItem.GetParentBucketItemOrSiteRoot();
                    _itemToDuplicate = sourceItem;
                    if (!BucketManager.IsBucket(parentBucketItemOrSiteRoot))
                    {
                        result = Context.Workflow.DuplicateItem(sourceItem, args.Parameters["name"]);
                    }
                    else
                    {
                        var objArray = new object[] { args, this };
                        if (Event.RaiseEvent("item:bucketing:duplicating", objArray).Cancel)
                        {
                            Log.Info(string.Format("Event {0} was cancelled", "item:bucketing:duplicating"), this);
                            args.AbortPipeline();
                            return null;
                        }
                        var clonedItem = Context.Workflow.DuplicateItem(sourceItem, args.Parameters["name"]);
                        var bucketFolderDestination = CreateAndReturnBucketFolderDestination(parentBucketItemOrSiteRoot, DateTime.Now, clonedItem);
                        if (!IsBucketTemplateCheck(sourceItem))
                            bucketFolderDestination = parentBucketItemOrSiteRoot;

                        ItemManager.MoveItem(clonedItem, bucketFolderDestination);
                        var objArray1 = new object[] { args, this };
                        Event.RaiseEvent("item:bucketing:duplicated", objArray1);
                        Log.Info(string.Concat("Item ", clonedItem.ID, " has been duplicated to another bucket"), this);
                        result = clonedItem;
                    }
                }
            }
            else
            {
                SheerResponse.Alert(Translate.Text("Item not found."), new string[0]);
                args.AbortPipeline();
            }
            args.AbortPipeline();

            return result;
        }

        internal static Item CreateAndReturnBucketFolderDestination(Item topParent, DateTime childItemCreationDateTime, Item itemToMove)
        {
            return BucketManager.Provider.CreateAndReturnBucketFolderDestination(topParent, childItemCreationDateTime, itemToMove);
        }

        internal static bool IsBucketTemplateCheck(Item item)
        {
            if (item != null)
            {
                if (item.Fields[Sitecore.Buckets.Util.Constants.IsBucket] != null)
                {
                    return item.Fields[Sitecore.Buckets.Util.Constants.BucketableField].Value.Equals("1");
                }
                if (item.Paths.FullPath.StartsWith("/sitecore/templates"))
                {
                    var templateItem = item.Children[0] != null ? item.Children[0].Template : null;
                    if (templateItem != null)
                    {
                        if (templateItem.StandardValues != null && templateItem.StandardValues[Sitecore.Buckets.Util.Constants.BucketableField] != null)
                        {
                            return templateItem.StandardValues[Sitecore.Buckets.Util.Constants.BucketableField].Equals("1");
                        }
                    }
                }
            }
            return false;
        }
    }
}
