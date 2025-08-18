using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMultiplayerSessionStatus : uint32_t
    //{
    //    Unknown,
    //    Active,
    //    Inactive,
    //    Reserved
    //};
    public enum XblMultiplayerSessionStatus : UInt32
    {
        Unknown,
        Active,
        Inactive,
        Reserved
    }
}
