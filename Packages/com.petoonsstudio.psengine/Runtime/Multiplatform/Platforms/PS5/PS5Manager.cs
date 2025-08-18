using PetoonsStudio.PSEngine.Utils;
using System;
using PetoonsStudio.PSEngine.Framework;
using System.Threading.Tasks;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_PS5
using Unity.PSN.PS5;
using Unity.PSN.PS5.Aysnc;
using Unity.PSN.PS5.WebApi;
using Unity.SaveData.PS5.Initialization;
using Unity.SaveData.PS5.Core;
using UnityEngine.PS5;
using Unity.SaveData.PS5.Dialog;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
    public class PS5Manager : PersistentSingleton<PS5Manager>, IPlatformManager<PS5Config>
    {
#if UNITY_PS5
        public enum Region
        {
            SIEA,
            SIEE
        }

        // By default Product Region is SIEA (America)
        public Region ProductRegion = Region.SIEA;

        public SonyNpTrophies Trophies { get => m_Trophies; }
        SonyNpTrophies m_Trophies;

        public SonyNpUDS UDS { get => m_UDS; }
        SonyNpUDS m_UDS;

        public SonyGameIntentNotifications SonyGameIntent { get => m_GameIntent; }
        SonyGameIntentNotifications m_GameIntent;

        private GamePad m_GamePad0 = null;
        public GamePad Gamepad0 { get => m_GamePad0; }

        private void Update()
        {
            User.CheckRegistration();

            try
            {
                Unity.PSN.PS5.Main.Update();
            }
            catch (Exception e)
            {
                Debug.LogError("Main.Update Exception : " + e.Message);
                Debug.LogError(e.StackTrace);
            }
        }
#endif

        public async Task Initialize(PS5Config config)
        {
#if UNITY_PS5
            try
            {
                //Handles gamepad state
                InitializeGamePad();
                //Handles others features and additionals controllers as Thropies more in deepth funcionality or UDS events
                InitializeNPMain();
                //Initialize Save system
                InitializeSavePlugin();
                //Create patform generic services
                InitializeInternalServices(config);
                //Creates Activity Card handler
                CreateActivityHandler(config);
            }
            catch (Exception e)
            {
                Log(e.Message);
            }
#endif
            await Task.CompletedTask;
        }

#if UNITY_PS5
        private void InitializeNPMain()
        {
            m_Trophies = new SonyNpTrophies();
            m_UDS = new SonyNpUDS();

            m_GameIntent = new SonyGameIntentNotifications();

            try
            {
                var initResult = Unity.PSN.PS5.Main.Initialize();

                if (initResult.Initialized == true)
                {
                    Debug.Log("PSN Initialized ");
                    Debug.Log("Plugin SDK Version : " + initResult.SceSDKVersion.ToString());
#if UNITY_PS5
                    m_UDS.StartUDS();
                    m_Trophies.StartTrophySystem();
#endif
                }
                else
                {
                    Debug.Log("PSN not initialized ");
                }
            }
            catch (PSNException e)
            {
                Debug.LogError("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                Debug.LogError("Missing DLL Expection : " + e.Message);
                Debug.LogError("The sample APP will not run in the editor.");
            }
#endif

            string[] args = System.Environment.GetCommandLineArgs();

            if (args.Length > 0)
            {
                Debug.Log("Args:");

                for (int i = 0; i < args.Length; i++)
                {
                    Debug.Log("  " + args[i]);
                }
            }
            else
            {
                Debug.Log("No Args");
            }

            GamePad[] gamePads = GetComponents<GamePad>();

            User.Initialize(gamePads);
        }

        public PS5Input.LoggedInUser GetUser()
        {
            PS5Input.LoggedInUser loggedInUser = PS5Input.RefreshUsersDetails(0);

            return loggedInUser;
        }
        
        private void CreateActivityHandler(PS5Config config)
        {
            if (string.IsNullOrEmpty(config.PS5ActivityHandler))
            {
                Debug.LogWarning("No activity handler setted up in config.");
                return;
            }

            Type CustomActivityHandler = Type.GetType(config.PS5ActivityHandler);
            gameObject.AddComponent(CustomActivityHandler);
        }
        
        private bool InitializeSavePlugin()
        {
            Unity.SaveData.PS5.Main.OnAsyncEvent += OnAsyncEvent;

            try
            {
                InitSettings settings = new InitSettings();

                settings.Affinity = ThreadAffinity.Core5;

                var initResult = Unity.SaveData.PS5.Main.Initialize(settings);

                if (initResult.Initialized == true)
                {
                    Log("Savedata Initialized ");
                }
                else
                {
                    Log("Savedata not initialized ");
                }

                Log("INITILIAZATED!");

                return initResult.Initialized;
            }
            catch (SaveDataException e)
            {
                Log("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (System.DllNotFoundException e)
            {
                Log("Missing DLL Expection : " + e.Message);
                Log("The sample APP will not run in the editor.");
            }
#endif
            Log("NOT INITIALIZATED!");

            return false;
        }

        private void InitializeGamePad()
        {
            m_GamePad0 = gameObject.AddComponent<GamePad>();
            GamePad.activeGamePad = m_GamePad0;
            m_GamePad0.playerId = 0;
        }

        private void InitializeInternalServices(PS5Config config)
        {
            PlatformServices ps5Services = new PlatformServices();

            ps5Services.Storage = PlatformManager.CreateStorageService(config, new PS5Storage());
            ps5Services.Achievements = new PS5AchievementUnlocker();
            ps5Services.DownloadableContentFinder = new PS5DownloadableContentFinder();

            PlatformManager.Instance.SetPlatformServices(ps5Services);
        }

        /// <summary>
        /// Get Product region from an User Defined Param on params
        /// </summary>
        private void SetProductRegion()
        {
            ProductRegion = (Region)Utility.GetApplicationParameter(1);
        }

        #region Helpers
        static Dictionary<ResponseBase, SaveDataCallbackEvent> callbackEvents = new Dictionary<ResponseBase, SaveDataCallbackEvent>();
        static object syncObj = new object();
        static private void OnAsyncEvent(SaveDataCallbackEvent callbackEvent)
        {
            lock (syncObj)
            {
                callbackEvents.Add(callbackEvent.Response, callbackEvent);
            }
        }

        public static void Log(string message)
        {
#if PETOONS_DEBUG
            string header = "[PS5]";
            string msg = $"{header}: {message}";

            var dialogRequest = new Dialogs.OpenDialogRequest();
            var dialogResponse = new Dialogs.OpenDialogResponse();
            dialogRequest.UserMessage = new Dialogs.UserMessageParam();
            dialogRequest.UserMessage.Message = msg;
            dialogRequest.UserMessage.MsgType = Dialogs.UserMessageType.Normal;

            Dialogs.OpenDialog(dialogRequest,dialogResponse);

            Debug.Log(msg);
#endif
        }
        #endregion

        public static bool CheckRequestOK<R>(R request) where R : Request
        {
            if (request == null)
            {
                UnityEngine.Debug.LogError("Request is null");
                return false;
            }

            if (request.Result.apiResult == Unity.PSN.PS5.APIResultTypes.Success)
            {
                return true;
            }

            OutputApiResult(request.Result);

            return false;
        }

        public static bool CheckAysncRequestOK<R>(AsyncRequest<R> asyncRequest) where R : Request
        {
            if (asyncRequest == null)
            {
                UnityEngine.Debug.LogError("AsyncRequest is null");
                return false;
            }

            return CheckRequestOK<R>(asyncRequest.Request);
        }

        public static void OutputApiResult(APIResult result)
        {
            if (result.apiResult == Unity.PSN.PS5.APIResultTypes.Success)
            {
                return;
            }

            string output = result.ErrorMessage();

            if (result.apiResult == Unity.PSN.PS5.APIResultTypes.Error)
            {
                Debug.LogError(output);
            }
            else
            {
                Debug.LogWarning(output);
            }
        }

        private string GetDebugFilterOutput()
        {
            string output = "";

            output += String.Format("{0,-30}\n", "WebApiFilters");
            output += String.Format("{0,-10} {1,-10} {2,-20}\n", "Id", "Ref Count", "First Filter");

            List<WebApiFilters> filters = new List<WebApiFilters>();
            WebApiNotifications.GetActiveFilters(filters);

            for (int i = 0; i < filters.Count; i++)
            {
                string firstFilterText = filters[i].Filters != null && filters[i].Filters.Count > 0 ? filters[i].Filters[0].DataType : "";

                output += String.Format("{0,-10} {1,-10} {2,-20}\n", filters[i].PushFilterId, filters[i].RefCount, firstFilterText);
            }

            return output;
        }

        private string GetDebugPushEventsOutput()
        {
            string output = "";

            output += String.Format("{0,-30}\n", "WebApiPushEvent");
            output += String.Format("{0,-10} {1,-16} {2,-10} {3,-8}\n", "Id", "User Id", "Filter Id", "Ordered");

            List<WebApiPushEvent> pushEvents = new List<WebApiPushEvent>();
            WebApiNotifications.GetActivePushEvents(pushEvents);

            for (int i = 0; i < pushEvents.Count; i++)
            {
                int filterId = pushEvents[i].Filters != null ? pushEvents[i].Filters.PushFilterId : -1;

                output += String.Format("{0,-10} {1,-10} {2,-10} {3,-8}\n", pushEvents[i].PushCallbackId, "0x" + pushEvents[i].UserId.ToString("X8"), filterId, pushEvents[i].OrderGuaranteed);
            }

            return output;
        }

        private void DisplayFilterList()
        {
            string output = GetDebugFilterOutput();
        }

        private void DisplayPushEventsList()
        {
            string output = GetDebugPushEventsOutput();
        }

        private void DisplayPendingRequestsList()
        {
            if (GamePad.activeGamePad == null)
            {
                return;
            }
        }
#endif
    }
}
