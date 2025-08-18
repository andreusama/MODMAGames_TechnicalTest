using UnityEngine;
using System;
using System.Threading;
using System.Collections.Generic;
using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Framework;

#if UNITY_GAMECORE
using UnityEngine.GameCore;
using Unity.GameCore;
using UnityEngine.WindowsGames;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GameCoreManager : PersistentSingleton<GameCoreManager>, IPlatformManager<GameCoreConfig>
    {
#if UNITY_GAMECORE
        public struct XUserData
        {
            public XUserHandle XboxUserHandle;
            public XblContextHandle XboxContextHandle;

            public XUserLocalId XUserLocalID;
            public ulong XUID;

            public XStoreContext StoreContext;
            public Dictionary<string, XStoreLicense> StoreLicenses;

            //Save Handles
            public XGameSaveProviderHandle GameSaveProviderHandle;
            public XGameSaveContainerHandle GameSaveContainerHandle;
        }

        public Action OnUserInitializationStarted;
        public Action OnUserInitializationEnded;

        public Action OnResume;
        public Action OnSuspend;
        public Action<GXDKDeviceAssociation> OnDeviceAssociationChanged;
        public Action<GXDKDeviceStateChange> OnDeviceStateChanged;
        public Action<NetworkingConnectivityHint> OnNetworkConnectivityChanged;
        public Action<XPackageDetails> OnPackageInstalled;

        public XUserData XboxUser;
        public ulong PreviousId = 0;

        private GameCoreUserController m_GameCoreUserController;
        private XRegistrationToken m_PackageInstalledCallbackToken;
        private Thread m_DispatchJob;
        private bool m_StopExecution;

        public enum UserState
        {
            NoInitialized,
            Initializing,
            Initialized
        }

        public UserState CurrentUserState = UserState.NoInitialized;
        public bool IsUserInitialized { get { return CurrentUserState == UserState.Initialized; } }

        public GameCoreUserController UserController { get => m_GameCoreUserController; }
#endif
        public async Task Initialize(GameCoreConfig config)
        {
#if UNITY_GAMECORE
            InitializeGameCoreSystem();

            m_GameCoreUserController = new GameCoreUserController();

            SubscribeEvents();

            InitializeInternalServices(config);
            ConfigureGamecoreControllerHandler(config.GamecoreControllerHandler);

            if (config.IsSimpleUserMode)
            {
                await InitializeSimplifiedUserMode();
            }
#endif
            await Task.CompletedTask;
        }

#if UNITY_GAMECORE
        private void OnDestroy()
        {
            WindowsGamesPLM.OnResumingEvent -= RespondOnResume;
            WindowsGamesPLM.OnSuspendingEvent -= RespondOnSuspending;
            GXDKInput.OnDeviceAssociationChanged -= RespondDeviceAssociationChanged;
            GXDKInput.OnDeviceStateChange -= RespondOnDeviceStateChangeEvent;
            Networking.OnConnectivityHintChanged -= RespondOnNetworkingConnectivityChanged;
            SDK.XPackageUnregisterPackageInstalled(m_PackageInstalledCallbackToken);
        }

        private void InitializeGameCoreSystem()
        {
            int hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameRuntimeInitialize();
            if (HR.SUCCEEDED(hResult))
            {
                m_StopExecution = false;
                m_DispatchJob = new Thread(DispatchGXDKTaskQueue) { Name = "GXDK Task Queue Dispatch" };
                m_DispatchJob.Start();

                InitializeXboxLive();
            }
            else
            {
                Debug.Log("Error initialising the gaming runtime, hresult: " + GameCoreOperationResults.GetName(hResult));
            }
        }

        private void SubscribeEvents()
        {
            WindowsGamesPLM.OnResumingEvent += RespondOnResume;
            WindowsGamesPLM.OnSuspendingEvent += RespondOnSuspending;

            GXDKInput.OnDeviceAssociationChanged += RespondDeviceAssociationChanged;
            GXDKInput.OnDeviceStateChange += RespondOnDeviceStateChangeEvent;

            Networking.OnConnectivityHintChanged += RespondOnNetworkingConnectivityChanged;

            SDK.XPackageRegisterPackageInstalled(RespondOnDLCInstalled, out m_PackageInstalledCallbackToken);
        }
        
        private void InitializeInternalServices(GameCoreConfig config)
        {
            PlatformServices gamecoreServices = new PlatformServices();

            gamecoreServices.Storage = PlatformManager.CreateStorageService(config, new GameCoreStorage());
            gamecoreServices.Achievements = new GameCoreAchievementUnlocker();
            gamecoreServices.DownloadableContentFinder = new GameCoreDownloadableContentFinder();

            PlatformManager.Instance.SetPlatformServices(gamecoreServices);
        }

        private Component ConfigureGamecoreControllerHandler(string controllerHandlerClass)
        {
            if (string.IsNullOrEmpty(controllerHandlerClass))
                return null;

            Type controllerHandlerType = Type.GetType(controllerHandlerClass);
            return gameObject.AddComponent(controllerHandlerType);
        }

        private static async Task InitializeUser(XUserAddOptions options)
        {
            if (_instance.CurrentUserState == UserState.Initializing) return;
            _instance.CurrentUserState = UserState.Initializing;
            _instance.OnUserInitializationStarted?.Invoke();

            var xUserHandle = await Instance.UserController.GetUser(options);
            await Instance.UserController.InititalizeUser(xUserHandle);

            _instance.CurrentUserState = UserState.Initialized;
            _instance.OnUserInitializationEnded?.Invoke();
        }

        /// <summary>
        /// Get the user silently and initialize it.
        /// Use it with Single mode user in order to get the user at the begining of the application.
        /// ¡¡Uncheck Advaced User Mode on PlayerSettings if you want to use SUM!!
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeSimplifiedUserMode()
        {
            await InitializeUser(XUserAddOptions.AddDefaultUserSilently);
        }

        /// <summary>
        /// Get the user by UI and initialize it.
        /// Use it with Advaced User Mode in order to get the user at any point of the application.
        /// Check that Advaced User Mode on PlayerSettings toggle is activated if you want to use AUM
        /// </summary>
        /// <returns></returns>
        public static async Task InitializeAdvancedUserMode()
        {
            await InitializeUser(XUserAddOptions.AddDefaultUserAllowingUI);
        }

        /// <summary>
        /// Use to request picker account with UI.
        /// </summary>
        /// <returns></returns>
        public static async Task<XUserHandle> RequestAccountPickerUI()
        {
            var asyncHResult = GameCoreOperationResults.Invalid;
            var userHandle = (XUserHandle)null;

            SDK.XUserAddAsync(XUserAddOptions.None, (int hresult, XUserHandle handle) =>
            {
                asyncHResult = hresult;
                userHandle = handle;
            });

            while (GameCoreOperationResults.IsInvalid(asyncHResult))
            {
                await Task.Yield();
            }

            if (asyncHResult == HR.S_OK && userHandle != null)
            {
                Instance.XboxUser.XboxUserHandle = userHandle;
                await Instance.UserController.InititalizeUser(userHandle);
                return userHandle;
            }
            else
            {
                Debug.Log($"[FORCE_SIGN_IN] Failed to Log in, HResult:{GameCoreOperationResults.GetName(asyncHResult)}");
                return null;
            }
        }

        /// <summary>
        /// Launch the Xbox UI requesting a Controller for a user
        /// </summary>
        /// <returns>Successful operation or not</returns>
        public async Task<bool> RequestAssociateControllerUI()
        {
            var asyncHResult = GameCoreOperationResults.Invalid;
            SDK.XUserFindControllerForUserWithUiAsync(Instance.XboxUser.XboxUserHandle, (hResult, _) =>
            {
                asyncHResult = hResult;
            });

            while (GameCoreOperationResults.IsInvalid(asyncHResult))
            {
                await Task.Yield();
            }

            return (asyncHResult == HR.S_OK) ? true : false;
        }

        /// <summary>
        /// Xbox loop to do GameCore async operations as user events or store operations.
        /// </summary>
        private void DispatchGXDKTaskQueue()
        {
            while (m_StopExecution == false)
            {
                SDK.XTaskQueueDispatch(0);
                Thread.Sleep(32);
            }
        }

        /// <summary>
        /// Try to initialize the Xbox Live services with the SCID
        /// </summary>
        private void InitializeXboxLive()
        {
            int hResult;
            if (string.IsNullOrEmpty(GameCoreSettings.SCID))
            {
                Debug.Log("GameCoreSettings.SCID not defined, XboxLive services won't be initialized.");
            }

            hResult = SDK.XBL.XblInitialize(GameCoreSettings.SCID);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"Xbox Live services error:{GameCoreOperationResults.GetName(hResult)}.");
            }
        }

        #region XBOX CALLBACK'S 

        private void RespondOnSuspending()
        {
            OnSuspend?.Invoke();
            WindowsGamesPLM.AmReadyToSuspendNow();
        }

        private void RespondOnResume(double secondsSuspended)
        {
            OnResume?.Invoke();
            AdditionalContentManager.Instance.Initialize();
        }

        private void RespondDeviceAssociationChanged(GXDKDeviceAssociation newDeviceAssociation)
        {
            OnDeviceAssociationChanged?.Invoke(newDeviceAssociation);
        }

        private void RespondOnDeviceStateChangeEvent(GXDKDeviceStateChange newDeviceState)
        {
            OnDeviceStateChanged?.Invoke(newDeviceState);
        }

        private void RespondOnNetworkingConnectivityChanged(NetworkingConnectivityHint newConnectivityHint)
        {
            OnNetworkConnectivityChanged?.Invoke(newConnectivityHint);
        }

        private void RespondOnDLCInstalled(XPackageDetails details)
        {
            OnPackageInstalled?.Invoke(details);
        }

        #endregion

        /// <summary>
        /// Close and clear all valid store licenses of the current user.
        /// </summary>
        public void CloseValidStoreLicenses()
        {
            if (XboxUser.StoreLicenses == null)
            {
                return;
            }
            foreach (var storeLicense in XboxUser.StoreLicenses)
            {
                if (storeLicense.Value == null)
                {
                    continue;
                }
                SDK.XStoreCloseLicenseHandle(storeLicense.Value);
            }
            XboxUser.StoreLicenses.Clear();
        }
#endif
    }
}

