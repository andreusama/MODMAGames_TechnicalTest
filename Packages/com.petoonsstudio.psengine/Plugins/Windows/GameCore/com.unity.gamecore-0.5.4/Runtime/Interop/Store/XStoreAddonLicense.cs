using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //#define STORE_SKU_ID_SIZE (18)
    //#define IN_APP_OFFER_TOKEN_MAX_SIZE (64)
    //
    //struct XStoreAddonLicense
    //{
    //    _Field_z_ char skuStoreId[STORE_SKU_ID_SIZE];
    //    _Field_z_ char inAppOfferToken[IN_APP_OFFER_TOKEN_MAX_SIZE];
    //    bool isActive;
    //    time_t expirationDate;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreAddonLicense
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        internal Byte[] skuStoreId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal Byte[] inAppOfferToken;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isActive;
        internal TimeT expirationDate;
    }
}
