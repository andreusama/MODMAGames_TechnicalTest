using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    /*
        struct XPackageDetails
        {
            _Field_z_ const char* packageIdentifier;
            XVersion version;
            XPackageKind kind;
            _Field_z_ const char* displayName;
            _Field_z_ const char* description;
            _Field_z_ const char* publisher;
            _Field_z_ const char* storeId;
            bool installing;
        };
     */
    [StructLayout(LayoutKind.Sequential)]
    internal struct XPackageDetails
    {
        internal UTF8StringPtr packageIdentifier;
        internal XVersion version;
        internal XPackageKind kind;
        internal UTF8StringPtr displayName;
        internal UTF8StringPtr description;
        internal UTF8StringPtr publisher;
        internal UTF8StringPtr storeId;
        [MarshalAs(UnmanagedType.U1)]
        internal bool installing;
    }
}
