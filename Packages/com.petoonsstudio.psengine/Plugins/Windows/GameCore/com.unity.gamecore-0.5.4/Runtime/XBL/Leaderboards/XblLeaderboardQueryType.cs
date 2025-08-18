using System;

namespace Unity.GameCore
{
    public enum XblLeaderboardQueryType : UInt32
    {
        /// <summary>
        /// A leaderboard based an event based user stat.
        /// </summary>
        UserStatBacked = 0,

        /// <summary>
        /// A global leaderboard backed by a title managed stat.
        /// </summary>
        TitleManagedStatBackedGlobal = 1,

        /// <summary>
        /// A social leaderboard backed by a title managed stat.
        /// </summary>
        TitleManagedStatBackedSocial = 2,
    }
}
