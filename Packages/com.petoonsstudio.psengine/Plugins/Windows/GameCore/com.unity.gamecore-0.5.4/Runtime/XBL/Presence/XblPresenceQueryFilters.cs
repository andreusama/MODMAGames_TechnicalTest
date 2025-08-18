using System;

namespace Unity.GameCore
{
    public class XblPresenceQueryFilters
    {
        private XblPresenceQueryFilters(
            XblPresenceDeviceType[] deviceTypes,
            UInt32[] titleIds,
            XblPresenceDetailLevel detailLevel,
            bool onlineOnly,
            bool broadcastingOnly)
        {
            this.DeviceTypes = deviceTypes;
            this.TitleIds = titleIds;
            this.DetailLevel = detailLevel;
            this.OnlineOnly = onlineOnly;
            this.BroadcastingOnly = broadcastingOnly;
        }

        public static Int32 Create(
            XblPresenceDeviceType[] deviceTypes,
            UInt32[] titleIds,
            XblPresenceDetailLevel detailLevel,
            bool onlineOnly,
            bool broadcastingOnly,
            out XblPresenceQueryFilters queryFilters)
        {
            queryFilters = new XblPresenceQueryFilters(
                deviceTypes,
                titleIds,
                detailLevel,
                onlineOnly,
                broadcastingOnly);

            return HR.S_OK;
        }

        public XblPresenceDeviceType[] DeviceTypes { get; }
        public UInt32[] TitleIds { get; }
        public XblPresenceDetailLevel DetailLevel { get; }
        public bool OnlineOnly { get; }
        public bool BroadcastingOnly { get; }
    }
}
