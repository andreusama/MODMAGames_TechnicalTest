using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;
using System.Threading.Tasks;
using System;
using PetoonsStudio.PSEngine.Framework;

#if UNITY_GAMECORE
using Unity.GameCore;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.GameCore
{
    public class GameCoreStorage : IStorage
    {
        [Obsolete("Use SaveExists(fileName, folderName) instead")]
        public static async Task<bool> SaveExists(string fileName)
        {
            var saveExists = false;
#if UNITY_GAMECORE

            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[SAVE_EXIST] User not initialized!");
                return saveExists;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return saveExists;
            }

            int hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveEnumerateBlobInfoByName(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, fileName, out XGameSaveBlobInfo[] blobInfos);

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.SUCCEEDED(hResult))
            {
                int indexOfBlobWithName = Array.FindIndex
                (
                    blobInfos,
                    (binfo) =>
                    {
                        return (binfo?.Name ?? "INVALID_BLOB_NAME") == fileName;
                    }
                );

                return GameCoreUtilities.IndexIsValid(indexOfBlobWithName);
            }
            else
            {
                Debug.Log($"[SAVE_EXIST] Enumerate blob failed {blobInfos?.Length}, blobs matching name {fileName}");
            }
#endif
            return await Task.FromResult(saveExists);
        }

        public async Task<bool> SaveExists(string fileName, string folderName)
        {
            var saveExists = false;
#if UNITY_GAMECORE

            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[SAVE_EXIST] User not initialized!");
                return saveExists;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return saveExists;
            }

            int hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveEnumerateBlobInfoByName(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, fileName, out XGameSaveBlobInfo[] blobInfos);

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.SUCCEEDED(hResult))
            {
                int indexOfBlobWithName = Array.FindIndex
                (
                    blobInfos,
                    (binfo) =>
                    {
                        return (binfo?.Name ?? "INVALID_BLOB_NAME") == fileName;
                    }
                );

                return GameCoreUtilities.IndexIsValid(indexOfBlobWithName);
            }
            else
            {
                Debug.Log($"[SAVE_EXIST] Enumerate blob failed {blobInfos?.Length}, blobs matching name {fileName}");
            }
#endif
            return await Task.FromResult(saveExists);
        }

        [Obsolete("Use Save(fileName, folderName) instead")]
        public static async Task<bool> Save(object saveObject, string fileName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[SAVE] User not initialized!");
                return false;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return false;
            }

            int hResult = SDK.XGameSaveCreateUpdate(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle,
                                                    fileName, 
                                                    out XGameSaveUpdateHandle updateHandle);

            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveCreateUpdate returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            var bf = new BinaryFormatter();
            byte[] data = null;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, saveObject);
                data = ms.ToArray();
            }

            hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveSubmitBlobWrite(updateHandle, fileName, data);
            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveSubmitBlobWrite returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;
            SDK.XGameSaveSubmitUpdateAsync(updateHandle, (hr) =>
            {
                hResult = hr;
            });

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveSubmitUpdateAsync returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            return true;
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[SAVE] User not initialized!");
                return false;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return false;
            }

            int hResult = SDK.XGameSaveCreateUpdate(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle,
                                                    fileName, 
                                                    out XGameSaveUpdateHandle updateHandle);

            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveCreateUpdate returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            var bf = new BinaryFormatter();
            byte[] data = null;
            using (var ms = new MemoryStream())
            {
                bf.Serialize(ms, saveObject);
                data = ms.ToArray();
            }

            hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveSubmitBlobWrite(updateHandle, fileName, data);
            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveSubmitBlobWrite returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;
            SDK.XGameSaveSubmitUpdateAsync(updateHandle, (hr) =>
            {
                hResult = hr;
            });

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"[SAVE] SDK.XGameSaveSubmitUpdateAsync returned {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            return true;
#else
            return await Task.FromResult(false);
#endif
        }

        [Obsolete("Use Load(fileName, folderName) instead")]
        public static async Task<T> Load<T>(string fileName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[LOAD] User not initialized!");
                return (T)default;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return (T)default;
            }

            int hResult = GameCoreOperationResults.Invalid;
            var arrayOfLoadedBlobData = (XGameSaveBlob[])null;

            SDK.XGameSaveReadBlobDataAsync
            (
                GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, new[] { fileName },
                (hr, arrayOfBlobs) =>
                {
                    hResult = hr;
                    arrayOfLoadedBlobData = arrayOfBlobs;
                }
            );

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.SUCCEEDED(hResult) && ((arrayOfLoadedBlobData?.Length ?? 0) > 0))
            {
                var gameData = (byte[])null;
                gameData = arrayOfLoadedBlobData[0].Data;
                object dataObject;
                using (var memStream = new MemoryStream())
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(gameData, 0, gameData.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    dataObject = binForm.Deserialize(memStream);
                }

                return dataObject != null ? (T)dataObject : default;
            }
            else
            {
                Debug.LogWarning($"[LOAD] Load:{fileName} Result failed: {GameCoreOperationResults.GetName(hResult)}. ");
                return (T)default;
            }
