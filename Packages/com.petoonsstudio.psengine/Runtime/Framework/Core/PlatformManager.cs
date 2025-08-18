using UnityEngine;
using System;
using System.Linq;
using PetoonsStudio.PSEngine.Multiplatform;
using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Multiplatform.WindowsStore;
using PetoonsStudio.PSEngine.Multiplatform.Switch;
using PetoonsStudio.PSEngine.Multiplatform.Standalone;
using PetoonsStudio.PSEngine.Multiplatform.Steam;
using PetoonsStudio.PSEngine.Multiplatform.GOG;
using PetoonsStudio.PSEngine.Multiplatform.PS4;
using PetoonsStudio.PSEngine.Multiplatform.PS5;
using PetoonsStudio.PSEngine.Multiplatform.GameCore;
using PetoonsStudio.PSEngine.Multiplatform.Android;

namespace PetoonsStudio.PSEngine.Framework
{
    public class PlatformServices
    {
        public IStorage Storage;
        public IAchievementUnlocker Achievements;
        public IDownloadableContentFinder DownloadableContentFinder;
    }

    [Flags]
    public enum Platform
    {
        Editor = 0,
        Ps4 = 1,
        Ps5 = 2,
        XboxOne = 4,
        XboxSeries = 8,
        Switch = 16,
        Standalone = 32,
        Steam = 64,
        Epic = 128,
        Gog = 256,
        WindowsStore = 512,
        Android = 1024
    }

    public class PlatformManager : PersistentSingleton<PlatformManager>
    {
        public enum State { 
            NotInitialized, 
            Initializing, 
            Initialized
        };

        public Platform CurrentPlatform { get; private set; }

        public static State InternalState { get; private set; }

        public static bool Initialized { get => InternalState == State.Initialized; }

        [Header("Platform Configurations")]
#if UNITY_STANDALONE || UNITY_EDITOR
        public StandaloneConfig StandaloneConfig;
#endif
#if UNITY_SWITCH || UNITY_EDITOR
        public SwitchConfig SwitchConfig;
#endif
#if UNITY_PS5 || UNITY_EDITOR
        public PS5Config PS5Config;
#endif
#if UNITY_PS4 || UNITY_EDITOR
        public PS4Config PS4Config;
#endif
#if UNITY_GAMECORE_XBOXONE || UNITY_EDITOR
        public XboxOneConfig XboxOneConfig;
#endif
#if UNITY_GAMECORE_XBOXSERIES || UNITY_EDITOR
        public XboxSeriesConfig XboxSeriesConfig;
#endif
#if STANDALONE_STEAM || UNITY_EDITOR
        public SteamConfig SteamConfig;
#endif
#if STANDALONE_EPIC || UNITY_EDITOR
        public EpicConfig EpicConfig;
#endif
#if STANDALONE_GOG || UNITY_EDITOR
        public GOGConfig GOGConfig;
#endif
#if MICROSOFT_GAME_CORE || UNITY_EDITOR
        public WSConfig WindowsStoreConfig;
#endif
#if UNITY_ANDROID || UNITY_EDITOR
        public AndroidConfig AndroidConfig;
#endif

        public PlatformServices m_Services = new PlatformServices();

        public IStorage Storage { get => m_Services.Storage; }
        public IDownloadableContentFinder DownloadableContetnFinder { get => m_Services.DownloadableContentFinder; }
        public IAchievementUnlocker Achievement { get => m_Services.Achievements; }

        protected override void Awake()
        {
            base.Awake();

            if (_instance != this) return;

            try
            {
                if (!Initialized)
                    _ = Initialize();
            }
            catch (Exception e)
            {
                Debug.LogError("[ERROR INIT] " + e);
            }
        }

        public void SetPlatformServices(PlatformServices services)
        {
            m_Services = services;
            AdditionalContentManager.Instance.Initialize();
        }

        public async Task Initialize()
        {
            InternalState = State.Initializing;
#if UNITY_EDITOR
            await InitializeEditorPlatform();
#elif STANDALONE_STEAM
            await InitializeSteamPlatform();
#elif STANDALONE_EPIC
            await InitializeEpicPlatform();
#elif STANDALONE_GOG
            await InitializeGOGPlatform();
#elif MICROSOFT_GAME_CORE
            await InitializeWSPlatform();
#elif UNITY_SWITCH
            await InitializeSwitchPlatform();
#elif UNITY_PS4
            await InitializePS4Platform();
#elif UNITY_PS5
            await InitializePS5Platform();
#elif UNITY_GAMECORE_XBOXONE
            await InitializeXboxOnePlatform();
#elif UNITY_GAMECORE_XBOXSERIES
            await InitializeXboxSeriesPlatform();
#elif UNITY_ANDROID
            await InitializeAndroidPlatform();
#elif UNITY_STANDALONE
            await InitializeStandalonePlatform();
#endif

            InternalState = State.Initialized;
        }

