using System;

namespace Unity.GameCore
{
    public class XblMultiplayerMatchmakingServer
    {
        internal XblMultiplayerMatchmakingServer(Interop.XblMultiplayerMatchmakingServer interopHandle)
        {
            this.InteropHandle = interopHandle;
            this.Status = interopHandle.Status;
            this.StatusDetails = interopHandle.StatusDetails.GetString();
            this.TypicalWaitInSeconds = interopHandle.TypicalWaitInSeconds;
            this.TargetSessionRef = new XblMultiplayerSessionReference(interopHandle.TargetSessionRef);
        }

        public XblMatchmakingStatus Status { get; }
        public string StatusDetails { get; }
        public UInt32 TypicalWaitInSeconds { get; }
        public XblMultiplayerSessionReference TargetSessionRef { get; }

        internal Interop.XblMultiplayerMatchmakingServer InteropHandle { get; }

    }
}