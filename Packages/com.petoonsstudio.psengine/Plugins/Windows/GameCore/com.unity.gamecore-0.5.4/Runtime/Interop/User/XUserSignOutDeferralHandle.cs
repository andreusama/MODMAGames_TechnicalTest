using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XUserSignOutDeferral* XUserSignOutDeferralHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XUserSignOutDeferralHandle
    {
        private readonly IntPtr Ptr;
    }
}
