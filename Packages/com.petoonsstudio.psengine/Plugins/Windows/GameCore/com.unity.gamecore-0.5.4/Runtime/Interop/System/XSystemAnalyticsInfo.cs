using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XSystemAnalyticsInfo
    //{
    //    XVersion osVersion;
    //    XVersion hostingOsVersion;
    //    _Field_z_ char family[64];
    //    _Field_z_ char form[64];
    //};

    [StructLayout(LayoutKind.Sequential)]
    public struct XSystemAnalyticsInfo
    {
        internal XVersion osVersion;
        internal XVersion hostingOsVersion;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal Byte[] family;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 64)]
        internal Byte[] form;
    }
}
