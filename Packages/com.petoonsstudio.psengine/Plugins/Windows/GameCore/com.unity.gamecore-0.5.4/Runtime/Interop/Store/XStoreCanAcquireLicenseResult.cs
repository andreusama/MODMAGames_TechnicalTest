using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //#define SKU_ID_SIZE (5)
    //
    //struct XStoreCanAcquireLicenseResult
    //{
    //    _Field_z_ char licensableSku[SKU_ID_SIZE];
    //    XStoreCanLicenseStatus status;
    //};

    internal struct XStoreCanAcquireLicenseResult
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        internal Byte[] licensableSku;
        internal XStoreCanLicenseStatus status;
    }
}
