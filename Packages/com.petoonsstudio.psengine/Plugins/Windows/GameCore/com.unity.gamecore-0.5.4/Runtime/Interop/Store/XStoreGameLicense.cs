using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //#define TRIAL_UNIQUE_ID_MAX_SIZE (64)
    //#define STORE_SKU_ID_SIZE (18)
    //
    //struct XStoreGameLicense
    //{
    //    _Field_z_ char skuStoreId[STORE_SKU_ID_SIZE];
    //    bool isActive;
    //    bool isTrialOwnedByThisUser;
    //    bool isDiscLicense;
    //    bool isTrial;
    //    uint32_t trialTimeRemainingInSeconds;
    //    _Field_z_ char trialUniqueId[TRIAL_UNIQUE_ID_MAX_SIZE];
    //    time_t expirationDate;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreGameLicense
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        internal Byte [] skuStoreId;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isActive;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isTrialOwnedByThisUser;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isDiscLicense;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isTrial;
        internal UInt32 trialTimeRemainingInSeconds;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal Byte[] trialUniqueId;
        internal TimeT expirationDate;
    }
}
