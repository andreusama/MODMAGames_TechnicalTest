using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Multiplatform;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Achievements
{
    public abstract class Achievement : ScriptableObject
    {
        [Tooltip("Internal string to identify unique Achievement.")]
        public string ID = "AchievementXX";

        [Tooltip("List of Platforms with his ID, notice that one platform can have only one ID.")]
        public List<PlatformID> PlatformIds = new List<PlatformID>();

        public string this[Platform platform]
        {
            get
            {
                var platformID = PlatformIds.Find(x => x.Platform.HasFlag(platform));
                return (platformID == null) ? string.Empty : platformID.ID;
            }
        }

        public abstract bool IsUnlockable();
    }
}