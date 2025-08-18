using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Framework;


#if UNITY_PS5
using UnityEngine.PS5;
using Unity.SaveData.PS5.Core;
using Unity.SaveData.PS5.Dialog;
using Unity.SaveData.PS5.Delete;
using Mounting = Unity.SaveData.PS5.Mount.Mounting;
using ReturnCodes = Unity.SaveData.PS5.Core.ReturnCodes;
using FileOps = Unity.SaveData.PS5.Info.FileOps;
using EmptyResponse = Unity.SaveData.PS5.Core.EmptyResponse;
using Searching = Unity.SaveData.PS5.Search.Searching;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.PS5
{
#if UNITY_PS5
    public class PS5Storage : IStorage
    {
        //TODO: WARNING. Right now the fileName is used as folderName, because internally you can't check the existence of a file inside a mount without mounting it.
        private static bool m_Busy = false;

        /// <summary>
        /// On PS5 Internally is used the filename as the name of the mountPoint.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<bool> SaveExists(string fileName, string folderName)
        {
            PS5Manager.Log("SaveExists Called");
            return (await SearchForFile(fileName)).Length > 0;
        }
        /// <summary>
        /// On PS5 Internally is used the filename as the name of the mountPoint.
        /// </summary>
        /// <param name="saveObject"></param>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
            PS5Manager.Log($"SAVE DATA CALLED: {folderName}/{fileName}");
            return await WriteFiles(saveObject, folderName, fileName);
        }
        /// <summary>
        /// On PS5 Internally is used the filename as the name of the mountPoint.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<T> Load<T>(string fileName, string folderName)
        {
            PS5Manager.Log($"LOAD DATA CALLED: {folderName}/{fileName}");
            return await ReadFiles<T>(folderName, fileName);
        }
        /// <summary>
        /// On PS5 Internally is used the filename as the name of the mountPoint.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
            PS5Manager.Log($"DELETE DIRECTORY DATA CALLED: {folderName}");
            return await DeleteSaveData(folderName);
        }

    #region Generic Methods
        /// <summary>
        /// Reads the data on the directory
        /// </summary>
        /// <param name="dirName"></param>
        /// <returns></returns>
        public static async Task<T> ReadFiles<T>(string dirName, string fileName)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);
            m_Busy = true;

            Mounting.MountResponse mountResponse = InternalMountSave(fileName, false);
            await TaskUtils.WaitUntil(() => mountResponse.Locked == false);
            PS5Manager.Log("Mounted Save " + fileName + " to " + mountResponse.MountPoint.PathName);

            Mounting.MountPoint mountPoint = mountResponse.MountPoint;
            ReadFilesResponse readResponse = InternalReadFiles(mountPoint, fileName);
            await TaskUtils.WaitUntil(() => readResponse.Locked == false);

            if (mountResponse.IsErrorCode)
            {
                await HandleLoadErrors(mountResponse.ReturnCode, dirName, fileName);
                m_Busy = false;
                return default(T);
            }

            PS5Manager.Log($"LOADED {dirName}/{fileName} with {readResponse.fileData.Length} lenght");
            object data = default(T);
            using (var ms = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                ms.Write(readResponse.fileData, 0, readResponse.fileData.Length);
                ms.Seek(0, SeekOrigin.Begin);
                data = binForm.Deserialize(ms);
            }

            EmptyResponse unmountReponse = InternalUnmountSave(mountPoint);
            await TaskUtils.WaitUntil(() => unmountReponse.Locked == false);
            PS5Manager.Log("Unmounted Save " + fileName + " from " + mountPoint.PathName);

            PS5Manager.Log("Files read");
            m_Busy = false;
            return data != null ? (T)data : default(T);
        }
        public static async Task<bool> WriteFiles(object objectData, string dirName, string fileName, bool restartCounter = true)
        {
            Byte[] byteData;
            ulong newSaveDataBlocks = Mounting.MountRequest.BLOCKS_MIN;
            ulong rawBytes = 0;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, objectData);
                rawBytes = (ulong)ms.Length;
                byteData = ms.ToArray();
            }

            newSaveDataBlocks = (UInt64)Mathf.CeilToInt(rawBytes / (float)Mounting.MountRequest.BLOCK_SIZE);
            PS5Manager.Log($"Needed space: {newSaveDataBlocks} blocks(?");

            return await WriteFiles(byteData, dirName, fileName);
        }
        /// <summary>
        /// Write the data on the specified directory
        /// </summary>
        /// <param name="dirName"></param>
        /// <param name="byteData"></param>
        /// <returns></returns>
        public static async Task<bool> WriteFiles(byte[] byteData, string dirName, string fileName, ulong blockSize = Mounting.MountRequest.BLOCKS_MIN)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);
            m_Busy = true;

            Mounting.MountResponse mountResponse = InternalMountSave(fileName, true, blockSize);
            await TaskUtils.WaitUntil(() => mountResponse.Locked == false);
            PS5Manager.Log("Mounted Save " + dirName + " to " + mountResponse.MountPoint.PathName);

            Mounting.MountPoint mountPoint = mountResponse.MountPoint;
            if (mountResponse.IsErrorCode)
            {
                await HandleSaveErrors(mountResponse.ReturnCode, mountResponse.RequiredBlocks, dirName, fileName, byteData);
                m_Busy = false;
                return false;
            }


            WriteFilesResponse writeResponse = InternalWriteFiles(byteData, mountPoint, fileName);
            await TaskUtils.WaitUntil(() => writeResponse.Locked == false);
            PS5Manager.Log("Files Written to Save " + dirName + " : Size written = " + writeResponse.totalFileSizeWritten);

            EmptyResponse unmountReponse = InternalUnmountSave(mountPoint);
            await TaskUtils.WaitUntil(() => unmountReponse.Locked == false);

            PS5Manager.Log("Unmounted Save " + dirName + " from " + mountPoint.PathName);

            PS5Manager.Log("Files written");
            m_Busy = false;
            return true;
        }
        public static async Task<Searching.SearchSaveDataItem[]> SearchForFile(string filename)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);
            m_Busy = true;

            var searchResponse = InternalSearch(filename);

            await TaskUtils.WaitUntil(() => searchResponse.Locked == false);

            PS5Manager.Log($"Founded {searchResponse.SaveDataItems.Length} files for {filename} with error {searchResponse.ReturnCode}");
            m_Busy = false;
            return searchResponse.SaveDataItems;
        }
        public static async Task<bool> SearchForFile(string dirName, string fileName)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);
            m_Busy = true;

            var searchResponse = InternalSearch(dirName);

            await TaskUtils.WaitUntil(() => searchResponse.Locked == false);

            PS5Manager.Log($"Founded {searchResponse.SaveDataItems.Length} files for {dirName} with error {searchResponse.ReturnCode}");

            Mounting.MountResponse mountResponse = null;
            EnumerateFilesResponse enumerateResponse = null;
            Mounting.MountPoint mp = null;
            string[] currentFiles = null;

            foreach (var item in searchResponse.SaveDataItems)
            {
                mountResponse = InternalMountSave(dirName, false);
                await TaskUtils.WaitUntil(() => mountResponse.Locked == false);

                if (mountResponse.IsErrorCode) continue;
                mp = mountResponse.MountPoint;

                enumerateResponse = InternalEnumerateFiles(mp);
                await TaskUtils.WaitUntil(() => enumerateResponse.Locked == false);

                if (enumerateResponse.IsErrorCode) continue;
                currentFiles = enumerateResponse.files;

                if (!string.IsNullOrEmpty(Array.Find(currentFiles, x => x == fileName)))
                {
                    m_Busy = false;
                    return true;
                }
            }

            m_Busy = false;
            return false;
        }
        public static async Task<bool> DeleteSaveData(string dirName)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);
            m_Busy = true;

            for (int i = 0; i < dirName.Length; i++)
            {
                EmptyResponse deleteResponse = InternalDeleteFiles(dirName);
                await TaskUtils.WaitUntil(() => deleteResponse.Locked == false);

                PS5Manager.Log("Deleted Save " + dirName[i]);

            }

            PS5Manager.Log("Files Deleted");
            m_Busy = false;
            return true;
        }
        public static async Task<string[]> EnumerateFiles(Mounting.MountPoint mp)
        {
            await TaskUtils.WaitUntil(() => !m_Busy);

            EnumerateFilesResponse enumerateResponse = InternalEnumerateFiles(mp);
            await TaskUtils.WaitUntil(() => enumerateResponse.Locked == false);

            if (enumerateResponse.IsErrorCode)
            {
                PS5Manager.Log($"Enumerate files for {mp.DirName} failed with error {enumerateResponse.ReturnCode}");
                return null;
            }

            EmptyResponse unmountReponse = InternalUnmountSave(mp);
            await TaskUtils.WaitUntil(() => unmountReponse.Locked == false);
            Debug.Log("Unmounted Save " + mp.DirName + " from " + mp.PathName);

            if (unmountReponse.IsErrorCode)
            {
                PS5Manager.Log($"Unmount on EnumerateFiles files for {mp.DirName} failed with error {unmountReponse.ReturnCode}");
            }

            return enumerateResponse.files;
        }
    #endregion

    #region InternalMethods
        internal static EmptyResponse InternalDeleteFiles(string _dirName)
        {
            PS5Manager.Log($"Starting Delete file request for {_dirName}");

            EmptyResponse response = new EmptyResponse();

            try
            {
                //Should be modified if multisave
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                Deleting.DeleteRequest request = new Deleting.DeleteRequest();

                DirName dirName = new DirName();
                dirName.Data = _dirName;

                request.UserId = user.userId;
                request.DirName = dirName;

                int requestId = Deleting.Delete(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed for delete file request on: {_dirName}");
                PS5Manager.Log(e.Message);
            }
            PS5Manager.Log($"Stopping Delete file request for {_dirName}");

            return response;
        }
        internal static WriteFilesResponse InternalWriteFiles(byte[] data, Mounting.MountPoint mp, string fileName)
        {
            PS5Manager.Log($"Starting Write file request for file: {fileName} in directory: {mp.DirName}");

            WriteFilesResponse response = new WriteFilesResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                WriteFilesRequest request = new WriteFilesRequest();

                request.UserId = user.userId;
                request.MountPointName = mp.PathName;
                request.fileData = data;
                request.fileName = fileName;

                int requestId = FileOps.CustomFileOp(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed for WriteFilesRequest on file: {fileName} in directory: {mp.DirName}");
                PS5Manager.Log(e.Message);
            }

            PS5Manager.Log($"Stopping Write file request for file: {fileName} in directory: {mp.DirName}");
            return response;
        }
        internal static EnumerateFilesResponse InternalEnumerateFiles(Mounting.MountPoint mp)
        {
            EnumerateFilesResponse response = new EnumerateFilesResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                EnumerateFilesRequest request = new EnumerateFilesRequest();

                request.UserId = user.userId;
                request.MountPointName = mp.PathName;

                int requestId = FileOps.CustomFileOp(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log("Aysnc call failed");
                PS5Manager.Log(e.Message);
            }

            return response;
        }
        internal static ReadFilesResponse InternalReadFiles(Mounting.MountPoint mp, string fileName)
        {
            PS5Manager.Log($"Starting Read file request for file: {fileName} in directory: {mp.DirName}");
            ReadFilesResponse response = new ReadFilesResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                ReadFilesRequest request = new ReadFilesRequest();

                request.UserId = user.userId;
                request.MountPointName = mp.PathName;
                request.fileName = fileName;

                int requestId = FileOps.CustomFileOp(request, response);

            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed for ReadFilesRequest on {mp.DirName}");
                PS5Manager.Log(e.Message);
            }

            PS5Manager.Log($"Stopping Read file request for file: {fileName} in directory: {mp.DirName}");
            return response;
        }
        internal static Searching.DirNameSearchResponse InternalSearch(string fileName)
        {
            PS5Manager.Log($"Starting Internal search for {fileName}");
            Searching.DirNameSearchResponse response = new Searching.DirNameSearchResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                Searching.DirNameSearchRequest request = new Searching.DirNameSearchRequest();

                request.UserId = user.userId;

                if (fileName != null)
                {
                    DirName _dirName = new DirName();
                    _dirName.Data = fileName;
                    request.DirName = _dirName;
                }

                request.Key = Searching.SearchSortKey.DirName;
                request.Order = Searching.SearchSortOrder.Ascending;
                request.IncludeBlockInfo = false;
                request.IncludeParams = false;
                request.MaxDirNameCount = Searching.DirNameSearchRequest.DIR_NAME_MAXSIZE;

                int requestId = Searching.DirNameSearch(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed on InternalSearch for {fileName}");
                PS5Manager.Log(e.Message);
            }

            PS5Manager.Log($"Stopping Internal search for {fileName}");
            return response;
        }
        internal static EnumerateFilesResponse InternalSearchFile(Mounting.MountPoint mp, string dirName)
        {
            EnumerateFilesResponse response = new EnumerateFilesResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                EnumerateFilesRequest request = new EnumerateFilesRequest();

                request.UserId = user.userId;
                request.MountPointName = mp.PathName;

                int requestId = FileOps.CustomFileOp(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log(e.Message);
            }

            return response;
        }
        internal static EmptyResponse InternalUnmountSave(Mounting.MountPoint mp)
        {
            PS5Manager.Log($"Starting Unmount save request for directory: {mp.DirName}");

            EmptyResponse response = new EmptyResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                Mounting.UnmountRequest request = new Mounting.UnmountRequest();

                request.UserId = user.userId;
                request.MountPointName = mp.PathName;

                int requestId = Mounting.Unmount(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed for UnmounSaveRequest for: {mp.DirName}");
                PS5Manager.Log(e.Message);
            }
            PS5Manager.Log($"Stopping Unmount save request for directory: {mp.DirName}");
            return response;
        }
        internal static Mounting.MountResponse InternalMountSave(string dirName, bool readWrite, ulong blockSize = Mounting.MountRequest.BLOCKS_MIN)
        {
            PS5Manager.Log($"Starting Mount save request for directory: {dirName}");

            Mounting.MountResponse response = new Mounting.MountResponse();

            try
            {
                PS5Input.LoggedInUser user = PS5Input.RefreshUsersDetails(0);

                Mounting.MountRequest request = new Mounting.MountRequest();

                DirName _dirName = new DirName();
                _dirName.Data = dirName;

                request.UserId = user.userId;
                request.Async = true;
                request.DirName = _dirName;

                if (readWrite == true)
                {
                    request.MountMode = Mounting.MountModeFlags.Create2 | Mounting.MountModeFlags.ReadWrite;
                }
                else
                {
                    request.MountMode = Mounting.MountModeFlags.ReadOnly;
                }

                request.Blocks = blockSize;

                int requestId = Mounting.Mount(request, response);
            }
            catch (SaveDataException e)
            {
                PS5Manager.Log($"Aysnc call failed for MountSaveRequest for {dirName}");
                PS5Manager.Log(e.Message);
            }

            PS5Manager.Log($"Stopping Mount save request for directory: {dirName}");
            return response;
        }
    #endregion

    #region InternalErrors
        internal static async Task HandleSaveErrors(ReturnCodes errorCode, ulong requiredBlocks, string dirName, string fileName, Byte[] byteData)
        {
            bool waitForDialog = false;

            Dialogs.OpenDialogResponse dialogResponse = new Dialogs.OpenDialogResponse();
            Dialogs.OpenDialogRequest dialogRequest = new Dialogs.OpenDialogRequest();
            try
            {
                PS5Manager.Log("HandleSaveErrors called");

                dialogRequest.Async = false;
                dialogRequest.Mode = Dialogs.DialogMode.SystemMsg;
                dialogRequest.DispType = Dialogs.DialogType.Save;
                dialogRequest.SystemMessage = new Dialogs.SystemMessageParam();
                dialogRequest.SystemMessage.Value = requiredBlocks;
                dialogRequest.UserId = PS5Input.RefreshUsersDetails(0).userId;

                PS5Manager.Log($"ErrorCode is {errorCode.ToString()}");

                switch (errorCode)
                {
                    case ReturnCodes.DATA_ERROR_NO_SPACE_FS:
                        PS5Manager.Log($"Opening dialog for no space");
                        dialogRequest.SystemMessage.SysMsgType = Dialogs.SystemMessageType.NoSpaceContinuable;
                        waitForDialog = true;
                        break;

                    case ReturnCodes.SAVE_DATA_ERROR_BROKEN:
                        PS5Manager.Log($"Opening dialog for corrupted data");
                        dialogRequest.SystemMessage.SysMsgType = Dialogs.SystemMessageType.Corrupted;
                        waitForDialog = true;
                        break;
                    default:
                        PS5Manager.Log($"Throwed NON CONTROLLED error code: {errorCode.ToString()}");
                        break;
                }
                Dialogs.OpenDialog(dialogRequest, dialogResponse);
            }
            catch (SaveDataException se)
            {
                PS5Manager.Log($"Exception Opening the dialog: {se.ExtendedMessage}");
            }
            catch (Exception e)
            {
                PS5Manager.Log(e.Message);
            }

            if (waitForDialog)
            {
                await TaskUtils.WaitUntil(() => dialogResponse.Locked == false);

                if (dialogResponse.IsErrorCode)
                {
                    PS5Manager.Log(dialogResponse.Exception.Message);
                }
            }

            switch (errorCode)
            {
                case ReturnCodes.DATA_ERROR_NO_SPACE_FS:
                    break;
                case ReturnCodes.SAVE_DATA_ERROR_BROKEN:
                    PS5Manager.Log($"Deleting corrupted files");
                    await DeleteSaveData(dirName);
                    break;
                default:
                    PS5Manager.Log($"Throwed NON CONTROLLED error code: {errorCode.ToString()}");
                    break;
            }
            PS5Manager.Log($"HandleSaveError stopped");

        }
        internal static async Task HandleLoadErrors(ReturnCodes errorCode, string dirName, string fileName)
        {
            PS5Manager.Log("HandleLoadErrors called");
            bool waitForDialog = false;
            Dialogs.OpenDialogRequest dialogRequest = new Dialogs.OpenDialogRequest();
            Dialogs.OpenDialogResponse dialogResponse = new Dialogs.OpenDialogResponse();

            try
            {
                dialogRequest.Async = false;
                dialogRequest.Mode = Dialogs.DialogMode.SystemMsg;
                dialogRequest.DispType = Dialogs.DialogType.Load;
                dialogRequest.UserId = PS5Input.RefreshUsersDetails(0).userId;
                PS5Manager.Log($"ErrorCode is {errorCode.ToString()}");

                switch (errorCode)
                {
                    case ReturnCodes.SAVE_DATA_ERROR_BROKEN://Now NP should handle this alone
                        PS5Manager.Log($"Opening dialog for corrupted data");
                        dialogRequest.SystemMessage.SysMsgType = Dialogs.SystemMessageType.Corrupted;
                        waitForDialog = true;
                        break;
                    case ReturnCodes.SAVE_DATA_ERROR_NOT_FOUND:
                        PS5Manager.Log($"SaveDataNotFound");
                        break;
                    default:
                        PS5Manager.Log($"Throwed NON CONTROLLED error code: {errorCode.ToString()}");
                        break;
                }
                Dialogs.OpenDialog(dialogRequest, dialogResponse);
            }
            catch (SaveDataException se)
            {
                PS5Manager.Log($"Exception Opening the dialog: {se.ExtendedMessage}");
            }
            catch (Exception e)
            {
                PS5Manager.Log(e.Message);
            }

            if (waitForDialog)
            {
                await TaskUtils.WaitUntil(() => dialogResponse.Locked == false);

                if (dialogResponse.IsErrorCode)
                {
                    PS5Manager.Log(dialogResponse.Exception.Message);
                }
            }

            switch (errorCode)
            {
                case ReturnCodes.SAVE_DATA_ERROR_BROKEN://Now NP should handle this alone
                    PS5Manager.Log($"Deleting corrupted files");
                    await DeleteSaveData(dirName);
                    break;
                default:
                    PS5Manager.Log($"Throwed NON CONTROLLED error code: {errorCode.ToString()}");
                    break;
            }
        }
    #endregion

    #region Requests
        public class WriteFilesRequest : FileOps.FileOperationRequest
        {
            public string fileName;
            public byte[] fileData;

            public override void DoFileOperations(Mounting.MountPoint mp, FileOps.FileOperationResponse response)
            {
                WriteFilesResponse fileResponse = response as WriteFilesResponse;

                //WRITING DATA
                string outpath = mp.PathName.Data + $"/{fileName}";
                int totalWritten = 0;

                Debug.Log($"[SAV_PS5]File at path: {outpath} will write a total of {fileData.Length} bytes");

                File.WriteAllBytes(outpath, fileData);

                FileInfo info = new FileInfo(outpath);
                fileResponse.lastWriteTime = info.LastWriteTime;
                fileResponse.totalFileSizeWritten += info.Length;
            }
        }
        public class WriteFilesResponse : FileOps.FileOperationResponse
        {
            public DateTime lastWriteTime;
            public long totalFileSizeWritten;
        }
        public class SearchFileRequest : FileOps.FileOperationRequest
        {
            public string fileName = string.Empty;

            public override void DoFileOperations(Mounting.MountPoint mp, FileOps.FileOperationResponse response)
            {
                EnumerateFilesResponse fileResponse = response as EnumerateFilesResponse;

                string outpath = mp.PathName.Data;

                string[] textFiles = Directory.GetFiles(outpath, "*.txt", SearchOption.AllDirectories);
                string[] dataFiles = Directory.GetFiles(outpath, "*.dat", SearchOption.AllDirectories);

                List<string> allFiles = new List<string>();

                allFiles.InsertRange(0, textFiles);
                allFiles.InsertRange(0, dataFiles);

                fileResponse.files = allFiles.ToArray();
            }
        }
        public class SearchFilesResponse : FileOps.FileOperationResponse
        {
            public string[] files;
        }
        public class EnumerateFilesRequest : FileOps.FileOperationRequest
        {
            public override void DoFileOperations(Mounting.MountPoint mp, FileOps.FileOperationResponse response)
            {
                EnumerateFilesResponse fileResponse = response as EnumerateFilesResponse;

                string outpath = mp.PathName.Data;

                string[] allFiles = Directory.GetFiles(outpath);

                fileResponse.files = allFiles;
            }
        }
        public class EnumerateFilesResponse : FileOps.FileOperationResponse
        {
            public string[] files;
        }
        public class ReadFilesRequest : FileOps.FileOperationRequest
        {
            public string fileName;

            public override void DoFileOperations(Mounting.MountPoint mp, FileOps.FileOperationResponse response)
            {
                ReadFilesResponse fileResponse = response as ReadFilesResponse;

                //GAME
                string outpath = mp.PathName.Data + $"/{fileName}";

                fileResponse.fileData = File.ReadAllBytes(outpath);

                FileInfo info = new FileInfo(outpath);

                Debug.Log($"[SAV_PS5]File at path: {outpath} will READ a total of {info.Length} bytes");
            }
        }
        public class ReadFilesResponse : FileOps.FileOperationResponse
        {
            public byte[] fileData;
        }
    #endregion
    }
#endif
}

