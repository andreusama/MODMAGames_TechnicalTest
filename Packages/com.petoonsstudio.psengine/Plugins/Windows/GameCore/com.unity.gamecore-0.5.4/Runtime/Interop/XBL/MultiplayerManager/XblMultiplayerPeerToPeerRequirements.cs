using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //typedef struct XblMultiplayerPeerToPeerRequirements
    //{
    //    uint64_t LatencyMaximum;
    //    uint64_t BandwidthMinimumInKbps;
    //} XblMultiplayerPeerToPeerRequirements;
    [StructLayout(LayoutKind.Sequential)]
    internal struct XblMultiplayerPeerToPeerRequirements
    {
        internal readonly UInt64 LatencyMaximum;
        internal readonly UInt64 BandwidthMinimumInKbps;

        internal XblMultiplayerPeerToPeerRequirements(Unity.GameCore.XblMultiplayerPeerToPeerRequirements publicObject)
        {
            this.LatencyMaximum = publicObject.LatencyMaximum;
            this.BandwidthMinimumInKbps = publicObject.BandwidthMinimumInKbps;
        }
    }
}