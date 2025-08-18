using System.Threading.Tasks;
using UnityEngine;
using System.Collections.Generic;
using System;

#if UNITY_GAMECORE
using Unity.GameCore;
using UnityEngine.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GameCoreUserController
    {
#if UNITY_GAMECORE

        /// <summary>
        /// Initialize the current user with the provided XUserHandle
        /// Initialize User context and Store Content
        /// </summary>
        /// <param name="userHandle"></param>
        /// <returns></returns>
        public async Task<bool> InititalizeUser(XUserHandle userHandle)
        {
            GameCoreManager.Instance.XboxUser.XboxUserHandle = userHandle;
            var hResult = GameCoreOperationResults.Invalid;


            SDK.XGameSaveInitializeProviderAsync(userHandle, GameCoreSettings.SCID, false,
                (hr, providerHandle) =>
                {
                    GameCoreManager.Instance.XboxUser.GameSaveProviderHandle = providerHandle;
                    hResult = hr;
                    if (providerHandle == null)
                    {
                        Debug.Log("ProviderHandle is NULL");
                    }
                }
            );

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.FAILED(hResult))
            {
                Debug.Log($"[INIT] GameSave Result failed: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            InitializeUserXboxContext(userHandle);
            InitializeUserStoreContext(userHandle);

            GameCoreManager.Instance.XboxUser.XUID = GetUserXUID(userHandle);
            GameCoreManager.Instance.XboxUser.XUserLocalID = GetLocalID(userHandle);

            return true;
        }

        /// <summary>
        /// Get user for Xbox with the options as parameter.
        /// </summary>
        /// <param name="xUserAddOptions">Determine if the user has to Show the UI</param>
        /// <returns></returns>
        public async Task<XUserHandle> GetUser(XUserAddOptions xUserAddOptions)
        {
            var hResult = GameCoreOperationResults.Invalid;
            var userHandle = (XUserHandle)null;

            SDK.XUserAddAsync(xUserAddOptions, (int hresult, XUserHandle handle) =>
            {
                hResult = hresult;
                userHandle = handle;
            });

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.SUCCEEDED(hResult) && userHandle != null)
            {
                return userHandle;
            }
            else
            {
                Debug.LogError($"[Get User] Failed to get user:{GameCoreOperationResults.GetName(hResult)}");
                return null;
            }
        }

        /// <summary>
            /// Save a valid store Licence to the player to close it later.
            /// </summary>
            /// <param name="storeId">Pacakage storeID.</param>
            /// <param name="storeLicense">XStore license of the package to release when no longer needed.</param>
        public void AddStoreLicenseToCurrentUser(string storeId, XStoreLicense storeLicense)
        {
            if (GameCoreManager.Instance.XboxUser.StoreLicenses == null)
            {
                GameCoreManager.Instance.XboxUser.StoreLicenses = new Dictionary<string, XStoreLicense>();
            }
            GameCoreManager.Instance.XboxUser.StoreLicenses.Add(storeId, storeLicense);
        }

        /// <summary>
        /// Close and clear all valida store licenses of the current user.
        /// </summary>
        public void CloseValidStoreLicenses()
        {
            if (GameCoreManager.Instance.XboxUser.StoreLicenses == null)
            {
                return;
            }
            foreach (var storeLicense in GameCoreManager.Instance.XboxUser.StoreLicenses)
            {
                if (storeLicense.Value == null)
                {
                    continue;
                }
                SDK.XStoreCloseLicenseHandle(storeLicense.Value);
            }
            GameCoreManager.Instance.XboxUser.StoreLicenses.Clear();
        }

        /// <summary>
        /// Return the Xbox User ID
        /// Returns 0 if the XUserHandle is null
        /// </summary>
        /// <param name="userHandle"></param>
        /// <returns></returns>
        public static ulong GetUserXUID(XUserHandle userHandle)
        {
            if (userHandle == null)
            {
                Debug.Log("[ID] No user handle");
                return 0;
            }

            ulong userXUID;
            int hResult = SDK.XUserGetId(userHandle, out userXUID);
            if (HR.FAILED(hResult))
            {
                Debug.Log($"[INIT] Get XUID failed: {GameCoreOperationResults.GetName(hResult)}");
                return 0;
            }
            else
            {
                return userXUID;
            }
        }

        /// <summary>
        /// Returns the XUserLocalID of a XUserHandle
        /// Default XUserHandle value will be returne if handle is null
        /// </summary>
        /// <param name="handle"></param>
        /// <returns>Returns the XUserLocalID</returns>
        public XUserLocalId GetLocalID(XUserHandle handle)
        {
            if (handle == null)
            {
                Debug.Log("[ID] No user handle");
                return default(XUserLocalId);
            }
            var hResult = GameCoreOperationResults.Invalid;
            XUserLocalId userLocalID;
            hResult = SDK.XUserGetLocalId(handle, out userLocalID);
            if (HR.FAILED(hResult))
            {
                Debug.Log($"[INIT] Get LocalID failed: {GameCoreOperationResults.GetName(hResult)}");
                return default(XUserLocalId);
            }
            else
            {
                return userLocalID;
            }
        }

        /// <summary>
        /// Check if the user passed as parameter is the previous user saved.
        /// </summary>
        /// <param name="newUserhandle"></param>
        /// <returns>Returns True if is the previous user.</returns>
        public static bool IsPreviousUser(XUserHandle newUserhandle)
        {
            if (newUserhandle == null)
            {
                return false;
            }
            return GameCoreManager.Instance.PreviousId == GetUserXUID(newUserhandle);
        }

        /// <summary>
        /// Check if 2 XUserHandles are the same
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns>Returns true if are the same, false if not</returns>
        public static bool AreEqualUserHandle(XUserHandle one, XUserHandle two)
        {
            return (HR.SUCCEEDED(SDK.XUserCompare(one, two, out int compareResult))
                && (0 == compareResult));
        }

        /// <summary>
        /// Check if 2 XUserLocalIDs are the same
        /// </summary>
        /// <param name="one"></param>
        /// <param name="two"></param>
        /// <returns>Returns true if are the same, false if not</returns>
        public static bool AreEqualLocalId(XUserLocalId one, XUserLocalId two)
        {
            return one.value == two.value;
        }

        /// <summary>
        /// Get the user Gamer Tag with provided format, Classic as default
        /// </summary>
        /// <param name="userHandle"></param>
        /// <param name="gamerTagFormat"></param>
        /// <returns></returns>
        public static string GetGamerTag(XUserHandle userHandle, XUserGamertagComponent gamerTagFormat = XUserGamertagComponent.Classic)
        {
            string gamerTag = "";
            if (userHandle != null)
            {
                int hResult = SDK.XUserGetGamertag(userHandle, gamerTagFormat, out gamerTag);

                if (HR.FAILED(hResult))
                {
                    Debug.Log($"[GAMERTAG] Failed to get gamertag:{GameCoreOperationResults.GetName(hResult)}");
                }
                return gamerTag;
            }
            else
            {
                Debug.Log($"[GAMERTAG] User not valid");
                return gamerTag;
            }
        }

        private void InitializeUserXboxContext(XUserHandle userHandle)
        {
            int hResult = SDK.XBL.XblContextCreateHandle(userHandle, out GameCoreManager.Instance.XboxUser.XboxContextHandle);
            if (HR.FAILED(hResult))
            {
                Debug.Log("[XBOX_CONTEXT] Failed to get context. ");
            }
        }

        private void InitializeUserStoreContext(XUserHandle userHandle)
        {
            if (userHandle == null)
            {
                Debug.Log("[SERIESXS] No user, no store content initialized.");
                return;
            }
            int hResult = SDK.XStoreCreateContext(userHandle, out XStoreContext storeContext);
            if (HR.FAILED(hResult))
            {
                Debug.Log($"[ADD_CON] Couldn't create store context:{GameCoreOperationResults.GetName(hResult)}");
                return;
            }
            GameCoreManager.Instance.XboxUser.StoreContext = storeContext;
        }

#endif
    }

}
