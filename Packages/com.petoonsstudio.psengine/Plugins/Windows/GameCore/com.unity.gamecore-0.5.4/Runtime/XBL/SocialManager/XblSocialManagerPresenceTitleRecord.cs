using System;
using System.Linq;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblSocialManagerPresenceTitleRecord
    {
        internal XblSocialManagerPresenceTitleRecord(Interop.XblSocialManagerPresenceTitleRecord interopRecord)
        {
            this.TitleId = interopRecord.titleId;
            this.IsTitleActive = interopRecord.isTitleActive;
            this.PresenceText = Converters.ByteArrayToString(interopRecord.presenceText);
            this.IsBroadcasting = interopRecord.isBroadcasting;
            this.DeviceType = interopRecord.deviceType;
            this.IsPrimary = interopRecord.isPrimary;
        }

        public UInt32 TitleId { get; }
        public bool IsTitleActive { get; }
        public string PresenceText { get; }
        public bool IsBroadcasting { get; }
        public XblPresenceDeviceType DeviceType { get; }
        public bool IsPrimary { get; }
    }
}
