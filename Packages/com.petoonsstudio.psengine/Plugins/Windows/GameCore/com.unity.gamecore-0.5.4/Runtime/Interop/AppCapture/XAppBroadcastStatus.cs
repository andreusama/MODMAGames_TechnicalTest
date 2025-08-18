using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XAppBroadcastStatus
    //{
    //    bool canStartBroadcast;
    //    bool isAnyAppBroadcasting;
    //    bool isCaptureResourceUnavailable;
    //    bool isGameStreamInProgress;
    //    bool isGpuConstrained;
    //    bool isAppInactive;
    //    bool isBlockedForApp;
    //    bool isDisabledByUser;
    //    bool isDisabledBySystem;
    //};
    [StructLayout(LayoutKind.Sequential)]
    struct XAppBroadcastStatus
    {
        internal NativeBool canStartBroadcast;
        internal NativeBool isAnyAppBroadcasting;
        internal NativeBool isCaptureResourceUnavailable;
        internal NativeBool isGameStreamInProgress;
        internal NativeBool isGpuConstrained;
        internal NativeBool isAppInactive;
        internal NativeBool isBlockedForApp;
        internal NativeBool isDisabledByUser;
        internal NativeBool isDisabledBySystem;
    };
}
