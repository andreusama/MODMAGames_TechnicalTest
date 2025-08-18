using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Collections.Generic;
using PetoonsStudio.PSEngine.Utils;

#if STANDALONE_EPIC
using Epic.OnlineServices;
using Epic.OnlineServices.Achievements;
using Epic.OnlineServices.Platform;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform
{
#if STANDALONE_EPIC
    public class EpicAchievementUnlocker : IAchievementUnlocker
    {
        private AchievementsInterface eosAchievementInterface;
        private const double PROGRESSION_COMPLETE = 1; 

        public void UnlockAchievement(string achievementID)
        {
            if (EpicManager.Instance != null)
            {
                if (EpicManager.Instance.GetEOSPlatformInterface().GetNetworkStatus() != NetworkStatus.Online)
                {
                    Debug.LogWarning($"[ACHIEVEMENT_EPIC] Network status: Not online, stop unlocking achievement:{achievementID}");
                }
                else
                {
                    UnlockAchievementManually(achievementID, (ref OnUnlockAchievementsCompleteCallbackInfo info) =>
                    {
                        if (info.ResultCode == Result.Success)
                        {
                            Debug.Log($"[ACHIEVEMENT_EPIC] Unlock Achievement {achievementID} Succeed");
                        }
                        else
                        {
                            Debug.Log($"[ACHIEVEMENT_EPIC] Unlock Achievement {achievementID} Failed:{info.ResultCode}");
                        }
                    });
                }
            }
            else
            {
                throw new Exception("[ACHIEVEMENT_EPIC] EpicManager not initialized");
            }
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();

            var eosAchievementInterface = GetEOSAchievementInterface();
            var localUserId = EpicManager.Instance.GetProductUserId();
            var queryPlayerAchievementsOptions = new QueryPlayerAchievementsOptions()
            {
                LocalUserId = localUserId,
                TargetUserId = localUserId
            };

            bool queryFinished = false;
            eosAchievementInterface.QueryPlayerAchievements(ref queryPlayerAchievementsOptions, null, (ref OnQueryPlayerAchievementsCompleteCallbackInfo playerAchiQueryData) =>
            {
                if (playerAchiQueryData.ResultCode == Result.Success)
                {
                    var getPlayerAchievementCountOptions = new GetPlayerAchievementCountOptions()
                    {
                        UserId = localUserId
                    };
                    var playerAchievementCount = eosAchievementInterface.GetPlayerAchievementCount(ref getPlayerAchievementCountOptions);
                    for (uint i = 0; i < playerAchievementCount; i++)
                    {
                        var copyPlayerAchievementByIndexOptions = new CopyPlayerAchievementByIndexOptions()
                        {
                            LocalUserId = localUserId,
                            TargetUserId = localUserId,
                            AchievementIndex = i
                        };
                        var result = eosAchievementInterface.CopyPlayerAchievementByIndex(ref copyPlayerAchievementByIndexOptions, out var playerAchievement);
                        if (result == Result.Success)
                        {
                            if (playerAchievement.Value.Progress == PROGRESSION_COMPLETE)
                            {
                                unlockeds.Add(playerAchievement.Value.AchievementId);
                            }
                        }
                    }
                }
                queryFinished = true;
            });

            await TaskUtils.WaitUntil(() => queryFinished);
            
            return unlockeds;
        }

        public void UnlockAchievementManually(string achievementID, OnUnlockAchievementsCompleteCallback callback)
        {
            Debug.Log("[ACHIEVEMENT_EPIC] Unlock Achievement Manually: " + achievementID);
            var eosAchievementInterface = GetEOSAchievementInterface();
            var localUserId = EpicManager.Instance.GetProductUserId();
            var eosAchievementOption = new UnlockAchievementsOptions
            {
                UserId = localUserId,
                AchievementIds = new Utf8String[] { achievementID }
            };
            eosAchievementInterface.UnlockAchievements(ref eosAchievementOption, null, callback);
        }

        AchievementsInterface GetEOSAchievementInterface()
        {
            if (eosAchievementInterface == null)
            {
                var eosPlatformInterface = EpicManager.Instance.GetEOSPlatformInterface();
                eosAchievementInterface = eosPlatformInterface.GetAchievementsInterface();
            }
            return eosAchievementInterface;
        }
    }
#endif
}
