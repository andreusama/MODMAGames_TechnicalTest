using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblCreateMatchTicketResponse
    {
        internal XblCreateMatchTicketResponse(Interop.XblCreateMatchTicketResponse interopHandle)
        {
            this.MatchTicketId = Converters.ByteArrayToString(interopHandle.matchTicketId);
            this.EstimatedWaitTime = interopHandle.estimatedWaitTime;
        }

        public string MatchTicketId { get; }

        public Int64 EstimatedWaitTime { get; }
    }
}
