using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionReferenceUri
    {
        internal XblMultiplayerSessionReferenceUri(Interop.XblMultiplayerSessionReferenceUri interopStruct)
        {
            this.Value = interopStruct.GetValue();
        }

        public string Value { get; }
    }
}
