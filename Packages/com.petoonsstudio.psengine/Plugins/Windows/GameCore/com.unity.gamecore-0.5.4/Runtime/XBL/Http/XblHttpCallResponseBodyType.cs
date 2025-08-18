using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblHttpCallResponseBodyType : uint32_t
    //{
    //    String,
    //    Vector
    //};
    public enum XblHttpCallResponseBodyType : UInt32
    {
        String = 0,
        Vector = 1,
    }
}