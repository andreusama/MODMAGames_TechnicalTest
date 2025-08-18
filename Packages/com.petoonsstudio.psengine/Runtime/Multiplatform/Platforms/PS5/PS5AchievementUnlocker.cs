#if UNITY_PS5
using PetoonsStudio.PSEngine.Achievements;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;
using UnityEngine;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public class PS5AchievementUnlocker : IAchievementUnlocker
    {
        public void UnlockAchievement(string achievementID)
        {
            if(int.TryParse(achievementID, out int achievementIntID))
            {
                UnlockTrophy(achievementIntID);
            }
        }

        private void UnlockTrophy(int id)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            UniversalDataSystem.UnlockTrophyRequest request = new UniversalDataSystem.UnlockTrophyRequest();

            request.TrophyId = id;
            request.UserId = GamePad.activeGamePad.loggedInUser.userId;

            var getTrophyOp = new AsyncRequest<UniversalDataSystem.UnlockTrophyRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("Trophy Unlock Request finished = " + antecedent.Request.TrophyId);
                }
                else
                {
                    Debug.LogError($"Trophy Unlock Request FAILED with a result of: {antecedent.Request.Result.message} || " +
                        $"errorMessage: {antecedent.Request.Result.ErrorMessage()} " +
                        $"for User: {antecedent.Request.UserId} and trophy: {antecedent.Request.TrophyId}");
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Trophy Unlocking");
        }

        public async Task<HashSet<string>> GetUnlockedAchievements()
        {
            HashSet<string> unlockeds = new HashSet<string>();
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }
            TrophySystem.GetTrophyInfoRequest request;
            foreach (var achievementID in AchievementSystem.Instance.AchievementTable.Keys)
            {
                Achievement achievement = AchievementSystem.Instance.GetAchievement(achievementID);
                if (int.TryParse(achievement[Platform.Ps5], out int achievementIntID))
                {
                    request = await GetTrophyInfo(achievementIntID);
                    if (request.TrophyData.Unlocked) unlockeds.Add(request.TrophyId.ToString());
                }
            }

            return unlockeds;
        }

        public async Task<TrophySystem.GetTrophyInfoRequest> GetTrophyInfo(int trophyId)
        {

            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("Getting info for trophy " + trophyId);

            TrophySystem.GetTrophyInfoRequest request = new TrophySystem.GetTrophyInfoRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.TrophyId = trophyId;
            request.TrophyDetails = new TrophySystem.TrophyDetails();
            request.TrophyData = new TrophySystem.TrophyData();
            bool finished = false;

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyInfoRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    request = antecedent.Request;
                }
                finished = true;
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            await TaskUtils.WaitUntil(() => finished);
            return request;
        }

    }
#endif
}
