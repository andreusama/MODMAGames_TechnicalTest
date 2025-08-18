using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XStorePackageUpdate
    //{
    //    _Field_z_ char packageIdentifier[XPACKAGE_IDENTIFIER_MAX_LENGTH];
    //    bool isMandatory;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XStorePackageUpdate
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XGRInterop.XPACKAGE_IDENTIFIER_MAX_LENGTH)]
        internal Byte[] packageIdentifier;
        [MarshalAs(UnmanagedType.U1)]
        internal bool isMandatory;
    }
}
