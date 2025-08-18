﻿using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore
{
    public class XblStatisticChangeEventArgs
    {
        internal XblStatisticChangeEventArgs(Interop.XblStatisticChangeEventArgs interopStruct)
        {
            this.XboxUserId = interopStruct.xboxUserId;
            this.ServiceConfigurationId = interopStruct.serviceConfigurationId;
            this.LatestStatistic = new XblStatistic(interopStruct.latestStatistic);
        }

        public UInt64 XboxUserId { get; }
        public byte[] ServiceConfigurationId { get; }
        public XblStatistic LatestStatistic { get; }
    }
}
