using System;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionQueryResult
    {
        internal XblMultiplayerSessionQueryResult(Interop.XblMultiplayerSessionQueryResult interopHandle)
        {
            this.StartTime = interopHandle.StartTime.DateTime;
            this.SessionReference = new XblMultiplayerSessionReference(interopHandle.SessionReference);
            this.Status = interopHandle.Status;
            this.Visibility = interopHandle.Visibility;
            this.IsMyTurn = interopHandle.IsMyTurn;
            this.Xuid = interopHandle.Xuid;
            this.AcceptedMemberCount = interopHandle.AcceptedMemberCount;
            this.JoinRestriction = interopHandle.JoinRestriction;
        }

        public DateTime StartTime { get; }

        public XblMultiplayerSessionReference SessionReference { get; }

        public XblMultiplayerSessionStatus Status { get; }

        public XblMultiplayerSessionVisibility Visibility { get; }

        public bool IsMyTurn { get; }

        public UInt64 Xuid { get; }

        public UInt32 AcceptedMemberCount { get; }

        public XblMultiplayerSessionRestriction JoinRestriction { get; }
    }
}
