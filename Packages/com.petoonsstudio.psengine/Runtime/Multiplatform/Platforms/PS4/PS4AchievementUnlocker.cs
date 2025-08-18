using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PS4
using UnityEngine.PS4;
using Sony.NP;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS4
{
    public class PS4AchievementUnlocker : IAchievementUnlocker
    {
        public async void UnlockAchievement(string achievementID)
        {
#if UNITY_PS4
            try
            {
                if (!int.TryParse(achievementID, out int achievementIDInt))
                {
                    Debug.Log($"Error parsing AchievementID:{achievementID} to Int, throphy won't be unlocked");
                    return;
                }

                if (await IsTrophyUnlocked(achievementIDInt)) return;

                Trophies.UnlockTrophyRequest request = new Trophies.UnlockTrophyRequest();
                request.TrophyId = achievementIDInt;
                request.UserId = Utility.initialUserId;

                Core.EmptyResponse response = new Core.EmptyResponse();

                int requestId = Trophies.UnlockTrophy(request, response);
#if PETOONS_DEBUG
                    Debug.Log($"ACHIEVEMENT SYSTEM: IUnlocking trophy --> Checking trophy {requestId}");
#endif
            }
            catch (NpToolkitException e)
            {
                Debug.LogError("Exception : " + e.ExtendedMessage);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#else
            await Task.CompletedTask;
#endif
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
#if UNITY_PS4
            try
            {
                Trophies.GetUnlockedTrophiesRequest request = new Trophies.GetUnlockedTrophiesRequest();
                request.UserId = UnityEngine.PS4.Utility.initialUserId;

                Trophies.UnlockedTrophiesResponse response = new Trophies.UnlockedTrophiesResponse();

                int requestId = Trophies.GetUnlockedTrophies(request, response);

                await TaskUtils.WaitUntil(() => !response.Locked);
#if PETOONS_DEBUG
                Debug.Log("GetUnlockedTrophies Async : Request Id = " + requestId);
#endif

                foreach (var item in response.TrophyIds)
                {
#if PETOONS_DEBUG
                    Debug.Log($"ACHIEVEMENT SYSTEM: IsTrophyUnlocked --> Checking trophy {item.ToString()}");
#endif
                    unlockeds.Add(item.ToString());
                }

            }
            catch (NpToolkitException e)
            {
                Debug.LogError("Exception : " + e.ExtendedMessage);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
            }
#endif
            return await Task.FromResult(unlockeds);
        }

#if UNITY_PS4
        public async Task<bool> IsTrophyUnlocked(int trophyId)
        {
            try
            {
                Trophies.GetUnlockedTrophiesRequest request = new Trophies.GetUnlockedTrophiesRequest();
                request.UserId = UnityEngine.PS4.Utility.initialUserId;

                Trophies.UnlockedTrophiesResponse response = new Trophies.UnlockedTrophiesResponse();

                int requestId = Trophies.GetUnlockedTrophies(request, response);

                await TaskUtils.WaitUntil(() => !response.Locked);
#if PETOONS_DEBUG
                Debug.Log("GetUnlockedTrophies Async : Request Id = " + requestId);
#endif

                foreach (var item in response.TrophyIds)
                {
#if PETOONS_DEBUG
                    Debug.Log($"ACHIEVEMENT SYSTEM: IsTrophyUnlocked --> Checking trophy {item.ToString()}");
#endif
                    if (item == trophyId)
                        return true;
                }

                return false;
            }
            catch (NpToolkitException e)
            {
                Debug.LogError("Exception : " + e.ExtendedMessage);
                return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
        }
#endif
    }

}
