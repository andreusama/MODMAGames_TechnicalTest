using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblServiceConfigurationStatistic
    {
        internal XblServiceConfigurationStatistic(Interop.XblServiceConfigurationStatistic interopStatistic)
        {
            this.ServiceConfigurationId = Interop.Converters.ByteArrayToString(interopStatistic.serviceConfigurationId);
            this.Statistics = interopStatistic.GetStatistics(s => new XblStatistic(s));
        }

        public string ServiceConfigurationId { get; }
        public XblStatistic[] Statistics { get; }
    }
}
