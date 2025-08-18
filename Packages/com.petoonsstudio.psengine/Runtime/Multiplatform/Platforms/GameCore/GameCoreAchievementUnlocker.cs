using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using PetoonsStudio.PSEngine.Utils;

#if UNITY_GAMECORE
using Unity.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
#if UNITY_GAMECORE
    public class GameCoreAchievementUnlocker : IAchievementUnlocker
    {
        private const int ACHIEVEMENT_FULL_PROGRESS = 100;
        public void UnlockAchievement(string achievementID)
        {
            SDK.XBL.XblAchievementsUpdateAchievementAsync(
                GameCoreManager.Instance.XboxUser.XboxContextHandle,
                GameCoreManager.Instance.XboxUser.XUID,
                achievementID,
                ACHIEVEMENT_FULL_PROGRESS, AchievementCompleted
               );
        }

        private void AchievementCompleted(int hresult)
        {
            if (HR.FAILED(hresult))
            {
                Debug.Log($"[ACHIEVEMENT] Failed to update:{GameCoreOperationResults.GetName(hresult)}. ");
            }
            else
            {
                Debug.Log($"[ACHIEVEMENT] Update result is:{HR.SUCCEEDED(hresult)}. ");
            }
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
            SDK.XGameGetXboxTitleId(out uint tittleID);

            if (GameCoreManager.Instance.XboxUser.XboxContextHandle == null || GameCoreManager.Instance.XboxUser.XboxUserHandle == null)
            {
                Debug.Log($"[ACHIEVEMENT] No user initialized, return. ");
                return await Task.FromResult(unlockeds);
            }
            
            XblAchievementsResultHandle achievementsResultHandle = null;
            int hResult = GameCoreOperationResults.Invalid;
            SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(
                GameCoreManager.Instance.XboxUser.XboxContextHandle,
                GameCoreManager.Instance.XboxUser.XUID,
                tittleID,
                XblAchievementType.All,
                true,
                XblAchievementOrderBy.TitleId,
                0,
                0,
                (result, resultHandle) =>
                {
                    achievementsResultHandle = resultHandle;
                    hResult = result;

                });

            await TaskUtils.WaitUntil(() => !GameCoreOperationResults.IsInvalid(hResult));

            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[ACHIEVEMENTS] Get achievements failed:{GameCoreOperationResults.GetName(hResult)}. ");
            }
            else
            {
                Debug.Log($"[ACHIEVEMENTS] Get achievements succeded:{GameCoreOperationResults.GetName(hResult)}. ");
                SDK.XBL.XblAchievementsResultGetAchievements(achievementsResultHandle, out var xblAchievementsArray);
                Debug.Log($"[ACHIEVEMENTS] Get achievements succeded geted: {xblAchievementsArray.Length} achievements.");
                for (int i = 0; i < xblAchievementsArray.Length; ++i)
                {
                    unlockeds.Add(xblAchievementsArray[i].Id);
                }
            }
            return unlockeds;
        }
    }
#endif
}
