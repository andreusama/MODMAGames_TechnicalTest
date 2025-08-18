using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMultiplayerSessionRestriction : uint32_t
    //{
    //    Unknown,
    //    None,
    //    Local,
    //    Followed
    //};
    public enum XblMultiplayerSessionRestriction : UInt32
    {
        Unknown = 0,
        None = 1,
        Local = 2,
        Followed = 3,
    }
}
