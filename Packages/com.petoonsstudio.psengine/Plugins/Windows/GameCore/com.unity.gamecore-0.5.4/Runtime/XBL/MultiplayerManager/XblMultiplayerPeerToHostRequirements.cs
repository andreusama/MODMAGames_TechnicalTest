using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerPeerToHostRequirements
    {
        internal XblMultiplayerPeerToHostRequirements(Interop.XblMultiplayerPeerToHostRequirements interopStruct)
        {
            this.LatencyMaximum = interopStruct.LatencyMaximum;
            this.BandwidthDownMinimumInKbps = interopStruct.BandwidthDownMinimumInKbps;
            this.BandwidthUpMinimumInKbps = interopStruct.BandwidthUpMinimumInKbps;
            this.HostSelectionMetric = interopStruct.HostSelectionMetric;
        }

        public UInt64 LatencyMaximum { get; }
        public UInt64 BandwidthDownMinimumInKbps { get; }
        public UInt64 BandwidthUpMinimumInKbps { get; }
        public XblMultiplayerMetrics HostSelectionMetric { get; }
    }
}