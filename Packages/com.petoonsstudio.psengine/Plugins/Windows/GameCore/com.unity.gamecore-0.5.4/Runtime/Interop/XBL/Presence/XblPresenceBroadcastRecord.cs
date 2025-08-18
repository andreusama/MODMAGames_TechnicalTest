using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace Unity.GameCore.Interop
{
    // typedef struct XblPresenceBroadcastRecord
    // {
    //     _Field_z_ const char* broadcastId;
    //     char session[XBL_GUID_LENGTH];
    //     XblPresenceBroadcastProvider provider;
    //     uint32_t viewerCount;
    //     time_t startTime;
    // } XblPresenceBroadcastRecord;

    [StructLayout(LayoutKind.Sequential)]
    internal struct XblPresenceBroadcastRecord
    {
        internal readonly UTF8StringPtr broadcastId;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = XblInterop.XBL_GUID_LENGTH)]
        internal readonly byte[] session;
        internal readonly XblPresenceBroadcastProvider provider;
        internal readonly UInt32 viewerCount;
        internal readonly TimeT startTime;
    }
}
