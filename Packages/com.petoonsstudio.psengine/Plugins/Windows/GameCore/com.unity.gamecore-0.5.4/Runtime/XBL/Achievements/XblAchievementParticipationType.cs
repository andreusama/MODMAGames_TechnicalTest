using System;

namespace Unity.GameCore
{
    public enum XblAchievementParticipationType : UInt32
    {
        /// <summary>The participation type is unknown.</summary>
        Unknown,

        /// <summary>An achievement that can be earned as an individual participant.</summary>
        Individual,

        /// <summary>An achievement that can be earned as a group participant.</summary>
        Group
    }
}

