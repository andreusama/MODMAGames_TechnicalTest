using System;
using System.Runtime.InteropServices;

namespace Unity.GameCore.Interop
{
    //struct XStoreConsumableResult
    //{
    //    uint32_t quantity;
    //};

    [StructLayout(LayoutKind.Sequential)]
    internal struct XStoreConsumable
    {
        internal UInt32 quantity;
    }
}
