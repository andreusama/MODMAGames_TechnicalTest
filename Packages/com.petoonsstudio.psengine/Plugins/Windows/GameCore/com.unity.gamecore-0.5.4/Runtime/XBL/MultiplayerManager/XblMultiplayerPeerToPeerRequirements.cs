using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerPeerToPeerRequirements
    {
        internal XblMultiplayerPeerToPeerRequirements(Interop.XblMultiplayerPeerToPeerRequirements interopStruct)
        {
            this.LatencyMaximum = interopStruct.LatencyMaximum;
            this.BandwidthMinimumInKbps = interopStruct.BandwidthMinimumInKbps;
        }

        public UInt64 LatencyMaximum { get; }
        public UInt64 BandwidthMinimumInKbps { get; }
    }
}