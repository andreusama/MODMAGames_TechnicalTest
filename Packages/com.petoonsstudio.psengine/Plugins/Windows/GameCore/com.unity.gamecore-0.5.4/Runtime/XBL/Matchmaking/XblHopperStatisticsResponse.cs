using System;

namespace Unity.GameCore
{
    public class XblHopperStatisticsResponse
    {
        internal XblHopperStatisticsResponse(Interop.XblHopperStatisticsResponse interopHandle)
        {
            this.HopperName = interopHandle.hopperName.GetString();
            this.EstimatedWaitTime = interopHandle.estimatedWaitTime;
            this.PlayersWaitingToMatch = interopHandle.playersWaitingToMatch;
        }

        public string HopperName { get; }
        public Int64 EstimatedWaitTime { get; }
        public UInt32 PlayersWaitingToMatch { get; }
    }
}
