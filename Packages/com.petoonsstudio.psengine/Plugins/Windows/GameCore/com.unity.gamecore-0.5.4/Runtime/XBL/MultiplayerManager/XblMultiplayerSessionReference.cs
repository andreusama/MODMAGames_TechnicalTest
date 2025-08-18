using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionReference
    {
        internal XblMultiplayerSessionReference(Interop.XblMultiplayerSessionReference interopStruct)
        {
            this.Scid = interopStruct.GetScid();
            this.SessionTemplateName = interopStruct.GetSessionTemplateName();
            this.SessionName = interopStruct.GetSessionName();
        }

        public string Scid { get; }
        public string SessionTemplateName { get; }
        public string SessionName { get; }
    }
}