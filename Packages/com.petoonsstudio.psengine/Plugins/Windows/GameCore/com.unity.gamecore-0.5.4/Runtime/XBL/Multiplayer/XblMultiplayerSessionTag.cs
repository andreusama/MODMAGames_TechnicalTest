using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionTag
    {
        public XblMultiplayerSessionTag(string value)
        {
            Value = value;
        }

        internal XblMultiplayerSessionTag(Interop.XblMultiplayerSessionTag interopStruct)
        {
            this.Value = interopStruct.GetValue();
        }

        public string Value { get; }
    }
}
