using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblUserStatisticsResult
    {
        internal XblUserStatisticsResult(Interop.XblUserStatisticsResult interopResult)
        {
            this.XboxUserId = interopResult.xboxUserId;
            this.ServiceConfigStatistics = interopResult.GetServiceConfigStatistics(scs => new XblServiceConfigurationStatistic(scs));
        }

        public UInt64 XboxUserId { get; }
        public XblServiceConfigurationStatistic[] ServiceConfigStatistics { get; }
    }
}
