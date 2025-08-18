using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Achievements
{
    public class AchievementSystem : PersistentSingleton<AchievementSystem>
    {
        public AchievementTable AchievementTable { get => m_AchievementTable; }

        [SerializeField]
        private AchievementTable m_AchievementTable = null;

        public async Task<bool> IsUnlockedOnPlatform(string achievementID)
        {
            if (AchievementTable == null || PlatformManager.Instance.Achievement == null)
            {
                return false;
            }

            var asyncOperation = AchievementTable.LoadAssetAsync(achievementID);
            await asyncOperation.Task;

            Achievement achievement = asyncOperation.Result;
            HashSet<string> unlockeds = await PlatformManager.Instance.Achievement.GetUnlockedAchievements();

            List<string> unlockedAchievementIDs = new List<string>();

            for (int i = 0; i < unlockeds.Count; ++i)
            {
                if (achievement[PlatformManager.Instance.CurrentPlatform] != null)
                {
                    unlockedAchievementIDs.Add(achievement.ID.ToString());
                }
            }

            AchievementTable.ReleaseAsset(achievement);

            return unlockedAchievementIDs.Contains(achievementID);
        }

        public bool IsUnlockable(string achievementID)
        {
            try
            {
                if (AchievementTable == null)
                {
                    return false;
                }

                if (!m_AchievementTable.Keys.Contains(achievementID))
                {
                    return false;
                }
                else
                {
                    Achievement achievement = AchievementTable.LoadAsset(achievementID);

                    bool result = achievement.IsUnlockable();

                    AchievementTable.ReleaseAsset(achievement);

                    return result;
                }

            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
                return false;
            }
        }

        /// <summary>
        /// Check condition unlockable for "achievements", if it is not unlocked add call to unlock
        /// </summary>
        /// <param name="achievements">The achievements that will be checked</param>
        public async void SynchronizePlatformAchievements(string[] achievements)
        {
            if (PlatformManager.Instance.Achievement == null)
                return;

            HashSet<string> unlockeds = await PlatformManager.Instance.Achievement.GetUnlockedAchievements();

            foreach (var achievementID in achievements)
            {
                await SynchronizePlatformAchievement(achievementID, unlockeds);
            }
        }

        /// <summary>
        /// Check condition unlockable for all achievements, if it is not unlocked add call to unlock
        /// </summary>
        public async void SynchronizeAllPlatformAchievements()
        {
            try
            {
                if (AchievementTable == null || PlatformManager.Instance.Achievement == null)
                {
                    return;
                }
                HashSet<string> unlockeds = await PlatformManager.Instance.Achievement.GetUnlockedAchievements();
                foreach (var achievementID in m_AchievementTable.Keys)
                {
                    await SynchronizePlatformAchievement(achievementID, unlockeds);
                }
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }

        /// <summary>
        /// Check condition unlockable for an achievement, if it is not unlocked add call to unlock it.
        /// </summary>
        public async Task SynchronizePlatformAchievement(string achievementID, HashSet<string> unlockeds = null)
        {
            try
            {
                if (AchievementTable == null || PlatformManager.Instance.Achievement == null)
                {
                    return;
                }

                var asyncOperation = AchievementTable.LoadAssetAsync(achievementID);
                await asyncOperation.Task;

                Achievement achievement = asyncOperation.Result;

                if (unlockeds == null)
                    unlockeds = await PlatformManager.Instance.Achievement.GetUnlockedAchievements();

                string platformAchievementKey = achievement[PlatformManager.Instance.CurrentPlatform];

                if (!string.IsNullOrEmpty(platformAchievementKey))
                {
                    if (achievement.IsUnlockable() && !unlockeds.Contains(platformAchievementKey))
                    {
                        PlatformManager.Instance.Achievement.UnlockAchievement(achievement[PlatformManager.Instance.CurrentPlatform]);
                    }
                }

                AchievementTable.ReleaseAsset(achievement);
            }
            catch (Exception e)
            {
                Debug.LogWarning(e);
            }
        }
    }
}