#else
            return await Task.FromResult<T>(default); 
#endif
        }

        public async Task<T> Load<T>(string fileName, string folderName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[LOAD] User not initialized!");
                return (T)default;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return (T)default;
            }

            int hResult = GameCoreOperationResults.Invalid;
            var arrayOfLoadedBlobData = (XGameSaveBlob[])null;

            SDK.XGameSaveReadBlobDataAsync
            (
                GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, new[] { fileName },
                (hr, arrayOfBlobs) =>
                {
                    hResult = hr;
                    arrayOfLoadedBlobData = arrayOfBlobs;
                }
            );

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (HR.SUCCEEDED(hResult) && ((arrayOfLoadedBlobData?.Length ?? 0) > 0))
            {
                var gameData = (byte[])null;
                gameData = arrayOfLoadedBlobData[0].Data;
                object dataObject;
                using (var memStream = new MemoryStream())
                {
                    BinaryFormatter binForm = new BinaryFormatter();
                    memStream.Write(gameData, 0, gameData.Length);
                    memStream.Seek(0, SeekOrigin.Begin);
                    dataObject = binForm.Deserialize(memStream);
                }

                return dataObject != null ? (T)dataObject : default;
            }
            else
            {
                Debug.LogWarning($"[LOAD] Load:{fileName} Result failed: {GameCoreOperationResults.GetName(hResult)}. ");
                return (T)default;
            }
#else
            return await Task.FromResult((T)default);
#endif
        }

        [Obsolete("Use DeleteSave(fileName, folderName) instead")]
        public static async Task<bool> DeleteSave(string fileName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[DELETE] User not initialized!");
                return false;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return false;
            }

            int hResult = GameCoreOperationResults.Invalid;
            XGameSaveUpdateHandle updateHandle = null;
            hResult = SDK.XGameSaveCreateUpdate(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, fileName, out updateHandle);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Create update: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveSubmitBlobDelete(updateHandle, fileName);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Submit Blob Delete: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;

            SDK.XGameSaveSubmitUpdateAsync(updateHandle, (hr) =>
            {
                hResult = hr;
            });

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Submit Update: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            SDK.XGameSaveCloseUpdateHandle(updateHandle);
            return true;
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
#if UNITY_GAMECORE
            if (!GameCoreManager.Instance.IsUserInitialized)
            {
                Debug.Log("[DELETE] User not initialized!");
                return false;
            }

            if (!CreateUserGameSaveContainer(fileName, GameCoreManager.Instance.XboxUser.GameSaveProviderHandle))
            {
                return false;
            }

            int hResult = GameCoreOperationResults.Invalid;
            XGameSaveUpdateHandle updateHandle = null;
            hResult = SDK.XGameSaveCreateUpdate(GameCoreManager.Instance.XboxUser.GameSaveContainerHandle, fileName, out updateHandle);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Create update: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;
            hResult = SDK.XGameSaveSubmitBlobDelete(updateHandle, fileName);
            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Submit Blob Delete: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            hResult = GameCoreOperationResults.Invalid;

            SDK.XGameSaveSubmitUpdateAsync(updateHandle, (hr) =>
            {
                hResult = hr;
            });

            while (GameCoreOperationResults.IsInvalid(hResult))
            {
                await Task.Yield();
            }

            if (!HR.SUCCEEDED(hResult))
            {
                Debug.Log($"[DELETE] Error on Submit Update: {GameCoreOperationResults.GetName(hResult)}");
                return false;
            }

            SDK.XGameSaveCloseUpdateHandle(updateHandle);
            return true;
#else
            return await Task.FromResult(false);
#endif
        }

#if UNITY_GAMECORE
        private static bool CreateUserGameSaveContainer(string containerName, XGameSaveProviderHandle gameSaveProvider)
        {
            int hResult = SDK.XGameSaveCreateContainer(gameSaveProvider, containerName, out GameCoreManager.Instance.XboxUser.GameSaveContainerHandle);

            if (HR.FAILED(hResult))
            {
                Debug.LogWarning($"Create container failed: {GameCoreOperationResults.GetName(hResult)}, Args Provider:{gameSaveProvider==null}/ Name:{containerName}");
                return false;
            }

            return true;
        }
#endif
    }
}