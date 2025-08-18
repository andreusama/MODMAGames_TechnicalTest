using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XGameSaveContainer* XGameSaveContainerHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XGameSaveContainerHandle
    {
        private readonly IntPtr Ptr;
    }
}
