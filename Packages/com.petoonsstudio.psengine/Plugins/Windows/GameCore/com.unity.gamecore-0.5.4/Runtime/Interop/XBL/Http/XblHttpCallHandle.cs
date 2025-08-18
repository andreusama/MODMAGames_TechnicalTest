using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    /// <summary>
    /// Handle to an Xbox live context. Needed to interact with Xbox live services.
    /// </summary>
    //typedef struct XblContext* XblContextHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblHttpCallHandle
    {
        private readonly IntPtr handle;
    }
}
