using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XGameSaveProvider* XGameSaveProviderHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XGameSaveProviderHandle
    {
        private readonly IntPtr Ptr;
    }
}
