using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Unity.GameCore
{
    public class XblStatistic
    {
        internal XblStatistic(Interop.XblStatistic interopStatistic)
        {
            this.StatisticName = interopStatistic.statisticName.GetString();
            this.StatisticType = interopStatistic.statisticType.GetString();
            this.Value = interopStatistic.value.GetString();
        }

        public string StatisticName { get; }
        public string StatisticType { get; }
        public string Value { get; }
    }
}
