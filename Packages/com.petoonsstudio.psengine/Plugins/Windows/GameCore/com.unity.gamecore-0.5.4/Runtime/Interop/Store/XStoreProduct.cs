using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //#define PRICE_MAX_SIZE (16)
    //
    //struct XStorePrice
    //{
    //    float basePrice;
    //    float price;
    //    float recurrencePrice;
    //    _Field_z_ const char* currencyCode;
    //    _Field_z_ char formattedBasePrice[PRICE_MAX_SIZE];
    //    _Field_z_ char formattedPrice[PRICE_MAX_SIZE];
    //    _Field_z_ char formattedRecurrencePrice[PRICE_MAX_SIZE];
    //    bool isOnSale;
    //    time_t saleEndDate;
    //};

    [StructLayout(LayoutKind.Sequential)]
    struct XStorePrice
    {
        internal float basePrice;
        internal float price;
        internal float recurrencePrice;
        internal UTF8StringPtr currencyCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal Byte[] formattedBasePrice;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal Byte[] formattedPrice;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        internal Byte[] formattedRecurrencePrice;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isOnSale;
        internal TimeT saleEndDate;

        internal XStorePrice(Unity.GameCore.XStorePrice publicObject, DisposableCollection disposableCollection)
        {
            this.basePrice = publicObject.BasePrice;
            this.price = publicObject.Price;
            this.recurrencePrice = publicObject.RecurrencePrice;
            this.currencyCode = new UTF8StringPtr(publicObject.CurrencyCode, disposableCollection);
            this.formattedBasePrice = Converters.StringToNullTerminatedUTF8ByteArray(publicObject.FormattedBasePrice);
            this.formattedPrice = Converters.StringToNullTerminatedUTF8ByteArray(publicObject.FormattedPrice);
            this.formattedRecurrencePrice = Converters.StringToNullTerminatedUTF8ByteArray(publicObject.FormattedRecurrencePrice);
            this.isOnSale = publicObject.IsOnSale;
            this.saleEndDate = new TimeT((Int64)(publicObject.SaleEndDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
    }

    //struct XStoreCollectionData
    //{
    //    time_t acquiredDate;
    //    time_t startDate;
    //    time_t endDate;
    //    bool isTrial;
    //    uint32_t trialTimeRemainingInSeconds;
    //    uint32_t quantity;
    //    _Field_z_ const char* campaignId;
    //    _Field_z_ const char* developerOfferId;
    //};

    [StructLayout(LayoutKind.Sequential)]
    struct XStoreCollectionData
    {
        internal TimeT acquiredDate;
        internal TimeT startDate;
        internal TimeT endDate;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isTrial;
        internal UInt32 trialTimeRemainingInSeconds;
        internal UInt32 quantity;
        internal UTF8StringPtr campaignId;
        internal UTF8StringPtr developerOfferId;
    }

    //struct XStoreSubscriptionInfo
    //{
    //    bool hasTrialPeriod;
    //    XStoreDurationUnit trialPeriodUnit;
    //    uint32_t trialPeriod;
    //    XStoreDurationUnit billingPeriodUnit;
    //    uint32_t billingPeriod;
    //};

    [StructLayout(LayoutKind.Sequential)]
    struct XStoreSubscriptionInfo
    {
        [MarshalAs(UnmanagedType.U1)]
        internal bool hasTrialPeriod;
        internal XStoreDurationUnit trialPeriodUnit;
        internal UInt32 trialPeriod;
        internal XStoreDurationUnit billingPeriodUnit;
        internal UInt32 billingPeriod;
    }

    //struct XStoreImage
    //{
    //    _Field_z_ const char* uri;
    //    uint32_t height;
    //    uint32_t width;
    //    _Field_z_ const char* caption;
    //    _Field_z_ const char* imagePurposeTag;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XStoreImage
    {
        internal UTF8StringPtr uri;
        internal UInt32 height;
        internal UInt32 width;
        internal UTF8StringPtr caption;
        internal UTF8StringPtr imagePurposeTag;
    }

    //struct XStoreVideo
    //{
    //    _Field_z_ const char* uri;
    //    uint32_t height;
    //    uint32_t width;
    //    _Field_z_ const char* caption;
    //    _Field_z_ const char* videoPurposeTag;
    //    XStoreImage previewImage;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XStoreVideo
    {
        internal UTF8StringPtr uri;
        internal UInt32 height;
        internal UInt32 width;
        internal UTF8StringPtr caption;
        internal UTF8StringPtr videoPurposeTag;
        internal XStoreImage previewImage;
    }

    //struct XStoreAvailability
    //{
    //    _Field_z_ const char* availabilityId;
    //    XStorePrice price;
    //    time_t endDate;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XStoreAvailability
    {
        internal UTF8StringPtr availabilityId;
        internal XStorePrice price;
        internal TimeT endDate;

        public XStoreAvailability(Unity.GameCore.XStoreAvailability publicObject, DisposableCollection disposableCollection)
        {
            this.availabilityId = new UTF8StringPtr(publicObject.AvailabilityId, disposableCollection);
            this.price = new XStorePrice(publicObject.Price, disposableCollection);
            this.endDate = new TimeT((Int64)(publicObject.EndDate - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds);
        }
    }

    //struct XStoreSku
    //{
    //    _Field_z_ const char* skuId;
    //    _Field_z_ const char* title;
    //    _Field_z_ const char* description;
    //    _Field_z_ const char* language;
    //    XStorePrice price;
    //    bool isTrial;
    //    bool isInUserCollection;
    //    XStoreCollectionData collectionData;
    //    bool isSubscription;
    //    XStoreSubscriptionInfo subscriptionInfo;
    //    uint32_t bundledSkusCount;
    //    _Field_z_ const char** bundledSkus;
    //    uint32_t imagesCount;
    //    XStoreImage* images;
    //    uint32_t videosCount;
    //    XStoreVideo* videos;
    //    uint32_t availabilitiesCount;
    //    XStoreAvailability* availabilities;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XStoreSku
    {
        internal string[] GetBundledSkus() => Converters.PtrToClassArray<string, UTF8StringPtr>(this.bundledSkus, this.bundledSkusCount, s => s.GetString());
        internal T[] GetImages<T>(Func<XStoreImage, T> ctor) => Converters.PtrToClassArray(this.images, this.imagesCount, ctor);
        internal T[] GetVideos<T>(Func<XStoreVideo, T> ctor) => Converters.PtrToClassArray(this.videos, this.videosCount, ctor);
        internal T[] GetAvailabilities<T>(Func<XStoreAvailability, T> ctor) => Converters.PtrToClassArray(this.availabilities, this.availabilitiesCount, ctor);

        internal UTF8StringPtr skuId;
        internal UTF8StringPtr title;
        internal UTF8StringPtr description;
        internal UTF8StringPtr language;
        internal XStorePrice price;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isTrial;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isInUserCollection;
        internal XStoreCollectionData collectionData;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isSubscription;
        internal XStoreSubscriptionInfo subscriptionInfo;
        private UInt32 bundledSkusCount;
        private IntPtr bundledSkus;
        private UInt32 imagesCount;
        private IntPtr images;
        private UInt32 videosCount;
        private IntPtr videos;
        private UInt32 availabilitiesCount;
        private IntPtr availabilities;
    }

    //struct XStoreProduct
    //{
    //    _Field_z_ const char* storeId;
    //    _Field_z_ const char* title;
    //    _Field_z_ const char* description;
    //    _Field_z_ const char* language;
    //    _Field_z_ const char* inAppOfferToken;
    //    _Field_z_ char* linkUri;
    //    XStoreProductKind productKind;
    //    XStorePrice price;
    //    bool hasDigitalDownload;
    //    bool isInUserCollection;
    //    uint32_t keywordsCount;
    //    _Field_z_ const char** keywords;
    //    uint32_t skusCount;
    //    XStoreSku* skus;
    //    uint32_t imagesCount;
    //    XStoreImage* images;
    //    uint32_t videosCount;
    //    XStoreVideo* videos;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XStoreProduct
    {
        internal string[] GetKeywords() => Converters.PtrToClassArray<string, UTF8StringPtr>(this.keywords, this.keywordsCount, str => str.GetString());
        internal T[] GetSkus<T>(Func<XStoreSku, T> ctor) => Converters.PtrToClassArray(this.skus, this.skusCount, ctor);
        internal T[] GetImages<T>(Func<XStoreImage, T> ctor) => Converters.PtrToClassArray(this.images, this.imagesCount, ctor);
        internal T[] GetVideos<T>(Func<XStoreVideo, T> ctor) => Converters.PtrToClassArray(this.videos, this.videosCount, ctor);

        internal UTF8StringPtr storeId;
        internal UTF8StringPtr title;
        internal UTF8StringPtr description;
        internal UTF8StringPtr language;
        internal UTF8StringPtr inAppOfferToken;
        internal UTF8StringPtr linkUri;
        internal XStoreProductKind productKind;
        internal XStorePrice price;
        [MarshalAs(UnmanagedType.U1)]
        internal bool hasDigitalDownload;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isInUserCollection;
        private UInt32 keywordsCount;
        private IntPtr keywords;
        private UInt32 skusCount;
        private IntPtr skus;
        private UInt32 imagesCount;
        private IntPtr images;
        private UInt32 videosCount;
        private IntPtr videos;
    }
}
