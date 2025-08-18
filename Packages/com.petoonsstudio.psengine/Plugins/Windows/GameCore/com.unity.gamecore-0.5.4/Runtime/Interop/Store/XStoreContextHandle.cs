using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XStoreContext* XStoreContextHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreContextHandle
    {
        private readonly IntPtr intPtr;
    }
}
