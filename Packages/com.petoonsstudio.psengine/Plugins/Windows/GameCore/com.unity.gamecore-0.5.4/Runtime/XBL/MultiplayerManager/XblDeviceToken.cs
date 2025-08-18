using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public class XblDeviceToken
    {
        internal XblDeviceToken(Interop.XblDeviceToken interopStruct)
        {
            this.Value = interopStruct.GetValue();
        }

        public string Value { get; set; }
    }
}