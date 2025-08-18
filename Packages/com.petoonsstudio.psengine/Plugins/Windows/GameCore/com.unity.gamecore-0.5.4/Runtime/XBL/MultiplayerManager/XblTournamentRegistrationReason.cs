using System;
using Unity.GameCore.Interop;

namespace Unity.GameCore
{
    //enum class XblTournamentRegistrationReason : uint32_t
    //{
    //    Unknown,
    //    RegistrationClosed,
    //    MemberAlreadyRegistered,
    //    TournamentFull,
    //    TeamEliminated,
    //    TournamentCompleted
    //};
    public enum XblTournamentRegistrationReason : UInt32
    {
        Unknown = 0,
        RegistrationClosed = 1,
        MemberAlreadyRegistered = 2,
        TournamentFull = 3,
        TeamEliminated = 4,
        TournamentCompleted = 5,
    }
}