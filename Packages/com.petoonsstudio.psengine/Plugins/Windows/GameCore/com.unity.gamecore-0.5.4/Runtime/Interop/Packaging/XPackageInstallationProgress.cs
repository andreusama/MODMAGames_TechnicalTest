using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XPackageInstallationProgress
    //{
    //    uint64_t totalBytes;
    //    uint64_t installedBytes;
    //    uint64_t launchBytes;
    //    bool launchable;
    //    bool completed;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XPackageInstallationProgress
    {
        internal UInt64 totalBytes;
        internal UInt64 installedBytes;
        internal UInt64 launchBytes;
        [MarshalAs(UnmanagedType.U1)]
        internal bool launchable;
        [MarshalAs(UnmanagedType.U1)]
        internal bool completed;
    }
}
