using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Framework;

#if UNITY_PS4
using Sony.PS4.SaveData;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS4
{
    public class PS4Storage : IStorage
    {
#if UNITY_PS4
        private const Mounting.MountModeFlags SAVE_FLAGS = Mounting.MountModeFlags.Create2 | Mounting.MountModeFlags.ReadWrite;
        private const Mounting.MountModeFlags LOAD_FLAGS = Mounting.MountModeFlags.ReadOnly;
#endif
        private const string DEFAULT_SAVEICON_PATH = "/app0/Media/StreamingAssets/SaveIcon.png";
        private static string m_SaveIconPath = DEFAULT_SAVEICON_PATH;

        public async Task<bool> SaveExists(string fileName, string folderName)
        {
#if UNITY_PS4
            Mounting.MountPoint mp = null;
            try
            {
                var foundedData = await SearchInfo(mp, fileName);
                if (foundedData == null) return false;
                bool exists = Array.Find(foundedData, (Searching.SearchSaveDataItem x) => x.DirName.Data == fileName) != null;
                return exists;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#endif
            return await Task.FromResult(false);
        }

        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
#if UNITY_PS4
            Log($"SAVE Called");
            Mounting.MountPoint mp = null;
            try
            {
                byte[] data;

                BinaryFormatter bf = new BinaryFormatter();
                using (var ms = new MemoryStream())
                {
                    bf.Serialize(ms, saveObject);
                    data = ms.ToArray();
                    Log($"SAVE BinaryFormatter created with {ms.Length} MemoryStream Lenght");
                    Log($"SAVE BinaryFormatter created with {data.Length} Data lenght");
                }

                mp = await InitializeMountPoint(SAVE_FLAGS, fileName, BlocksFromBytes(data.Length));

                if (mp == null) return false;
                if (!await SaveFiles(mp, data, fileName)) return false;
                if (!await WriteIcon(mp)) return false;
                if (!await WriteParams(mp)) return false;
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                Log($"SAVE Ended unsuccessfully");
                return false;
            }
            finally
            {
                if (mp != null)
                    await Unmount(mp, true);
            }

            Log($"SAVE Ended successfully");
            return true;
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<T> Load<T>(string fileName, string folderName)
        {
#if UNITY_PS4
            Mounting.MountPoint mp = null;
            object loadedData = null;
            try
            {
                mp = await InitializeMountPoint(LOAD_FLAGS, fileName);
                if (mp == null) return default(T);
                loadedData = await LoadFiles(mp, fileName);
                if (loadedData == null) return default(T);


            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return default;
            }
            finally
            {
                if (mp != null)
                    await Unmount(mp, true);
            }

            return loadedData != null ? (T)loadedData : default;
#else
            return await Task.FromResult((T)default);
#endif
        }

        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
#if UNITY_PS4
            try
            {
                if (!await SaveExists(fileName, folderName)) return false;

                return await Delete(fileName);
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                return false;
            }
#else
            return await Task.FromResult(false);
#endif
        }

#if UNITY_PS4
        /// <summary>
        /// Change the Save Icon that will be used to save. Note that you need to provide a new path to like: /app0/Media/StreamingAssets/SaveIcon.png
        /// </summary>
        /// <param name="saveIconPath"></param>
        public static void ChangeSaveIcon(string saveIconPath)
        {
            m_SaveIconPath = saveIconPath;
        }

        private static async Task<Mounting.MountPoint> InitializeMountPoint(Mounting.MountModeFlags flags, string fileName, ulong requiredBlocks = Mounting.MountRequest.BLOCKS_MIN)
        {
            Log($"INITIALIZE MOUNT POINT Called");
            requiredBlocks = requiredBlocks < Mounting.MountRequest.BLOCKS_MIN ? Mounting.MountRequest.BLOCKS_MIN : requiredBlocks;

            Mounting.MountRequest request = new Mounting.MountRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                IgnoreCallback = true,
                DirName = new DirName() { Data = fileName },
                MountMode = flags,
                Blocks = requiredBlocks,
                Async = true
            };
            Log($"Initialize Mount Point for {fileName} with {flags} flags and {request.Blocks} blocks");
            Mounting.MountResponse mountResponse = new Mounting.MountResponse();

            Mounting.Mount(request, mountResponse);

            await TaskUtils.WaitUntil(() => !mountResponse.Locked);

            if (!mountResponse.IsErrorCode)
            {
                Log($"INITIALIZE MOUNT POINT Ended successfully");
                return mountResponse.MountPoint;
            }

            if (flags == LOAD_FLAGS)
                await HandleLoadError(mountResponse.ReturnCodeValue, mountResponse.MountPoint);
            else if (flags == SAVE_FLAGS)
                await HandleSaveError(mountResponse.ReturnCodeValue, mountResponse.MountPoint, mountResponse.RequiredBlocks);

            Log($"INITIALIZE MOUNT POINT Ended unsuccessfully");
            return null;
        }

        private static async Task<Searching.SearchSaveDataItem[]> SearchInfo(Mounting.MountPoint mp, string fileName)
        {
            var searchRequest = new Searching.DirNameSearchRequest()
            {
                DirName = new DirName() { Data = fileName },
                UserId = PS4Manager.Instance.InitialUserId
            };
            var searchResponse = new Searching.DirNameSearchResponse();

            Searching.DirNameSearch(searchRequest, searchResponse);

            await TaskUtils.WaitUntil(() => !searchResponse.Locked);

            if (searchResponse.IsErrorCode)
                return null;

            return searchResponse.SaveDataItems;
        }

        private static async Task<bool> SaveFiles(Mounting.MountPoint mp, byte[] saveData, string fileName)
        {
            // Actual custom file operation to perform on the savedata, once it is mounted.
            WriteFilesRequest fileRequest = new WriteFilesRequest
            {
                Data = saveData,
                IgnoreCallback = false,
                FileName = fileName,
                MountPointName = mp.PathName,
                Async = true,
                UserId = PS4Manager.Instance.InitialUserId
            };

            WriteFilesResponse fileResponse = new WriteFilesResponse();

            // Do actual saving
            FileOps.CustomFileOp(fileRequest, fileResponse);
            await TaskUtils.WaitUntil(() => !fileResponse.Locked);

            if (!fileResponse.IsErrorCode) return true;

            await HandleSaveError(fileResponse.ReturnCodeValue, mp, (ulong)fileRequest.Data.Length);

            return false;
        }

        private static async Task<bool> WriteIcon(Mounting.MountPoint mp)
        {
            // Write the icon and any detail parmas set here.
            EmptyResponse iconResponse = new EmptyResponse();

            // Create the new item for the saves dialog list
            Dialogs.NewItem newItem = new Dialogs.NewItem
            {
                IconPath = m_SaveIconPath,
                Title = Application.productName
            };

            Mounting.SaveIconRequest request = new Mounting.SaveIconRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                MountPointName = mp.PathName,
                RawPNG = newItem.RawPNG,
                IconPath = newItem.IconPath,
                IgnoreCallback = true
            };

            Mounting.SaveIcon(request, iconResponse);
            await TaskUtils.WaitUntil(() => !iconResponse.Locked);

            if (!iconResponse.IsErrorCode) return true;
            else return false;
        }

        private static async Task<bool> WriteParams(Mounting.MountPoint mp)
        {
            EmptyResponse paramsResponse = new EmptyResponse();

            SaveDataParams saveDataParams = new SaveDataParams
            {
                Title = Application.productName
            };

            Mounting.SetMountParamsRequest request = new Mounting.SetMountParamsRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                MountPointName = mp.PathName,
                IgnoreCallback = true,
                Params = saveDataParams
            };

            Mounting.SetMountParams(request, paramsResponse);

            // Wait for save icon to be mounted.
            await TaskUtils.WaitUntil(() => !paramsResponse.Locked);

            if (!paramsResponse.IsErrorCode) return true;
            else return false;
        }

        private static async Task<bool> Unmount(Mounting.MountPoint mp, bool backup)
        {
            EmptyResponse unmountResponse = new EmptyResponse();

            Mounting.UnmountRequest request = new Mounting.UnmountRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                MountPointName = mp.PathName,
                Backup = backup,
                IgnoreCallback = true
            };

            Mounting.Unmount(request, unmountResponse);
            await TaskUtils.WaitUntil(() => !unmountResponse.Locked);

            if (!unmountResponse.IsErrorCode) return true;
            else return false;
        }

        private static async Task<object> LoadFiles(Mounting.MountPoint mp, string fileName)
        {
            var fileRequest = new ReadFilesRequest
            {
                IgnoreCallback = false,
                FileName = fileName,
                MountPointName = mp.PathName,
                Async = true,
                UserId = PS4Manager.Instance.InitialUserId
            };

            var fileResponse = new ReadFilesResponse();

            // Do actual loading
            FileOps.CustomFileOp(fileRequest, fileResponse);

            await TaskUtils.WaitUntil(() => !fileResponse.Locked);

            if (fileResponse.IsErrorCode)
            {
                await HandleLoadError(fileResponse.ReturnCodeValue, mp);
                return null;
            }

            using var memStream = new MemoryStream();
            var binForm = new BinaryFormatter();
            memStream.Write(fileResponse.Data, 0, fileResponse.Data.Length);
            memStream.Seek(0, SeekOrigin.Begin);
            var data = binForm.Deserialize(memStream);

            return data;
        }

        private static async Task<bool> Delete(string fileName)
        {
            Log($"DELETE Called");

            var deleteRequest = new Deleting.DeleteRequest()
            {
                UserId = PS4Manager.Instance.InitialUserId,
                DirName = new DirName() { Data = fileName }
            };
            var deleteResponse = new EmptyResponse();

            Deleting.Delete(deleteRequest, deleteResponse);

            await TaskUtils.WaitUntil(() => !deleteResponse.Locked);

            if (!deleteResponse.IsErrorCode)
            {
                Log($"DELETE Ended successfully");
                return true;
            }
            else
            {
                Log($"DELETE Ended unsuccessfully");
                return false;
            }
        }

        private static async Task HandleSaveError(int errorCode, Mounting.MountPoint mp, ulong requiredBlocks)
        {
            //Showed space will be the real space + 32Mb for 
            if ((uint)errorCode == (uint)ReturnCodes.DATA_ERROR_NO_SPACE_FS)
            {
                Log($"HANDLE SAVE ERROR: NO_SPACE for {mp.DirName} with {requiredBlocks} blocks");
                NoSpaceContinuableSystemMessageDialog(requiredBlocks);
            }
            else if ((uint)errorCode == (uint)ReturnCodes.SAVE_DATA_ERROR_BROKEN)
            {
                Log($"HandleSaveError: CORRUPTED DATA for {mp.DirName}");
                CorruptedDataSystemMessageDialog(mp.DirName, Dialogs.DialogType.Save);
                await Delete(mp.DirName.Data);
            }
        }

        private static async Task HandleLoadError(int errorCode, Mounting.MountPoint mp)
        {
            if ((uint)errorCode == (uint)ReturnCodes.SAVE_DATA_ERROR_BROKEN)
            {
                Log($"HANDLE LOAD ERROR: NO_SPACE for {mp.DirName}");
                CorruptedDataSystemMessageDialog(mp.DirName, Dialogs.DialogType.Load);
                await Delete(mp.DirName.Data);
            }
        }

        #region HELPERS
        private static void NoSpaceContinuableSystemMessageDialog(ulong requiredBlocks)
        {
            var request = new Dialogs.OpenDialogRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                Mode = Dialogs.DialogMode.SystemMsg,
                DispType = Dialogs.DialogType.Save,

                // A system mesage has some specific param requests to fill
                SystemMessage = new Dialogs.SystemMessageParam()
                {
                    SysMsgType = Dialogs.SystemMessageType.NoSpaceContinuable,
                    Value = requiredBlocks  // This will show as the ammount of blocks needed to free
                },

                Async = false
            };

            var response = new Dialogs.OpenDialogResponse();
            var requestId = Dialogs.OpenDialog(request, response);
        }

        private static void CorruptedDataSystemMessageDialog(DirName dirName, Dialogs.DialogType type)
        {
            var request = new Dialogs.OpenDialogRequest
            {
                UserId = PS4Manager.Instance.InitialUserId,
                Mode = Dialogs.DialogMode.SystemMsg,
                DispType = type,

                // A system mesage has some specific param requests to fill
                SystemMessage = new Dialogs.SystemMessageParam
                {
                    SysMsgType = Dialogs.SystemMessageType.CorruptedAndDelete
                },

                Items = new Dialogs.Items
                {
                    // dirName is the directory name of the SaveData that failed to mount
                    DirNames = new DirName[] { dirName },
                    ItemStyle = Dialogs.ItemStyle.DateSizeSubtitle
                },

                Async = false
            };

            var response = new Dialogs.OpenDialogResponse();
            var requestId = Dialogs.OpenDialog(request, response);
        }

        private static ulong BlocksFromBytes(int dataSize)
        {
            ulong blocks = (ulong)Mathf.CeilToInt(dataSize / (float)Sony.PS4.SaveData.Mounting.MountRequest.BLOCK_SIZE);
            blocks = blocks < Mounting.MountRequest.BLOCKS_MIN ? Mounting.MountRequest.BLOCKS_MIN : blocks;
            return blocks;
        }

        public static void Log(string message)
        {
#if PETOONS_DEBUG
            string header = "[SAV_PS4]";
            string msg = $"{header}: {message}";

            var dialogRequest = new Dialogs.OpenDialogRequest();
            var dialogResponse = new Dialogs.OpenDialogResponse();
            dialogRequest.UserMessage = new Dialogs.UserMessageParam();
            dialogRequest.UserMessage.Message = msg;
            dialogRequest.UserMessage.MsgType = Dialogs.UserMessageType.Normal;

            Dialogs.OpenDialog(dialogRequest, dialogResponse);

            Debug.Log(msg);
#endif
        }
        #endregion
#endif
    }
}