using UnityEngine;

#if UNITY_PS5
using PetoonsStudio.PSEngine.Utils;
using System;
using Unity.PSN.PS5.GameIntent;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public struct SonyLaunchActivityEvent
    {
        public SonyLaunchActivityEvent(string activityID)
        {
            this.activityID = activityID;
        }

        public string activityID;
    }

    public class SonyGameIntentNotifications
    {
        public SonyGameIntentNotifications()
        {
            GameIntentSystem.OnGameIntentNotification += OnGameIntentNotification;
        }

        private void OnGameIntentNotification(GameIntentSystem.GameIntent gameIntent)
        {
            try
            {
                Debug.Log("Game Intent");
                Debug.Log(System.String.Format("    UserId : 0x{0:X}", gameIntent.UserId));
                Debug.Log("    Intent Type : " + gameIntent.IntentType);

                if (gameIntent is GameIntentSystem.LaunchActivity)
                {
                    GameIntentSystem.LaunchActivity activity = gameIntent as GameIntentSystem.LaunchActivity;
                    Debug.Log("    Activity Id : " + activity.ActivityId);
                    PSEventManager.TriggerEvent(new SonyLaunchActivityEvent(activity.ActivityId));
                }
                else if (gameIntent is GameIntentSystem.JoinSession)
                {
                    GameIntentSystem.JoinSession activity = gameIntent as GameIntentSystem.JoinSession;
                    Debug.Log("    Player Session Id : " + activity.PlayerSessionId);
                    Debug.Log("    Member Type : " + activity.MemberType);
                }
                else if (gameIntent is GameIntentSystem.LaunchMultiplayerActivity)
                {
                    GameIntentSystem.LaunchMultiplayerActivity activity = gameIntent as GameIntentSystem.LaunchMultiplayerActivity;
                    Debug.Log("    Activity Id : " + activity.ActivityId);
                    Debug.Log("    Player Session Id : " + activity.PlayerSessionId);
                }
            }
            catch(Exception e)
            {
                Debug.LogError(e.Message);
            }
        }
    }
#endif
}
