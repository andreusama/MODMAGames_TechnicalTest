using System;
using System.Collections.Generic;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XStoreAddonLicense
    {
        internal XStoreAddonLicense(Interop.XStoreAddonLicense interopLicense)
        {
            SkuStoreId = Converters.ByteArrayToString(interopLicense.skuStoreId);
            InAppOfferToken = Converters.ByteArrayToString(interopLicense.inAppOfferToken);
            IsActive = interopLicense.isActive;
            ExpirationDate = interopLicense.expirationDate.DateTime;
        }

        public string SkuStoreId { get; }
        public string InAppOfferToken { get; }
        public bool IsActive { get; }
        public DateTime ExpirationDate { get; }
    }
}
