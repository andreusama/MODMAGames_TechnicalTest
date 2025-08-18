using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblMultiplayerSessionNumberAttribute
    {
        public XblMultiplayerSessionNumberAttribute(string name, double value)
        {
            Name = name;
            Value = value;
        }

        internal XblMultiplayerSessionNumberAttribute(Interop.XblMultiplayerSessionNumberAttribute interopStruct)
        {
            this.Name = interopStruct.GetName();
            this.Value = interopStruct.value;
        }

        public string Name { get; }
        public double Value { get; }
    }
}
