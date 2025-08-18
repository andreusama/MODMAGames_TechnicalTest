using UnityEngine;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.Threading.Tasks;
using System.Linq;
using PetoonsStudio.PSEngine.Framework;

#if UNITY_SWITCH
using UnityEngine.Switch;
using nn.fs;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.Switch
{
    public class SwitchFileStorage : IStorage
    {
#if UNITY_SWITCH
        private static FileHandle m_FileHandle = new FileHandle();
#endif
        //TODO Porting: This number what is it and should be customizable?
        private const int JOURNAL_SAVE_DATA_SIZE = 32768;

        public async Task<bool> SaveExists(string fileName, string folderName)
        {
#if UNITY_SWITCH
            EntryType type = 0;
            FileSystem.GetEntryType(ref type, DetermineSavePath());
            nn.Result result = nn.fs.File.Open(ref m_FileHandle, DetermineSavePath() + fileName, OpenFileMode.Read);

            if (!result.IsSuccess())
            {
                return await Task.FromResult(false);
            }

            nn.fs.File.Close(m_FileHandle);

            return await Task.FromResult(true);
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
#if UNITY_SWITCH
            string filePath = DetermineSavePath() + fileName;

            Notification.EnterExitRequestHandlingSection();

            BinaryFormatter bf = new BinaryFormatter();
            using (var ms = new MemoryStream())
            {
                try
                {
                    ms.Seek(0, SeekOrigin.Begin);
                    bf.Serialize(ms, saveObject);
                    var bytes = ms.ToArray();
                    nn.fs.File.Delete(filePath);
                    nn.fs.File.Create(filePath, JOURNAL_SAVE_DATA_SIZE); //this makes a file the size of your save journal. You may want to make a file smaller than this.
                    nn.fs.File.Open(ref m_FileHandle, filePath, OpenFileMode.Write | OpenFileMode.AllowAppend);
                    nn.fs.File.Write(m_FileHandle, 0, bytes, bytes.LongLength, WriteOption.None); // Writes and flushes the write at the same time
                    nn.fs.File.Flush(m_FileHandle);
                    nn.fs.File.Close(m_FileHandle);
                    FileSystem.Commit(GetMountName()); //you must commit the changes.
                }
                catch (Exception e)
                {
                    Debug.LogError($"Error serializing file with name {fileName}. {e.Message}");
                    return false;
                }
                finally
                {
                    ms.Flush();
                    ms.Close();
                }
            }

            Notification.LeaveExitRequestHandlingSection();

            return await Task.FromResult(true);
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<T> Load<T>(string fileName, string folderName)
        {
#if UNITY_SWITCH
            EntryType entryType = 0; //init to a dummy value (C# requirement)
            FileSystem.GetEntryType(ref entryType, DetermineSavePath());
            nn.Result result = nn.fs.File.Open(ref m_FileHandle, DetermineSavePath() + fileName, OpenFileMode.Read | nn.fs.OpenFileMode.AllowAppend);

            if (result.IsSuccess() == false)
            {
                return default;   // Could not open file. This can be used to detect if this is the first time a user has launched your game. 
                                  // (However, be sure you are not getting this error due to your file being locked by another process, etc.)
            }

            long m_FileSize = 0;
            long m_Offset = 0;
            nn.fs.File.GetSize(ref m_FileSize, m_FileHandle);
            byte[] loadedData = null;

            do
            {
                long buffer = 0;
                if (m_FileSize - m_Offset >= JOURNAL_SAVE_DATA_SIZE) buffer = JOURNAL_SAVE_DATA_SIZE;
                else buffer = m_FileSize - m_Offset;
                byte[] currentLoadedData = new byte[buffer];

                nn.fs.File.Read(m_FileHandle, m_Offset, currentLoadedData, buffer);

                if (loadedData == null) loadedData = currentLoadedData;
                else loadedData = loadedData.Concat(currentLoadedData).ToArray();

                m_Offset += JOURNAL_SAVE_DATA_SIZE;
            }
            while (m_Offset < m_FileSize);

            nn.fs.File.Close(m_FileHandle);

            if (loadedData == null)
            {
                throw new Exception("No bytes loaded");
            }
            else
            {
                using (var memStream = new MemoryStream())
                {
                    var binForm = new BinaryFormatter();
                    memStream.Position = 0;
                    memStream.Write(loadedData, 0, loadedData.Length);
                    memStream.Seek(0, SeekOrigin.Begin);

                    return await Task.FromResult((T)binForm.Deserialize(memStream));
                }
            }
#else
            return await Task.FromResult((T)default);
#endif
        }

        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
#if UNITY_SWITCH
            string savePath = DetermineSavePath() + fileName;
            var result = nn.fs.File.Delete(savePath);
            FileSystem.Commit(GetMountName());

            return await Task.FromResult(result.IsSuccess() ? true : false);
#else
            return await Task.FromResult(true);
#endif
        }
#if UNITY_SWITCH
        private string DetermineSavePath()
        {
            return GetMountName() + ":/";
        }

        public string GetMountName()
        {
            return SwitchManager.Instance.GetMountName;
        }
#endif
    }
}
