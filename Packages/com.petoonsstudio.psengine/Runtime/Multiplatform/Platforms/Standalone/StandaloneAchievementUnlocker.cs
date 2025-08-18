using PetoonsStudio.PSEngine.Achievements;
using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class StandaloneAchievementUnlocker : IAchievementUnlocker
    {
        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
            foreach (var achievementID in AchievementSystem.Instance.AchievementTable.Keys)
            {
                unlockeds.Add(achievementID);
            }
            return await Task.FromResult(unlockeds);
        }

        public void UnlockAchievement(string achievementID)
        {
            Debug.Log($"Achievement: {achievementID} unlocked.");
        }
    }
}
