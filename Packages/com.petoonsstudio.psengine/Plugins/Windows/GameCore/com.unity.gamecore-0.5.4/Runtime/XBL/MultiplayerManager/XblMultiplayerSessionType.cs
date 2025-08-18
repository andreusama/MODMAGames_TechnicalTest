using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblMultiplayerSessionType : uint32_t
    //{
    //    Unknown,
    //    LobbySession,
    //    GameSession,
    //    MatchSession
    //};
    public enum XblMultiplayerSessionType : UInt32
    {
        Unknown = 0,
        LobbySession = 1,
        GameSession = 2,
        MatchSession = 3,
    }
}