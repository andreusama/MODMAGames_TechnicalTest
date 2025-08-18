using System;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionInitArgs
    {
        public XblMultiplayerSessionInitArgs()
        {
        }

        public UInt32 MaxMembersInSession { get; set; }
        public  XblMultiplayerSessionVisibility Visibility { get; set; }
        public UInt64[] InitiatorXuids { get; set; }
        public string CustomJson { get; set; }
    }
}
