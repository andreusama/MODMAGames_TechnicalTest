using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMatchmakingStatus : uint32_t
    //{
    //	Unknown,
    //	None,
    //	Searching,
    //	Expired,
    //	Found,
    //	Canceled,
    //};
    public enum XblMatchmakingStatus : UInt32
    {
        Unknown = 0,
        None = 1,
        Searching = 2,
        Expired = 3,
        Found = 4,
        Canceled = 5
    }
}