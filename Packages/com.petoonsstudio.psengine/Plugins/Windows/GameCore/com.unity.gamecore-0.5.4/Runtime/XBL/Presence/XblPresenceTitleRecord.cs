using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblPresenceTitleRecord
    {
        internal XblPresenceTitleRecord(Interop.XblPresenceTitleRecord interopRecord)
        {
            this.TitleId = interopRecord.titleId;
            this.TitleName = interopRecord.titleName.GetString();
            this.LastModified = interopRecord.lastModified.DateTime;
            this.TitleActive = interopRecord.titleActive;
            this.RichPresenceString = interopRecord.richPresenceString.GetString();
            this.ViewState = interopRecord.viewState;
            this.BroadcastRecord = interopRecord.GetBroadcastRecord(br => new XblPresenceBroadcastRecord(br));
        }

        public UInt32 TitleId { get; }
        public string TitleName { get; }
        public DateTime LastModified { get; }
        public bool TitleActive { get; }
        public string RichPresenceString { get; }
        public XblPresenceTitleViewState ViewState { get; }
        public XblPresenceBroadcastRecord BroadcastRecord { get; }
    }
}
