using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    // typedef struct XblTitleStorageBlobMetadataResult* XblTitleStorageBlobMetadataResultHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblTitleStorageBlobMetadataResultHandle
    {
        private readonly IntPtr intPtr;
    }
}