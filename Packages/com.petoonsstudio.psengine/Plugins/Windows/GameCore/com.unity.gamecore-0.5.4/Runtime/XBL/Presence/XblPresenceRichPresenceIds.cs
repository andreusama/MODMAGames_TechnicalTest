using System;

namespace Unity.GameCore
{
    public class XblPresenceRichPresenceIds
    {
        private XblPresenceRichPresenceIds(string serviceConfigurationId, string presenceId, string[] presenceTokenIds)
        {
            this.ServiceConfigurationId = serviceConfigurationId;
            this.PresenceId = presenceId;
            this.PresenceTokenIds = presenceTokenIds;
        }

        public static Int32 Create(string serviceConfigurationId, string presenceId, string[] presenceTokenIds, out XblPresenceRichPresenceIds richPresenceIds)
        {
            if (!Interop.XblPresenceRichPresenceIdsRef.ValidateFields(serviceConfigurationId))
            {
                richPresenceIds = default(XblPresenceRichPresenceIds);
                return HR.E_INVALIDARG;
            }

            richPresenceIds = new XblPresenceRichPresenceIds(serviceConfigurationId, presenceId, presenceTokenIds);
            return HR.S_OK;
        }
        
        public string ServiceConfigurationId { get; }
        public string PresenceId { get; }
        public string[] PresenceTokenIds { get; }
    }
}
