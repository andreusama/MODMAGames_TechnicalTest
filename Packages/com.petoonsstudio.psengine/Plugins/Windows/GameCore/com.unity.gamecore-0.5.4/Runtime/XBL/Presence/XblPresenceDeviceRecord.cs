using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblPresenceDeviceRecord
    {
        internal XblPresenceDeviceRecord(Interop.XblPresenceDeviceRecord interopRecord)
        {
            this.DeviceType = interopRecord.deviceType;
            this.TitleRecords = interopRecord.GetTitleRecords(tr => new XblPresenceTitleRecord(tr));
        }

        public XblPresenceDeviceType DeviceType { get; }
        public XblPresenceTitleRecord[] TitleRecords { get; }
    }
}
