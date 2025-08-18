using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblGuid
    {
        internal XblGuid(Interop.XblGuid interopStruct)
        {
            this.Value = interopStruct.GetValue();
        }

        public string Value { get; }
    }
}