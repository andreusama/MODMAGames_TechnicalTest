using PetoonsStudio.PSEngine.Achievements;
using PetoonsStudio.PSEngine.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if STANDALONE_GOG
using Galaxy.Api;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GOG
{
#if STANDALONE_GOG
    public class GOGAchievementUnlocker : IAchievementUnlocker
    {
        public void UnlockAchievement(string achievementID)
        {
            GalaxyManager.Instance.StatsAndAchievements.SetAchievement(achievementID);
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
            foreach (var achievementID in AchievementSystem.Instance.AchievementTable.Keys)
            {
                Achievement achievement = AchievementSystem.Instance.GetAchievement(achievementID);
                if (GalaxyManager.Instance.StatsAndAchievements.GetAchievement(achievement[PlatformManager.Instance.CurrentPlatform]))
                    unlockeds.Add(achievement[PlatformManager.Instance.CurrentPlatform]);
            }
            return await Task.FromResult(unlockeds);
        }
    }
#endif
}
