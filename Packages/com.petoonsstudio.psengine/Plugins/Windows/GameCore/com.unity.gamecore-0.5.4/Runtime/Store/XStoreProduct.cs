using System;
using Unity.GameCore.Interop;
using System.Runtime.InteropServices;
using System.Collections.Generic;

namespace Unity.GameCore
{
    [Flags]
    public enum XStoreProductKind: UInt32
    {
        None = 0x0,
        Consumable = 0x1,
        Durable = 0x2,
        Game = 0x4,
        Pass = 0x8,
        UnmanagedConsumable = 0x10
    }

    public enum XStoreDurationUnit: UInt32
    {
        Minute = 0,
        Hour = 1,
        Day = 2,
        Week = 3,
        Month = 4,
        Year = 5
    }

    public class XStorePrice
    {
        internal XStorePrice(Interop.XStorePrice rawPrice)
        {
            this.BasePrice = rawPrice.basePrice;
            this.Price = rawPrice.price;
            this.RecurrencePrice = rawPrice.recurrencePrice;
            this.CurrencyCode = rawPrice.currencyCode.GetString();
            this.FormattedBasePrice = Converters.ByteArrayToString(rawPrice.formattedBasePrice);
            this.FormattedPrice = Converters.ByteArrayToString(rawPrice.formattedPrice);
            this.FormattedRecurrencePrice = Converters.ByteArrayToString(rawPrice.formattedRecurrencePrice);
            this.IsOnSale = rawPrice.isOnSale;
            this.SaleEndDate = rawPrice.saleEndDate.DateTime;
        }

        public float BasePrice { get; }
        public float Price { get; }
        public float RecurrencePrice { get; }
        public string CurrencyCode { get; }
        public string FormattedBasePrice { get; }
        public string FormattedPrice { get; }
        public string FormattedRecurrencePrice { get; }
        public bool IsOnSale { get; }
        public DateTime SaleEndDate { get; }
    }

    public class XStoreCollectionData
    {
        internal XStoreCollectionData(Interop.XStoreCollectionData rawCollectionData)
        {
            this.AcquiredDate = rawCollectionData.acquiredDate.DateTime;
            this.StartDate = rawCollectionData.startDate.DateTime;
            this.EndDate = rawCollectionData.endDate.DateTime;
            this.IsTrial = rawCollectionData.isTrial;
            this.TrialTimeRemainingInSeconds = rawCollectionData.trialTimeRemainingInSeconds;
            this.Quantity = rawCollectionData.quantity;
            this.CampaignId = rawCollectionData.campaignId.GetString();
            this.DeveloperOfferId = rawCollectionData.developerOfferId.GetString();
        }

        public DateTime AcquiredDate { get; }
        public DateTime StartDate { get; }
        public DateTime EndDate { get; }
        public bool IsTrial { get; }
        public UInt32 TrialTimeRemainingInSeconds { get; }
        public UInt32 Quantity { get; }
        public string CampaignId { get; }
        public string DeveloperOfferId { get; }
    }

    public class XStoreSubscriptionInfo
    {
        internal XStoreSubscriptionInfo(Interop.XStoreSubscriptionInfo rawSubscriptionInfo)
        {
            this.HasTrialPeriod = rawSubscriptionInfo.hasTrialPeriod;
            this.TrialPeriodUnit = rawSubscriptionInfo.trialPeriodUnit;
            this.TrialPeriod = rawSubscriptionInfo.trialPeriod;
            this.BillingPeriodUnit = rawSubscriptionInfo.billingPeriodUnit;
            this.BillingPeriod = rawSubscriptionInfo.billingPeriod;
        }

        public bool HasTrialPeriod { get; }
        public XStoreDurationUnit TrialPeriodUnit { get; }
        public UInt32 TrialPeriod { get; }
        public XStoreDurationUnit BillingPeriodUnit { get; }
        public UInt32 BillingPeriod { get; }
    }

    public class XStoreImage
    {
        internal XStoreImage(Interop.XStoreImage rawStoreImage)
        {
            this.Uri = rawStoreImage.uri.GetString();
            this.Height = rawStoreImage.height;
            this.Width = rawStoreImage.width;
            this.Caption = rawStoreImage.caption.GetString();
            this.ImagePurposeTag = rawStoreImage.imagePurposeTag.GetString();
        }

