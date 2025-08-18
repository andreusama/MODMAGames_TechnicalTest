using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore
{
    [StructLayout(LayoutKind.Sequential)]
    public struct XUserLocalId
    {
        public XUserLocalId(UInt64 value)
        {
            this.value = value;
        }

        public readonly UInt64 value;
    }
}
