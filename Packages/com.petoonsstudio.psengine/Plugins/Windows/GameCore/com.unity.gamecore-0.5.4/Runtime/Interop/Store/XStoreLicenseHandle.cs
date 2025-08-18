using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XStoreLicense* XStoreLicenseHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreLicenseHandle
    {
        private readonly IntPtr intPtr;
    }
}