        protected virtual async Task InitializeEditorPlatform()
        {
#if UNITY_EDITOR
            await InitiliazePlatform<StandaloneManager, StandaloneConfig>(Platform.Editor);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeStandalonePlatform()
        {
#if UNITY_STANDALONE
            await InitiliazePlatform<StandaloneManager, StandaloneConfig>(Platform.Standalone);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeSteamPlatform()
        {
#if STANDALONE_STEAM
            await InitiliazePlatform<SteamManager, SteamConfig>(Platform.Steam);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeGOGPlatform()
        {
#if STANDALONE_GOG
            await InitiliazePlatform<GalaxyManager, GOGConfig>(Platform.Gog);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeEpicPlatform()
        {
#if STANDALONE_EPIC
            await InitiliazePlatform<EpicManager, EpicConfig>(Platform.Epic);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeWSPlatform()
        {
#if MICROSOFT_GAME_CORE
            await InitiliazePlatform<WSManager, WSConfig>(Platform.WindowsStore);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeSwitchPlatform()
        {
#if UNITY_SWITCH
            await InitiliazePlatform<SwitchManager, SwitchConfig>(Platform.Switch);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializePS4Platform()
        {
#if UNITY_PS4
            await InitiliazePlatform<PS4Manager, PS4Config>(Platform.Ps4);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializePS5Platform()
        {
#if UNITY_PS5
            await InitiliazePlatform<PS5Manager, PS5Config>(Platform.Ps5);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeXboxOnePlatform()
        {
#if UNITY_GAMECORE_XBOXONE
            await InitiliazePlatform<GameCoreManager, GameCoreConfig>(Platform.XboxOne);
#else
            await Task.CompletedTask;
#endif
        }
        protected virtual async Task InitializeXboxSeriesPlatform()
        {
#if UNITY_GAMECORE_XBOXSERIES
            await InitiliazePlatform<GameCoreManager, GameCoreConfig>(Platform.XboxSeries);
#else
            await Task.CompletedTask;
#endif
        }

        protected virtual async Task InitializeAndroidPlatform()
        {
#if UNITY_ANDROID
            await InitiliazePlatform<AndroidManager, AndroidConfig>(Platform.Android);
#else
            await Task.CompletedTask;
#endif
        }

        private async Task InitiliazePlatform<Manager, Config>(Platform platform) 
            where Manager : MonoBehaviour, IPlatformManager<Config> 
            where Config : PlatformBaseConfiguration
        {
            CurrentPlatform = platform;
            var manager = gameObject.AddComponent<Manager>();
            var config = GetPlatformConfig<Config>(platform);
            
            await manager.Initialize(config);
        }

        private T GetPlatformConfig<T>(Platform platform) where T : ScriptableObject
        {
            switch (platform)
            {
                case Platform.Steam:
#if STANDALONE_STEAM || UNITY_EDITOR
                    return SteamConfig as T;
#endif
                case Platform.Epic:
#if STANDALONE_EPIC || UNITY_EDITOR
                    return EpicConfig as T;
#endif
                case Platform.Gog:
#if STANDALONE_GOG || UNITY_EDITOR
                    return GOGConfig as T;
#endif
                case Platform.WindowsStore:
#if MICROSOFT_GAME_CORE || UNITY_EDITOR
                    return WindowsStoreConfig as T;
#endif
                case Platform.Switch:
#if UNITY_SWITCH || UNITY_EDITOR
                    return SwitchConfig as T;
#endif
                case Platform.Ps4:
#if UNITY_PS4 || UNITY_EDITOR
                    return PS4Config as T;
#endif
                case Platform.Ps5:
#if UNITY_PS5 || UNITY_EDITOR
                    return PS5Config as T;
#endif
                case Platform.XboxOne:
#if UNITY_GAMECORE_XBOXONE || UNITY_EDITOR
                    return XboxOneConfig as T;
#endif
                case Platform.XboxSeries:
#if UNITY_GAMECORE_XBOXSERIES || UNITY_EDITOR
                    return XboxSeriesConfig as T;
#endif
                case Platform.Android:
#if UNITY_ANDROID || UNITY_EDITOR
                    return AndroidConfig as T;
#endif
                case Platform.Standalone:
#if UNITY_STANDALONE || UNITY_EDITOR
                    return StandaloneConfig as T;
#endif
                case Platform.Editor:
#if UNITY_EDITOR
                    return StandaloneConfig as T;
#endif
                default:
                    return null;
            }
        }

        public static IStorage CreateStorageService(PlatformBaseConfiguration configuration, IStorage platformStorage)
        {
            if (configuration.OverrideStorage)
                return CreateService<IStorage>(configuration.StorageService);
            else
                return platformStorage;
        }

        private static T CreateService<T>(string serviceTypeName)
        {
            return (T)Activator.CreateInstance(Type.GetType(serviceTypeName));
        }
    }
}