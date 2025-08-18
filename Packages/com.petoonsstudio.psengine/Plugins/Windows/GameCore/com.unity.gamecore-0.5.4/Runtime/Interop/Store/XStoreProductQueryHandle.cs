using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XStoreProductQuery* XStoreProductQueryHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreProductQueryHandle
    {
        private readonly IntPtr intPtr;
    }
}
