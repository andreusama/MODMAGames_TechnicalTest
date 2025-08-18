using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblPresenceRecord* XblPresenceRecordHandle;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblPresenceRecordHandle
    {
        private readonly IntPtr intPtr;
    }
}
