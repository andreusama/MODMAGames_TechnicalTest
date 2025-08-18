using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerSearchHandleDetails* XblMultiplayerSearchHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerSearchHandle
    {
        private readonly IntPtr Ptr;
    }
}
