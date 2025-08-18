#if MICROSOFT_GAME_CORE
using XGamingRuntime;
#endif

using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Multiplatform.WindowsStore
{
    public class WSAchievementUnlocker : IAchievementUnlocker
    {
        private const int HUNDRED_PERCENT_ACHIEVEMENT_PROGRESS = 100;

        public void UnlockAchievement(string achievementID)
        {
#if MICROSOFT_GAME_CORE
            ulong xuid;
            if (!WSManager.Succeeded(SDK.XUserGetId(WSManager.Helpers.UserHandle, out xuid), "Get Xbox user ID"))
            {
                return;
            }
            SDK.XBL.XblAchievementsUpdateAchievementAsync(
                    WSManager.Helpers.XblContextHandle,
                    xuid,
                    achievementID,
                    HUNDRED_PERCENT_ACHIEVEMENT_PROGRESS,
                    UnlockAchievementComplete
                );
#endif
        }

#if MICROSOFT_GAME_CORE
        private void UnlockAchievementComplete(int hresult)
        {
            WSManager.Succeeded(hresult, "Unlock achievement");
        }
#endif
        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
#if MICROSOFT_GAME_CORE
            SDK.XGameGetXboxTitleId(out uint tittleID);

            if (WSManager.Helpers.XblContextHandle == null || WSManager.Helpers.UserHandle == null)
            {
                Debug.Log($"[ACHIEVEMENT] No user initialized, return. ");
                return await Task.FromResult(unlockeds);
            }

            ulong xuid;
            if (!WSManager.Succeeded(SDK.XUserGetId(WSManager.Helpers.UserHandle, out xuid), "Get Xbox user ID"))
            {
                return await Task.FromResult(unlockeds);
            }

            XblAchievementsResultHandle achievementsResultHandle = null;

            int hResult = HR.E_INVALIDARG;
            var finished = false;
            SDK.XBL.XblAchievementsGetAchievementsForTitleIdAsync(
                WSManager.Helpers.XblContextHandle,
                xuid,
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
                    finished = true;
                });

            await TaskUtils.WaitUntil(() => finished); 

            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[ACHIEVEMENTS] Get achievements failed: {hResult}. ");
            }
            else
            {
                Debug.Log($"[ACHIEVEMENTS] Get achievements succeded: {hResult}. ");
                SDK.XBL.XblAchievementsResultGetAchievements(achievementsResultHandle, out var xblAchievementsArray);
                Debug.Log($"[ACHIEVEMENTS] Get achievements succeded geted: {xblAchievementsArray.Length} achievements.");
                for (int i = 0; i < xblAchievementsArray.Length; ++i)
                {
                    unlockeds.Add(xblAchievementsArray[i].Id);
                }
            }
#endif
            return await Task.FromResult(unlockeds);
        }
    }
}
