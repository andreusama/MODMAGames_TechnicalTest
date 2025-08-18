//Modified by Petoons Studio

// Copyright (c) Microsoft Corporation.
// Licensed under the MIT license.

#if MICROSOFT_GAME_CORE
using XGamingRuntime;
#endif

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Threading.Tasks;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Multiplatform.WindowsStore
{
    public class ErrorEventArgs : System.EventArgs
    {
        public string ErrorCode { get; private set; }
        public string ErrorMessage { get; private set; }

        public ErrorEventArgs(string errorCode, string errorMessage)
        {
            this.ErrorCode = errorCode;
            this.ErrorMessage = errorMessage;
        }
    }
    public class GameSaveLoadedArgs : System.EventArgs
    {
        public byte[] Data { get; private set; }

        public GameSaveLoadedArgs(byte[] data)
        {
            this.Data = data;
        }
    }

#if MICROSOFT_GAME_CORE
    public delegate void ShowPurchaseUICallback(Int32 hresult, XStoreProduct storeProduct);
    public delegate void GetAssociatedProductsCallback(Int32 hresult, List<XStoreProduct> associatedProducts);
#endif

    public class WSManager : MonoBehaviour, IPlatformManager<WSConfig>
    {

        private static WSManager m_XboxHelpers;
        private enum InitializationState { Uninitialized, Initializing, Intialized };
        private static InitializationState m_InitializationState = InitializationState.Uninitialized;
        private static Dictionary<int, string> m_ResultToFriendlyErrorLookup;

#if MICROSOFT_GAME_CORE
        public XGameSaveWrapper GameSaveHelper;
        public XUserHandle UserHandle;
        public XblContextHandle XblContextHandle;
        private XStoreContext m_StoreContext = null;

        private List<XStoreProduct> m_AssociatedProducts;
        private GetAssociatedProductsCallback m_QueryAssociatedProductsCallback;

        private const XStoreProductKind m_AddOnProducts = XStoreProductKind.Durable;
#endif
        private WSConfig m_Config;

        private const string GAME_SAVE_CONTAINER_NAME = "x_game_save_default_container";
        private const string GAME_SAVE_BLOB_NAME = "x_game_save_default_blob";
        private const int MAX_ASSOCIATED_PRODUCTS_TO_RETRIEVE = 25;

        /// <summary>
        /// Static function for getting a reference to an instance of the Helpers class.
        /// This class contains useful methods for integrating your game with Xbox.
        /// </summary>
        /// <returns>The singleton instance of the Helpers class.</returns>
        public static WSManager Helpers
        {
            get
            {
                if (m_XboxHelpers == null)
                {
                    WSManager[] xboxHelperInstances = FindObjectsOfType<WSManager>();
                    if (xboxHelperInstances.Length > 0)
                    {
                        m_XboxHelpers = xboxHelperInstances[0];
                        m_XboxHelpers.Initialize();
                    }
                    else
                    {
                        Debug.LogError("Error: Could not find Xbox prefab. Make sure you have added the Xbox prefab to your scene.");
                    }
                }

                return m_XboxHelpers;
            }
        }

        public delegate void OnGameSaveLoadedHandler(object sender, GameSaveLoadedArgs e);
#pragma warning disable 0067 // Called when MICROSOFT_GAME_CORE is defined
        public event OnGameSaveLoadedHandler OnGameSaveLoaded;
#pragma warning restore 0067

        public delegate void OnErrorHandler(object sender, ErrorEventArgs e);
        public event OnErrorHandler OnError;

        public Action<bool> InitializationCompleted;

        public async Task Initialize(WSConfig config)
        {
            m_Config = config;

            Initialize();

            InitializeInternalService(config);

            await Task.CompletedTask;
        }

        private void InitializeInternalService(WSConfig config)
        {
            PlatformServices platformServices = new PlatformServices();

            platformServices.Storage = PlatformManager.CreateStorageService(config, new WSStorage());
            platformServices.Achievements = new WSAchievementUnlocker();

            PlatformManager.Instance.SetPlatformServices(platformServices);
        }

        private void Initialize()
        {
            if (m_InitializationState == InitializationState.Initializing || m_InitializationState == InitializationState.Intialized)
            {
                return;
            }
            m_InitializationState = InitializationState.Initializing;

            DontDestroyOnLoad(gameObject);

            m_ResultToFriendlyErrorLookup = new Dictionary<int, string>();
            InitializeHresultToFriendlyErrorLookup();

#if MICROSOFT_GAME_CORE
            try
            {
                if (!Succeeded(SDK.XGameRuntimeInitialize(), "Initialize gaming runtime"))
                {
#if UNITY_EDITOR
                    Debug.LogError("You may need to update your config file for the editor. GDK -> PC -> Update Editor Game Config will copy your current game config to the Unity.exe location to enable GDK features when playing in-editor.");
#endif
                    return;
                }
            }catch(Exception e)
            {
                Debug.LogError(e);
            }


            // Check for store updates
            int hresult = SDK.XStoreCreateContext(out m_StoreContext);
            if (Succeeded(hresult, "Create store context"))
            {
                SDK.XStoreQueryGameAndDlcPackageUpdatesAsync(m_StoreContext, HandleQueryForUpdatesComplete);
            }

            GameSaveHelper = new XGameSaveWrapper();
            if (m_Config.SignInOnStart)
            {
                SignIn();
            }
            else
            {
                m_InitializationState = InitializationState.Intialized;
            }
#endif
        }

        private void InitializeHresultToFriendlyErrorLookup()
        {
            if (m_ResultToFriendlyErrorLookup == null)
            {
                return;
            }

            m_ResultToFriendlyErrorLookup.Add(-2143330041, "IAP_UNEXPECTED: Does the player you are signed in as have a license for the game? " +
                "You can get one by downloading your game from the store and purchasing it first. If you can't find your game in the store, " +
                "have you published it in Partner Center?");

            m_ResultToFriendlyErrorLookup.Add(-1994108656, "E_GAMEUSER_NO_PACKAGE_IDENTITY: Are you trying to call GDK APIs from the Unity editor?" +
                " To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once your" +
                "game is launched.");

            m_ResultToFriendlyErrorLookup.Add(-1994129152, "E_GAMERUNTIME_NOT_INITIALIZED: Are you trying to call GDK APIs from the Unity editor?" +
                " To call GDK APIs, you must use the GDK > Build and Run menu. You can debug your code by attaching the Unity debugger once your" +
                "game is launched.");

            m_ResultToFriendlyErrorLookup.Add(-2015559675, "AM_E_XAST_UNEXPECTED: Have you added the Windows 10 PC platform on the Xbox Settings page " +
                "in Partner Center? Learn more: aka.ms/sandboxtroubleshootingguide");
        }

        public void SignIn()
        {
#if MICROSOFT_GAME_CORE
            SignInImpl();
#endif
        }

        public void Save(byte[] data)
        {
#if MICROSOFT_GAME_CORE
            GameSaveHelper.Save(
                GAME_SAVE_CONTAINER_NAME,
                GAME_SAVE_BLOB_NAME,
                data,
                GameSaveSaveCompleted);
#endif
        }

        public void LoadSaveData()
        {
#if MICROSOFT_GAME_CORE
            GameSaveHelper.Load(
                GAME_SAVE_CONTAINER_NAME,
                GAME_SAVE_BLOB_NAME,
                GameSaveLoadCompleted);
#endif
        }

#if MICROSOFT_GAME_CORE
        private void SignInImpl()
        {
            XUserAddOptions options = XUserAddOptions.AddDefaultUserAllowingUI;
            SDK.XUserAddAsync(options, AddUserComplete);
        }

        private void AddUserComplete(int hresult, XUserHandle userHandle)
        {
            if (!Succeeded(hresult, "Sign in."))
            {
                return;
            }

            UserHandle = userHandle;
            CompletePostSignInInitialization();
        }

        private void CompletePostSignInInitialization()
        {
            Succeeded(SDK.XBL.XblInitialize(m_Config.Scid), "Initialize Xbox Live");
            Succeeded(SDK.XBL.XblContextCreateHandle(UserHandle, out XblContextHandle), "Create Xbox Live context");
            InitializeGameSaves();
        }

        public string GetGamertag()
        {
            SDK.XUserGetGamertag(UserHandle, XUserGamertagComponent.UniqueModern, out var gamertag);
            return gamertag;
        }

        private void InitializeGameSaves()
        {
            GameSaveHelper.InitializeAsync(UserHandle, m_Config.Scid, XGameSaveInitializeCompleted);
        }

        private void XGameSaveInitializeCompleted(int hresult)
        {
            InitializationCompleted?.Invoke(Succeeded(hresult, "Initialize game save provider"));
            if (!Succeeded(hresult, "Initialize game save provider"))
            {
                return;
            }
            m_InitializationState = InitializationState.Intialized;
        }

        private void GameSaveSaveCompleted(int hresult)
        {
            Succeeded(hresult, "Game save submit update complete");
        }

        private void GameSaveLoadCompleted(int hresult, byte[] savedData)
        {
            if (!Succeeded(hresult, "Loaded Blob"))
            {
                return;
            }

            if (Helpers.OnGameSaveLoaded != null)
            {
                Helpers.OnGameSaveLoaded(Helpers, new GameSaveLoadedArgs(savedData));
            }
        }

        private void ProcessAssociatedProductsResults(Int32 hresult, XStoreQueryResult result)
        {
            if (Succeeded(hresult, "GetAssociatedProductsAsync callback"))
            {
                m_AssociatedProducts.AddRange(result.PageItems);
                if (result.HasMorePages)
                {
                    SDK.XStoreQueryAssociatedProductsAsync(
                        m_StoreContext,
                        m_AddOnProducts,
                        MAX_ASSOCIATED_PRODUCTS_TO_RETRIEVE,
                        ProcessAssociatedProductsResults
                        );
                }
                else
                {
                    if (m_QueryAssociatedProductsCallback != null)
                    {
                        m_QueryAssociatedProductsCallback(hresult, m_AssociatedProducts);
                    }
                }
            }
            else
            {
                if (m_QueryAssociatedProductsCallback != null)
                {
                    m_QueryAssociatedProductsCallback(hresult, m_AssociatedProducts);
                }
            }
        }

        public void GetAssociatedProductsAsync(GetAssociatedProductsCallback callback)
        {
            if (callback == null)
            {
                Debug.LogError("Callback cannot be null.");
            }

            m_AssociatedProducts = new List<XStoreProduct>();
            m_QueryAssociatedProductsCallback = callback;
            Succeeded(SDK.XStoreCreateContext(out m_StoreContext), "Failed to create store context.");
            SDK.XStoreQueryAssociatedProductsAsync(
                m_StoreContext,
                m_AddOnProducts,
                MAX_ASSOCIATED_PRODUCTS_TO_RETRIEVE,
                ProcessAssociatedProductsResults
                );
        }

        public void ShowPurchaseUIAsync(XStoreProduct storeProduct, ShowPurchaseUICallback callback)
        {
            SDK.XStoreShowPurchaseUIAsync(
                    m_StoreContext,
                    storeProduct.StoreId,
                    null,
                    null,
                    (Int32 hresult) =>
                    {
                        callback(hresult, storeProduct);
                    });
        }

        private void HandleQueryForUpdatesComplete(int hresult, XStorePackageUpdate[] packageUpdates)
        {
            List<string> _packageIdsToUpdate = new List<string>();
            if (hresult >= 0)
            {
                if (packageUpdates != null &&
                    packageUpdates.Length > 0)
                {
                    foreach (XStorePackageUpdate packageUpdate in packageUpdates)
                    {
                        _packageIdsToUpdate.Add(packageUpdate.PackageIdentifier);
                    }
                    // What do we do?
                    SDK.XStoreDownloadAndInstallPackageUpdatesAsync(
                        m_StoreContext,
                        _packageIdsToUpdate.ToArray(),
                        DownloadFinishedCallback);
                }
            }
            else
            {
                // No-op
            }
        }

        private void DownloadFinishedCallback(int hresult)
        {
            Succeeded(hresult, "DownloadAndInstallPackageUpdates callback");
        }
#endif

        // Update is called once per frame
        void Update()
        {
#if MICROSOFT_GAME_CORE
            SDK.XTaskQueueDispatch();
#endif
        }

        // Helper methods
        public static bool Succeeded(int hresult, string operationFriendlyName)
        {
            bool succeeded = false;
#if MICROSOFT_GAME_CORE
            if (HR.SUCCEEDED(hresult))
            {
                succeeded = true;
            }
            else
            {
                string errorCode = hresult.ToString("X8");
                string errorMessage = string.Empty;
                if (m_ResultToFriendlyErrorLookup.ContainsKey(hresult))
                {
                    errorMessage = m_ResultToFriendlyErrorLookup[hresult];
                }
                else
                {
                    errorMessage = operationFriendlyName + " failed.";
                }
                string formattedErrorString = string.Format("{0} Error code: hr=0x{1}", errorMessage, errorCode);
                Debug.LogError(formattedErrorString);
                if (Helpers.OnError != null)
                {
                    Helpers.OnError(Helpers, new ErrorEventArgs(errorCode, errorMessage));
                }
            }
#endif
            return succeeded;
        }


#region DATA HELPERS
        /// Convert an object to a byte array
        public static byte[] ObjectToByteArray(object obj)
        {
            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        /// Convert a byte array to an Object
        public static object ByteArrayToObject(byte[] arrBytes)
        {
            using var memStream = new MemoryStream(arrBytes);
            var binForm = new BinaryFormatter();
            var obj = binForm.Deserialize(memStream);
            return obj;
        }

        #endregion
    }
}