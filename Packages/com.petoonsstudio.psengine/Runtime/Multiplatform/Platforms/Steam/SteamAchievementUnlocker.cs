using PetoonsStudio.PSEngine.Achievements;
using PetoonsStudio.PSEngine.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

#if STANDALONE_STEAM
using PetoonsStudio.PSEngine.Multiplatform.Steam;
using Steamworks;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class SteamAchievementUnlocker : IAchievementUnlocker
    {
        public void UnlockAchievement(string achievementID)
        {
#if STANDALONE_STEAM
            if (!SteamManager.Initialized)
            {
                throw new Exception("UnlockSteamAchievement: SteamManager not initialized");
            }

            SteamUserStats.GetAchievement(achievementID, out bool achieved);
            if (!achieved)
            {
                SteamUserStats.SetAchievement(achievementID);
                SteamUserStats.StoreStats();
            }
#endif
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
#if STANDALONE_STEAM
            bool achieved;
            foreach (var achievementID in AchievementSystem.Instance.AchievementTable.Keys)
            {
                achieved = false;
                Achievement achievement = AchievementSystem.Instance.GetAchievement(achievementID);
                SteamUserStats.GetAchievement(achievement[PlatformManager.Instance.CurrentPlatform], out achieved);
                if (achieved)
                    unlockeds.Add(achievement[PlatformManager.Instance.CurrentPlatform]);
            }
#endif
            return await Task.FromResult(unlockeds);
        }
    }

}
