using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct XPackageMountHandle
    {
        internal IntPtr handle;
    }
}
