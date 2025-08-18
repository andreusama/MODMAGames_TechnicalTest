using System;

namespace Unity.GameCore
{
    public class XblRequestedStatistics
    {
        private XblRequestedStatistics(string serviceConfigurationId, string[] statistics)
        {
            this.ServiceConfigurationId = serviceConfigurationId;
            this.Statistics = statistics;
        }

        public static Int32 Create(string serviceConfigurationId, string[] statistics, out XblRequestedStatistics requestedStatistics)
        {
            if (!Interop.XblRequestedStatistics.ValidateFields(serviceConfigurationId))
            {
                requestedStatistics = default(XblRequestedStatistics);
                return HR.E_INVALIDARG;
            }

            requestedStatistics = new XblRequestedStatistics(serviceConfigurationId, statistics);
            return HR.S_OK;
        }

        public string ServiceConfigurationId { get; }
        public string[] Statistics { get; }
    }
}
