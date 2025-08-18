using System;
using System.Runtime.InteropServices;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerActivityRecentPlayerUpdate
    {
        public XblMultiplayerActivityRecentPlayerUpdate()
        {

        }

        public UInt64 Xuid { get; set; }

        public XblMultiplayerActivityEncounterType EncounterType { get; set; }
    }
}
