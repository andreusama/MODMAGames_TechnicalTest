using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_PS4
using Sony.NP;
using Sony.PS4.SaveData;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS4
{
    [AddComponentMenu("Petoons Studio/PSEngine/Multiplatform/PS4/PS4 Manager")]
    public class PS4Manager : PersistentSingleton<PS4Manager>, IPlatformManager<PS4Config>
    {
#if UNITY_PS4
        private PS4NpToolkit m_PS4NpToolkit = new();
        public int InitialUserId = -1;


        public enum Region
        {
            SIEA,
            SIEE
        }

        // By default Product Region is SIEA (America)
        public Region ProductRegion = Region.SIEA;

        private void OnDestroy()
        {
            TerminateLibrary();
        }
#endif


        public async Task Initialize(PS4Config config)
        {
#if UNITY_PS4
            InitializeExternalServices();

            InitializeSaveData();
            SetProductRegion();
            RegisterTrophyPack();

            InitializeInternalServices(config);
#endif
            await Task.CompletedTask;
        }

#if UNITY_PS4
        private void InitializeExternalServices()
        {
            m_PS4NpToolkit.InitialiseNpToolkit();
            InitialUserId = UnityEngine.PS4.Utility.initialUserId;
        }

        private void InitializeInternalServices(PS4Config config)
        {
            PlatformServices ps4Services = new PlatformServices();

            ps4Services.Storage = PlatformManager.CreateStorageService(config, new PS4Storage());
            ps4Services.Achievements = new PS4AchievementUnlocker();
            ps4Services.DownloadableContentFinder = new PS4DownloadableContentFinder();

            PlatformManager.Instance.SetPlatformServices(ps4Services);
        }

        private void InitializeSaveData()
        {
            try
            {

                Sony.PS4.SaveData.Main.OnAsyncEvent += Main_OnAsyncEvent;

                InitSettings settings = new InitSettings();

                settings.Affinity = ThreadAffinity.Core5;

                Sony.PS4.SaveData.InitResult initResult = Sony.PS4.SaveData.Main.Initialize(settings);

                if (initResult.Initialized == true)
                {
                    Debug.Log("SaveData Initialized ");
                }
                else
                {
                    Debug.Log("SaveData not initialized ");
                }
            }
            catch (SaveDataException e)
            {
                Debug.Log("Exception During Initialization : " + e.ExtendedMessage);
            }
#if UNITY_EDITOR
            catch (DllNotFoundException e)
            {
                Debug.Log("Missing DLL Expection : " + e.Message);
                Debug.Log("The sample APP will not run in the editor.");
            }
#endif
        }

        private void Main_OnAsyncEvent(Sony.PS4.SaveData.SaveDataCallbackEvent callbackEvent)
        {
            //Necessary so no NullReferenceException is throwed
        }

        private void TerminateLibrary()
        {
            try
            {
                Sony.PS4.SaveData.Main.Terminate();
            }
            catch (SaveDataException e)
            {
                Debug.Log("Exception During Termination : " + e.ExtendedMessage);
            }
        }

        /// <summary>
        /// Get Product region from an User Defined Param on params
        /// </summary>
        private void SetProductRegion()
        {
            ProductRegion = (Region)UnityEngine.PS4.Utility.GetApplicationParameter(1);
        }

        private void RegisterTrophyPack()
        {
            try
            {
                Trophies.RegisterTrophyPackRequest request = new Trophies.RegisterTrophyPackRequest();
                request.UserId = UnityEngine.PS4.Utility.initialUserId;

                Core.EmptyResponse response = new Core.EmptyResponse();

                // Make the async call which will return the Request Id 
                int requestId = Trophies.RegisterTrophyPack(request, response);
#if PETOONS_DEBUG
                Debug.Log("RegisterTrophyPack : Request Id = " + requestId);
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
        }
#endif
    }
}