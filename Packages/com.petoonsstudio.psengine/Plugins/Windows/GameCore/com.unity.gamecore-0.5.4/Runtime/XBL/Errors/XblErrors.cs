using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    public partial class SDK
    {
        public partial class XBL
        {
            public static XblErrorCondition XblGetErrorCondition(Int32 hr)
            {
                return XblInterop.XblGetErrorCondition(hr);
            }
        }
    }
}
