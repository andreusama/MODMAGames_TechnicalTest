using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct HC_WEBSOCKET* HCWebsocketHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct HCWebsocketHandle
    {
        internal readonly IntPtr Ptr;
    }
}
