using Sitecore.Data.Events;
using Sitecore.Data.Items;
using Sitecore.Events;
using Sitecore.Diagnostics;
using Sitecore.Buckets;
using System;
using Sitecore.Buckets.Managers;
using Sitecore.Data;
using DCX.Foundation.Extension.Constant;
using Sitecore.Buckets.Extensions;
using Sitecore.Links;
namespace DCX.Foundation.Extension.Buckets
{
    public class BucketSyncEventHandler
    {
        public void OnItemSaved(object sender, EventArgs args)
        {
            try
            {
                Item itemEventArgs = Event.ExtractParameter(args, 0) as Item;
                if (itemEventArgs == null)
                {
                    Log.Warn("ItemCreatedEventArgs is null in OnItemCreated", this);
                    return;
                }
                if (IsItemUpdated(itemEventArgs))
                {
                    var bucketItem = itemEventArgs.GetParentBucketItemOrParent();
                    Database masterDb = Sitecore.Configuration.Factory.GetDatabase(ExtensionConstant.MASTERDB);
                    Item itemdata = masterDb.GetItem(bucketItem.ID);
                    if (IsBucketable(itemdata))
                    {
                        PerformBucketSync(itemdata);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error("Error Form : OnItemSaved method in form " + ex.Message, this);
            }
        }
        private bool IsItemUpdated(Item item)
        {
            var bucketItem = item.GetParentBucketItemOrParent();
            Sitecore.Data.ID locateUsLandingPage = new Sitecore.Data.ID(ExtensionConstant.LocateUsLandingPage);
            Sitecore.Data.ID articleFolder = new Sitecore.Data.ID(ExtensionConstant.ArticleFolder);
            if (bucketItem.Template.ID == locateUsLandingPage)
            {
                string locationTarget = null;
                Sitecore.Data.Fields.ReferenceField regionField = item.Fields[ExtensionConstant.Region];
                if (regionField != null && regionField.TargetItem != null)
                {
                    locationTarget = regionField.TargetItem.Name;
                }
                if (!String.IsNullOrWhiteSpace(locationTarget))
                {
                    return true;
                }
                return false;
            }
            if (bucketItem.Template.ID == articleFolder)
            {
                string newItemDate = null;
                newItemDate = item.Fields[ExtensionConstant.ArticleDate]?.Value;
                if (!String.IsNullOrWhiteSpace(newItemDate))
                {
                    return true;
                }
                return false;
            }
            return false;
        }
        private bool IsBucketable(Item item)
        {
            return true;
        }
        private void PerformBucketSync(Item item)
        {
            try
            {
                BucketManager.Sync(item);
                Log.Info($"Bucket synchronization performed for item: {item.ID}", this);
            }
            catch (Exception ex)
            {
                Log.Error("Error during bucket synchronization", ex, this);
            }
        }
    }
}