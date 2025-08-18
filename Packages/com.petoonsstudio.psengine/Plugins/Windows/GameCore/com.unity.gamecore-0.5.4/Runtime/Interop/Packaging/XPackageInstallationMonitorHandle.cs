using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XPackageInstallationMonitorHandle
    {
        internal IntPtr handle;
    }
}
