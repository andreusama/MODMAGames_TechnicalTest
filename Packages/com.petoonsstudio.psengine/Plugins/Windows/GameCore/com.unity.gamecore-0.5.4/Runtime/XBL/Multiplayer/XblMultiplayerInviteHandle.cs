using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerInviteHandle
    {
        internal XblMultiplayerInviteHandle(Interop.XblMultiplayerInviteHandle interopStruct)
        {
            this.Data = interopStruct.GetData();
        }

        public string Data { get; }
    }
}