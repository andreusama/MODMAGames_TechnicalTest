using System;

namespace Unity.GameCore
{
    public struct XAppBroadcastStatus
    {
        public bool canStartBroadcast { get; }
        public bool isAnyAppBroadcasting { get; }
        public bool isCaptureResourceUnavailable { get; }
        public bool isGameStreamInProgress { get; }
        public bool isGpuConstrained { get; }
        public bool isAppInactive { get; }
        public bool isBlockedForApp { get; }
        public bool isDisabledByUser { get; }
        public bool isDisabledBySystem { get; }

        internal XAppBroadcastStatus(Interop.XAppBroadcastStatus interop)
        {
            canStartBroadcast = interop.canStartBroadcast.Value;
            isAnyAppBroadcasting = interop.isAnyAppBroadcasting.Value;
            isCaptureResourceUnavailable = interop.isCaptureResourceUnavailable.Value;
            isGameStreamInProgress = interop.isGameStreamInProgress.Value;
            isGpuConstrained = interop.isGpuConstrained.Value;
            isAppInactive = interop.isAppInactive.Value;
            isBlockedForApp = interop.isBlockedForApp.Value;
            isDisabledByUser = interop.isDisabledByUser.Value;
            isDisabledBySystem = interop.isDisabledBySystem.Value;
        }
    }
}
