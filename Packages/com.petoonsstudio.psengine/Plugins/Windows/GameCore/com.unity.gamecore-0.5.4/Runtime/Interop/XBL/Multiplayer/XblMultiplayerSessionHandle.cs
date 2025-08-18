using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerSession* XblMultiplayerSessionHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerSessionHandle
    {
        private readonly IntPtr intPtr;
    }
}
