using System.Collections.Generic;
using UnityEngine;
using PetoonsStudio.PSEngine.Utils;

#if UNITY_PS5
using Unity.PSN.PS5.UDS;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public enum ActivityOutcome
    {
        completed, abandoned, failed, error
    }
#endif

    public abstract class PS5ActivityHandler : PersistentSingleton<PS5ActivityHandler>
#if UNITY_PS5
        , PSEventListener<SonyLaunchActivityEvent>
#endif
    {
#if UNITY_PS5
        protected const string ACTIVITY_ID = "activityId";
        protected const string ACTIVITY_START = "activityStart";
        protected const string ACTIVITY_AVAIABILITY = "activityAvailabilityChange";
        protected const string ACTIVITY_RESUME = "activityResume";
        protected const string ACTIVITY_TERMINATE = "activityTerminate";
        protected const string ACTIVITY_END = "activityEnd";
        protected const string ACTIVITY_OUTCOME = "outcome";

        protected const string AVAILABLE_ACTIVITIES = "availableActivities";
        protected const string UNAVAILABLE_ACTIVITIES = "unavailableActivities";
        protected const string IN_PROGRESS_ACTIVITIES = "inProgressActivities";
        protected const string COMPLETED_ACTIVITIES = "completedActivities";

        protected SonyNpUDS UDS;

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            UDS = PS5Manager.Instance.UDS;
        }
        protected virtual void OnEnable()
        {
            if (_instance != this) return;
            Application.quitting += HandleExitGame;
            PSEventManager.AddListener<SonyLaunchActivityEvent>(this);
        }
        protected virtual void OnDisable()
        {
            if (_instance != this) return;
            Application.quitting -= HandleExitGame;
            PSEventManager.RemoveListener<SonyLaunchActivityEvent>(this);
        }

        #region CallBacks
        public virtual void OnPSEvent(SonyLaunchActivityEvent eventType)
        {
            Log($"LAUNCH ACTIVITY CALLED with id: {eventType.activityID}");
            OnLaunchActivity(eventType.activityID);
        }
        protected virtual void HandleExitGame()
        {
            Log($"HandleExitGame called");
            OnExitGame();
        }
        protected abstract void OnLaunchActivity(string activityId);
        protected abstract void OnExitGame();
        #endregion

        #region Helper Methods
        protected void ShowActivity(string activityId, bool fullMode = true, string[] activitySubTasksId = null)
        {
            Log($"Showing Activity with ID: {activityId}");

            List<string> availableActivitiesList = new List<string>();
            //Activity
            availableActivitiesList.Add(activityId);

            if (activitySubTasksId != null)
            {
                //Tasks
                foreach (var item in activitySubTasksId)
                {
                    availableActivitiesList.Add(item);
                }
            }
            List<string> unavailableActivitiesList = new List<string>();

            UpdateActivityAvailabityVisibility(availableActivitiesList, unavailableActivitiesList, fullMode);
        }

        protected UniversalDataSystem.UDSEvent NewActivityEvent(string activityId)
        {
            var _event = new UniversalDataSystem.UDSEvent();
            _event.Create(activityId);
            return _event;
        }
        protected void HideActivity(string activityId, bool fullMode = true, string[] activitySubTasksId = null)
        {
            Log($"Hiding Activity with ID: {activityId}");

            List<string> availableActivitiesList = new List<string>();
            List<string> unavailableActivitiesList = new List<string>();
            //Activity
            unavailableActivitiesList.Add(activityId);
            //SubTasks
            if (activitySubTasksId != null)
                unavailableActivitiesList.AddRange(activitySubTasksId);

            UpdateActivityAvailabityVisibility(availableActivitiesList, unavailableActivitiesList, fullMode);
        }
        protected void StartActivity(string activityId)
        {
            Log($"Starting Activity with ID: {activityId}");

            var props = new List<UniversalDataSystem.EventProperty>();
            props.Add(new UniversalDataSystem.EventProperty(ACTIVITY_ID, activityId));

            UDS.PostActivityEvent(ACTIVITY_START, props);
        }
        protected void EndActivity(string activityId, ActivityOutcome outcome)
        {
            Log($"Ending Activity with ID: {activityId}");

            var props = new List<UniversalDataSystem.EventProperty>();
            props.Add(new UniversalDataSystem.EventProperty(ACTIVITY_ID, activityId));
            props.Add(new UniversalDataSystem.EventProperty(ACTIVITY_OUTCOME, outcome.ToString()));

            UDS.PostActivityEvent(ACTIVITY_END, props);
        }
        protected void ResumeActivity(string activityId, string[] inProgressSubActivities, string[] completedSubActivities)
        {
            var props = new List<UniversalDataSystem.EventProperty>();
            props.Add(new UniversalDataSystem.EventProperty(ACTIVITY_ID, activityId));

            var inProgressSubActivitiesUDS = new UniversalDataSystem.EventPropertyArray(UniversalDataSystem.PropertyType.String);
            var completedSubActivitiesUDS = new UniversalDataSystem.EventPropertyArray(UniversalDataSystem.PropertyType.String);

            inProgressSubActivitiesUDS.CopyValues(inProgressSubActivities);
            completedSubActivitiesUDS.CopyValues(completedSubActivities);

            props.Add(new UniversalDataSystem.EventProperty(IN_PROGRESS_ACTIVITIES, inProgressSubActivitiesUDS));
            props.Add(new UniversalDataSystem.EventProperty(COMPLETED_ACTIVITIES, completedSubActivitiesUDS));

            PS5Manager.Instance.UDS.PostActivityEvent(ACTIVITY_RESUME, props);
        }
        protected void EndAllActivities()
        {
            Debug.Log($"[UDS]ActivityTerminate called");

            UDS.PostActivityEvent(ACTIVITY_TERMINATE, null);
        }
        #endregion

        public void UpdateActivityAvailabityVisibility(List<string> availableActivitiesList, List<string> unavailableActivitiesList, bool fullMode = true)
        {
            Debug.Log($"[SNP_UDS]UpdateActivityVisibility called");
            UniversalDataSystem.UDSEvent myEvent = new UniversalDataSystem.UDSEvent();

            myEvent.Create(ACTIVITY_AVAIABILITY);

            UniversalDataSystem.EventPropertyArray availableActivities =
                new UniversalDataSystem.EventPropertyArray(UniversalDataSystem.PropertyType.String);
            UniversalDataSystem.EventPropertyArray unavailableActivities =
                new UniversalDataSystem.EventPropertyArray(UniversalDataSystem.PropertyType.String);

            myEvent.Properties.Set(AVAILABLE_ACTIVITIES, availableActivities);
            myEvent.Properties.Set(UNAVAILABLE_ACTIVITIES, unavailableActivities);
            if (!fullMode)//Instead of replacing all the values, the system calculates the difference and adds/removes the value to/from the state.
            {
                myEvent.Properties.Set("mode", "delta");
            }
            else//Replaces all the values in the state.
            {
                myEvent.Properties.Set("mode", "full");
            }

            availableActivities.CopyValues<string>(availableActivitiesList.ToArray());
            unavailableActivities.CopyValues<string>(unavailableActivitiesList.ToArray());


            UDS.PostEvent(myEvent);
        }

        protected virtual void Log(string msg)
        {
            Debug.Log($"[AH_PS5]{msg}");
        }
#endif
    }
}
