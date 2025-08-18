using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblPresenceBroadcastRecord
    {
        internal XblPresenceBroadcastRecord(Interop.XblPresenceBroadcastRecord interopRecord)
        {
            this.BroadcastId = interopRecord.broadcastId.GetString();
            this.Session = Converters.ByteArrayToString(interopRecord.session);
            this.Provider = interopRecord.provider;
            this.ViewerCount = interopRecord.viewerCount;
            this.StartTime = interopRecord.startTime.DateTime;
        }

        public string BroadcastId { get; }
        public string Session { get; }
        public XblPresenceBroadcastProvider Provider { get; }
        public UInt32 ViewerCount { get; }
        public DateTime StartTime { get; }
    }
}
