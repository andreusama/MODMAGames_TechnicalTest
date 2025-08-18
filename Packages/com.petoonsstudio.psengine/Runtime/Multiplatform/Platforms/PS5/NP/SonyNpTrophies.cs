using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS5
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.Trophies;
using Unity.PSN.PS5.UDS;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public class SonyNpTrophies
    {
        public SonyNpTrophies()
        {
            TrophySystem.OnUnlockNotification += OnUnlockNotification;
        }

        public void StartTrophySystem()
        {
            Debug.Log("[SNP_PS5]SENDING START REQUEST FOR TROPHIES SYSTEM");
            TrophySystem.StartSystemRequest trpStartSystemRequest = new TrophySystem.StartSystemRequest();

            var trpAsyncOp = new AsyncRequest<TrophySystem.StartSystemRequest>(trpStartSystemRequest).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("[SNP_PS5]Trophies System Started");
                }
                else
                {
                    Debug.Log($"[SNP_PS5]UDS System Initialization failed: {antecedent.Request.Result.ErrorMessage()}");
                }
            });

            TrophySystem.Schedule(trpAsyncOp);
        }

        public void StopTrophySystem()
        {
            TrophySystem.StopSystemRequest request = new TrophySystem.StopSystemRequest();

            var requestOp = new AsyncRequest<TrophySystem.StopSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("System Stopped");
                }
            });

            TrophySystem.Schedule(requestOp);
        }

        public void GetGameDetails()
        {
            TrophySystem.TrophyGameDetails gameDetails = new TrophySystem.TrophyGameDetails();
            TrophySystem.TrophyGameData gameData = new TrophySystem.TrophyGameData();

            TrophySystem.GetGameInfoRequest request = new TrophySystem.GetGameInfoRequest()
            {
                UserId = GamePad.activeGamePad.loggedInUser.userId,
                GameDetails = gameDetails,
                GameData = gameData
            };

            var requestOp = new AsyncRequest<TrophySystem.GetGameInfoRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("GetGameInfoRequest completed");

                    OutputTrophyGameDetails(antecedent.Request.GameDetails);
                    OutputTrophyGameData(antecedent.Request.GameData);
                }
            });

            TrophySystem.Schedule(requestOp);
        }

        private void IncreaseProgressStat()
        {
            int id = (int)SampleTrophies.ProgressStatTwenty;

            if (currentData[id] != null && currentData[id].IsProgress == true)
            {
                long currentProgress = currentData[id].ProgressValue;

                currentProgress += 1;

                UniversalDataSystem.UDSEvent myEvent = new UniversalDataSystem.UDSEvent();

                myEvent.Create("UpdateKillCount");
                myEvent.Properties.Set("newKillCount", (int)currentProgress);

                UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

                request.UserId = GamePad.activeGamePad.loggedInUser.userId;
                request.CalculateEstimatedSize = false;
                request.EventData = myEvent;

                var requestOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
                {
                    if (PS5Manager.CheckAysncRequestOK(antecedent))
                    {
                        Debug.Log("UpdateKillCount Event sent");
                        //GetTrophyInfo(id);
                    }
                    else
                    {
                        Debug.Log("Event send error");
                    }
                });

                UniversalDataSystem.Schedule(requestOp);
            }
        }

        private void IncreaseBasicProgress()
        {
            int id = (int)SampleTrophies.BasicProgress;

            if(currentData[id] != null && currentData[id].IsProgress == true)
            {
                long currentProgress = currentData[id].ProgressValue;

                currentProgress += 10;

                UnlockProgressTrophy(id, currentProgress);
            }
        }

        private void OnUnlockNotification(int trophyId)
        {
            Debug.Log("OnUnlockNotification: Trophy Unlocked " + trophyId);
            //GetTrophyInfo(trophyId);
        }

        public void UnlockNextLockedTrophy()
        {
            if(!UniversalDataSystem.IsInitialized)
            {
                Debug.Log("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            for (int i = 0; i < (int)SampleTrophies.TrophyCount; i++)
            {
                if ( currentData[i].Unlocked == false && currentData[i].IsProgress == false && currentDetails[i].TrophyGrade != 1)
                {
                    UnlockTrophy(currentData[i].TrophyId);
                    return;
                }
            }
        }

        public void UnlockTrophy(int id)
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

        public void UnlockProgressTrophy(int id, long value)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            UniversalDataSystem.UpdateTrophyProgressRequest request = new UniversalDataSystem.UpdateTrophyProgressRequest();

            request.TrophyId = id;
            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.Progress = value;

            var getTrophyOp = new AsyncRequest<UniversalDataSystem.UpdateTrophyProgressRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("Progress Trophy Update Request finished = " + antecedent.Request.TrophyId + " : Progress = " + antecedent.Request.Progress);
                    //GetTrophyInfo(id);
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Progress Trophy Updating");
        }

        public void GetGroupInfo(int groupId)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.GetGroupInfoRequest request = new TrophySystem.GetGroupInfoRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.GroupId = groupId;
            request.GroupDetails = new TrophySystem.TrophyGroupDetails();
            request.GroupData = new TrophySystem.TrophyGroupData();

            var getTrophyOp = new AsyncRequest<TrophySystem.GetGroupInfoRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    OutputTrophyGroupDetails(antecedent.Request.GroupDetails);
                    OutputTrophyGroupData(antecedent.Request.GroupData);
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Getting Group Info");
        }

        enum SampleTrophies
        {
            Platinum = 0,
            BasicGold = 1,
            BasicSilver = 2,
            BasicBronze = 3,
            Hidden = 4,
            ProggreeStatThree = 5,
            ProgressStatTwenty = 6,
            BasicProgress = 7,
            Reward = 8,

            LastIndex = Reward,
            TrophyCount,
        }


        int numTrophiesReturned = 0;
        TrophySystem.TrophyDetails[] currentDetails;
        TrophySystem.TrophyData[] currentData;

        public void GetAllTrophyState()
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            currentDetails = new TrophySystem.TrophyDetails[(int)SampleTrophies.TrophyCount];
            currentData = new TrophySystem.TrophyData[(int)SampleTrophies.TrophyCount];

            numTrophiesReturned = 0;

            for (int i = 0; i < (int)SampleTrophies.TrophyCount; i++)
            {
                //GetTrophyInfo(i);
            }
        }


        public void GetTrophyInfo(int trophyId)
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

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyInfoRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    OutputTrophyDetails(antecedent.Request.TrophyDetails);
                    OutputTrophyData(antecedent.Request.TrophyData);

                    int id = antecedent.Request.TrophyId;

                    if(currentDetails[id] == null)
                    {
                        numTrophiesReturned++;
                    }

                    currentDetails[id] = antecedent.Request.TrophyDetails;
                    currentData[id] = antecedent.Request.TrophyData;
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);
        }

        public List<TrophySystem.Icon> pendingIcons = new List<TrophySystem.Icon>();

        public void GetTrophyGameIcon()
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.GetTrophyGameIconRequest request = new TrophySystem.GetTrophyGameIconRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyGameIconRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    if (request.Icon != null)
                    {
                        pendingIcons.Add(request.Icon);
                    }
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Getting Game Icon");
        }

        public void GetTrophyGroupIcon(int groupId)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.GetTrophyGroupIconRequest request = new TrophySystem.GetTrophyGroupIconRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.GroupId = groupId;

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyGroupIconRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    if (request.Icon != null)
                    {
                        pendingIcons.Add(request.Icon);
                    }
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Getting Group Icon");
        }

        public void GetTrophyIcon(int trophyId)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.GetTrophyIconRequest request = new TrophySystem.GetTrophyIconRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.TrophyId = trophyId;

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyIconRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    if (request.Icon != null)
                    {
                        pendingIcons.Add(request.Icon);
                    }
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Getting Trophy Icon");
        }

        public void GetTrophyRewardIcon(int trophyId)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.GetTrophyRewardIconRequest request = new TrophySystem.GetTrophyRewardIconRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.TrophyId = trophyId;

            var getTrophyOp = new AsyncRequest<TrophySystem.GetTrophyRewardIconRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    if (request.Icon != null)
                    {
                        pendingIcons.Add(request.Icon);
                    }
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Getting Trophy Reward Icon");
        }

        public void ShowTrophyList()
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            TrophySystem.ShowTrophyListRequest request = new TrophySystem.ShowTrophyListRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;

            var getTrophyOp = new AsyncRequest<TrophySystem.ShowTrophyListRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);

            Debug.Log("Show Trophy List");
        }

        private void OutputTrophyGameDetails(TrophySystem.TrophyGameDetails gameDetails)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyGameDetails");

            Debug.Log("   # Groups : " + gameDetails.NumGroups);
            Debug.Log("   # Trophies : " + gameDetails.NumTrophies);
            Debug.Log("   # Platinum : " + gameDetails.NumPlatinum);
            Debug.Log("   # Gold : " + gameDetails.NumGold);
            Debug.Log("   # Silver : " + gameDetails.NumSilver);
            Debug.Log("   # Bronze : " + gameDetails.NumBronze);
            Debug.Log("   Title : " + gameDetails.Title);
        }

        private void OutputTrophyGameData(TrophySystem.TrophyGameData gameData)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyGameData");

            Debug.Log("   # UnlockedTrophies : " + gameData.UnlockedTrophies);
            Debug.Log("   # UnlockedPlatinum : " + gameData.UnlockedPlatinum);
            Debug.Log("   # UnlockedGold : " + gameData.UnlockedGold);
            Debug.Log("   # UnlockedSilver : " + gameData.UnlockedSilver);
            Debug.Log("   # UnlockedBronze : " + gameData.UnlockedBronze);
            Debug.Log("   # ProgressPercentage : " + gameData.ProgressPercentage);
        }

        private void OutputTrophyGroupDetails(TrophySystem.TrophyGroupDetails groupDetails)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyGroupDetails");

            Debug.Log("   GroupId : " + groupDetails.GroupId);
            Debug.Log("   # Trophies : " + groupDetails.NumTrophies);
            Debug.Log("   # Platinum : " + groupDetails.NumPlatinum);
            Debug.Log("   # Gold : " + groupDetails.NumGold);
            Debug.Log("   # Silver : " + groupDetails.NumSilver);
            Debug.Log("   # Bronze : " + groupDetails.NumBronze);
            Debug.Log("   Title : " + groupDetails.Title);
        }

        private void OutputTrophyGroupData(TrophySystem.TrophyGroupData groupData)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyGroupData");

            Debug.Log("   GroupId : " + groupData.GroupId);
            Debug.Log("   # UnlockedTrophies : " + groupData.UnlockedTrophies);
            Debug.Log("   # UnlockedPlatinum : " + groupData.UnlockedPlatinum);
            Debug.Log("   # UnlockedGold : " + groupData.UnlockedGold);
            Debug.Log("   # UnlockedSilver : " + groupData.UnlockedSilver);
            Debug.Log("   # UnlockedBronze : " + groupData.UnlockedBronze);
            Debug.Log("   # ProgressPercentage : " + groupData.ProgressPercentage);
        }

        private void OutputTrophyDetails(TrophySystem.TrophyDetails trophyDetails)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyDetails");

            Debug.Log("   TrophyId : " + trophyDetails.TrophyId);
            Debug.Log("   TrophyGrade : " + trophyDetails.TrophyGrade);
            Debug.Log("   GroupId : " + trophyDetails.GroupId);
            Debug.Log("   Hidden : " + trophyDetails.Hidden);
            Debug.Log("   HasReward : " + trophyDetails.HasReward);
            Debug.Log("   Title : " + trophyDetails.Title);
            Debug.Log("   Description : " + trophyDetails.Description);
            Debug.Log("   Reward : " + trophyDetails.Reward);
            Debug.Log("   IsProgress : " + trophyDetails.IsProgress);

            if (trophyDetails.IsProgress)
            {
                Debug.Log("   TargetValue : " + trophyDetails.TargetValue);
            }
        }

        private void OutputTrophyData(TrophySystem.TrophyData trophyData)
        {
            if (!UniversalDataSystem.IsInitialized)
            {
                Debug.LogError("The Universal Data System needs to be running. Use the UDS menu option to start it.");
            }

            Debug.Log("TrophyData");

            Debug.Log("   TrophyId : " + trophyData.TrophyId);
            Debug.Log("   Unlocked : " + trophyData.Unlocked);
            Debug.Log("   TimeStamp : " + trophyData.TimeStamp);
            Debug.Log("   IsProgress : " + trophyData.IsProgress);

            if (trophyData.IsProgress)
            {
                Debug.Log("   ProgressValue : " + trophyData.ProgressValue);
            }
        }

    }
#endif
}

