using System;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS5
using Unity.PSN.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.UDS;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public class SonyNpUDS
    {
        public Dictionary<string, bool> VisibleActivities = new Dictionary<string, bool>();

        public enum ActivityEndEvent { completed, abandoned, failed, error };

        FrameCounterEvent reusableCounterEvent = new FrameCounterEvent();

        public void StartUDS()
        {
            Debug.Log("[SNP_UDS]SENDING START REQUEST FOR UDS");
            UniversalDataSystem.StartSystemRequest udsStartSystemRequest = new UniversalDataSystem.StartSystemRequest();

            udsStartSystemRequest.PoolSize = 256 * 1024;

            var udsAsyncOp = new AsyncRequest<UniversalDataSystem.StartSystemRequest>(udsStartSystemRequest).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("[SNP_PS5]UDS System Started");
                }
                else
                {
                    Debug.Log($"[SNP_PS5]UDS System Start failed: {antecedent.Request.Result.ErrorMessage()}");
                }
            });

            UniversalDataSystem.Schedule(udsAsyncOp);
        }

        public void StopUDS()
        {
            Debug.Log("[SNP_UDS]SENDING STOP REQUEST FOR UDS");
            UniversalDataSystem.StopSystemRequest request = new UniversalDataSystem.StopSystemRequest();

            var requestOp = new AsyncRequest<UniversalDataSystem.StopSystemRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("System Stopped");
                }
                else
                {
                    Debug.Log($"[SNP_PS5]UDS System Stop failed: {antecedent.Request.Result.ErrorMessage()}");
                }
            });

            UniversalDataSystem.Schedule(requestOp);
        }

        /// <summary>
        /// Post an activity
        /// </summary>
        /// <param name="eventId"></param>
        /// <param name="props"></param>
        public void PostActivityEvent(string eventId, List<UniversalDataSystem.EventProperty> props = null)
        {
            Debug.Log($"[SNP_UDS]PostActivityEvent called");
            UniversalDataSystem.UDSEvent activityEvent = new UniversalDataSystem.UDSEvent();
            activityEvent.Create(eventId);

            if (props != null)
            {
                foreach (var prop in props)
                {
                    activityEvent.Properties.Set(prop);
                }
            }

            UniversalDataSystem.PostEventRequest activityRequest = new UniversalDataSystem.PostEventRequest();
            activityRequest.UserId = GamePad.activeGamePad.loggedInUser.userId;
            activityRequest.EventData = activityEvent;

            var activityOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(activityRequest).ContinueWith((continueWith) =>
            {
                if (continueWith.Request.Result.apiResult != APIResultTypes.Success)
                {
                    Debug.Log("[PS DEBUG] Error when trying to post " + eventId +
                        " for the activity, error : 0x" + continueWith.Request.Result.sceErrorCode.ToString("X8") +
                        " with message: " + continueWith.Request.Result.message);

                }
            });

            // Schedule the event to be posted
            UniversalDataSystem.Schedule(activityOp);
        }

        /// <summary>
        /// Post a UDS Event. This manually unlocks a trophy using an event built in C#.
        /// </summary>
        /// <param name="trophyID"></param>
        public void PostEventUnlockTrophy(int trophyID)
        {
            UniversalDataSystem.UDSEvent myEvent = new UniversalDataSystem.UDSEvent();

            myEvent.Create("_UnlockTrophy");

            UniversalDataSystem.EventProperty prop = new UniversalDataSystem.EventProperty("_trophy_id", (Int32)trophyID);

            myEvent.Properties.Set(prop);

            UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.EventData = myEvent;

            var getTrophyOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("Trophy unlocked");
                }
            });

            UniversalDataSystem.Schedule(getTrophyOp);
        }

        public void GetMemoryStats()
        {
            UniversalDataSystem.GetMemoryStatsRequest request = new UniversalDataSystem.GetMemoryStatsRequest();

            var requestOp = new AsyncRequest<UniversalDataSystem.GetMemoryStatsRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("Get Memory Stats");

                    Debug.Log("  PoolSize         : " + antecedent.Request.PoolSize);
                    Debug.Log("  MaxInuseSize     : " + antecedent.Request.MaxInuseSize);
                    Debug.Log("  CurrentInuseSize : " + antecedent.Request.CurrentInuseSize);
                }
            });

            UniversalDataSystem.Schedule(requestOp);
        }

        public void PostEvent(UniversalDataSystem.UDSEvent udsEvent)
        {
            if (!UniversalDataSystem.IsInitialized)
                Debug.Log("Trying to use UDS without initialization");

            UniversalDataSystem.PostEventRequest request = new UniversalDataSystem.PostEventRequest();

            request.UserId = GamePad.activeGamePad.loggedInUser.userId;
            request.CalculateEstimatedSize = true;
            request.EventData = udsEvent;

            var requestOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
            {
                if (PS5Manager.CheckAysncRequestOK(antecedent))
                {
                    Debug.Log("Event sent - Estimated size = " + antecedent.Request.EstimatedSize);
                }
                else
                {
                    Debug.LogError("Event send error");
                }
            });

            UniversalDataSystem.Schedule(requestOp);

            UniversalDataSystem.EventDebugStringRequest stringRequest = new UniversalDataSystem.EventDebugStringRequest();

            stringRequest.UserId = GamePad.activeGamePad.loggedInUser.userId;
            stringRequest.EventData = udsEvent;

            var secondRequestOp = new AsyncRequest<UniversalDataSystem.EventDebugStringRequest>(stringRequest).ContinueWith((antecedent) =>
            {

                if (request.Result.apiResult == APIResultTypes.Success)
                {
                    Debug.Log("Event sent");
                }
                else
                {
                    Debug.LogError("Event send error");
                }
            });

            UniversalDataSystem.Schedule(secondRequestOp);
        }
    }

    public class FrameCounterEvent
    {
        UniversalDataSystem.UDSEvent udsEvent;
        UniversalDataSystem.EventProperty usernameProp;
        UniversalDataSystem.EventProperty framenumProp;
        UniversalDataSystem.PostEventRequest request;

        AsyncAction<AsyncRequest<UniversalDataSystem.PostEventRequest>> requestOp;

        public bool InUse { get; internal set; }

        public FrameCounterEvent()
        {
            InUse = false;

            udsEvent = new UniversalDataSystem.UDSEvent();
            udsEvent.Create("FrameCounter");

            usernameProp = udsEvent.Properties.Set("username", UniversalDataSystem.PropertyType.String);
            framenumProp = udsEvent.Properties.Set("framenum", UniversalDataSystem.PropertyType.UInt32);

            request = new UniversalDataSystem.PostEventRequest();
            request.UserId = 0;
            request.EventData = udsEvent;

            requestOp = new AsyncRequest<UniversalDataSystem.PostEventRequest>(request).ContinueWith((antecedent) =>
            {
                try
                {
                    if (PS5Manager.CheckAysncRequestOK(antecedent))
                    {
                        Debug.Log("Frame Event sent");
                    }
                    else
                    {
                        Debug.LogError("Event send error");
                        InUse = false;
                    }
                }
                catch (Exception e)
                {
                    InUse = false;
                    throw e;  // rethrow the exception
                }

                InUse = false;
            });
        }

        public void UpdateValues(string username, UInt32 framenum)
        {
            usernameProp.Update(username);
            framenumProp.Update(framenum);
        }

        public void Post(int userId)
        {
            if (InUse == true)
            {
                return;
            }

            requestOp.Reset(); // Reset the async op 

            request.UserId = userId;

            UniversalDataSystem.Schedule(requestOp);
        }
    }

#endif
}

