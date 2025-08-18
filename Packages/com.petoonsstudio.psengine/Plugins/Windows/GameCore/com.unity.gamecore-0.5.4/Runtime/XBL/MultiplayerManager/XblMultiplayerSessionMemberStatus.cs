using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMultiplayerSessionMemberStatus : uint32_t
    //{
    //    Reserved,
    //    Inactive,
    //    Ready,
    //    Active
    //};
    public enum XblMultiplayerSessionMemberStatus : UInt32
    {
        Reserved = 0,
        Inactive = 1,
        Ready = 2,
        Active = 3,
    }
}