        public string Uri { get; }
        public UInt32 Height { get; }
        public UInt32 Width { get; }
        public string Caption { get; }
        public string ImagePurposeTag { get; }
    }

    public class XStoreVideo
    {
        internal XStoreVideo(Interop.XStoreVideo rawVideo)
        {
            this.Uri = rawVideo.uri.GetString();
            this.Height = rawVideo.height;
            this.Width = rawVideo.width;
            this.Caption = rawVideo.caption.GetString();
            this.VideoPurposeTag = rawVideo.videoPurposeTag.GetString();
            this.PreviewImage = new XStoreImage(rawVideo.previewImage);
        }

        public string Uri { get; }
        public UInt32 Height { get; }
        public UInt32 Width { get; }
        public string Caption { get; }
        public string VideoPurposeTag { get; }
        public XStoreImage PreviewImage { get; }
    }

    public class XStoreAvailability
    {
        internal XStoreAvailability(Interop.XStoreAvailability rawAvailability)
        {
            this.AvailabilityId = rawAvailability.availabilityId.GetString();
            this.Price = new XStorePrice(rawAvailability.price);
            this.EndDate = rawAvailability.endDate.DateTime;
        }

        public string AvailabilityId { get; }
        public XStorePrice Price { get; }
        public DateTime EndDate { get; }
    }

    public class XStoreSku
    {
        internal XStoreSku(Interop.XStoreSku rawSku)
        {
            this.SkuId = rawSku.skuId.GetString();
            this.Title = rawSku.title.GetString();
            this.Description = rawSku.description.GetString();
            this.Language = rawSku.language.GetString();
            this.Price = new XStorePrice(rawSku.price);
            this.IsTrial = rawSku.isTrial;
            this.IsInUserCollection = rawSku.isInUserCollection;
            this.CollectionData = new XStoreCollectionData(rawSku.collectionData);
            this.IsSubscription = rawSku.isSubscription;
            this.SubscriptionInfo = new XStoreSubscriptionInfo(rawSku.subscriptionInfo);
            this.BundledSkus = rawSku.GetBundledSkus();
            this.Images = rawSku.GetImages(x => new XStoreImage(x));
            this.Videos = rawSku.GetVideos(x => new XStoreVideo(x));
            this.Availabilities = rawSku.GetAvailabilities(x => new XStoreAvailability(x));
        }

        public string SkuId { get; }
        public string Title { get; }
        public string Description { get; }
        public string Language { get; }
        public XStorePrice Price { get; }
        public bool IsTrial { get;  }
        public bool IsInUserCollection { get;  }
        public XStoreCollectionData CollectionData { get; }
        public bool IsSubscription { get; }
        public XStoreSubscriptionInfo SubscriptionInfo { get; }
        public string[] BundledSkus { get; }
        public XStoreImage[] Images { get; }
        public XStoreVideo[] Videos { get; }
        public XStoreAvailability[] Availabilities { get; }
    }

    public class XStoreProduct
    {
        internal XStoreProduct(Interop.XStoreProduct rawProduct)
        {
            this.StoreId = rawProduct.storeId.GetString();
            this.Title = rawProduct.title.GetString();
            this.Description = rawProduct.description.GetString();
            this.Language = rawProduct.language.GetString();
            this.InAppOfferToken = rawProduct.inAppOfferToken.GetString();
            this.LinkUri = rawProduct.linkUri.GetString();
            this.ProductKind = rawProduct.productKind;
            this.Price = new XStorePrice(rawProduct.price);
            this.HasDigitalDownload = rawProduct.hasDigitalDownload;
            this.IsInUserCollection = rawProduct.isInUserCollection;
            this.Keywords = rawProduct.GetKeywords();
            this.Skus = rawProduct.GetSkus(x => new XStoreSku(x));
            this.Images = rawProduct.GetImages(x => new XStoreImage(x));
            this.Videos = rawProduct.GetVideos(x => new XStoreVideo(x));
        }

        public string StoreId { get; }
        public string Title { get; }
        public string Description { get; }
        public string Language { get; }
        public string InAppOfferToken { get; }
        public string LinkUri { get; }
        public XStoreProductKind ProductKind { get; }
        public XStorePrice Price { get; }
        public bool HasDigitalDownload { get; }
        public bool IsInUserCollection { get; }
        public string[] Keywords { get; }
        public XStoreSku[] Skus { get; }
        public XStoreImage[] Images { get; }
        public XStoreVideo[] Videos { get; }
    }
}
