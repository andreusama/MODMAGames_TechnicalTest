using PetoonsStudio.PSEngine.Framework;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using IStorage = PetoonsStudio.PSEngine.Framework.IStorage;

namespace PetoonsStudio.PSEngine.Multiplatform
{
    public class AndroidStorage : IStorage
    {
        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
            DeleteSaveAndroid(fileName, folderName);
            var result = await SaveExists(fileName, folderName);
            return result;
        }

        public async Task Initialize()
        {
            await Task.Yield();
        }

        public Task<T> Load<T>(string fileName, string folderName)
        {
            return Task.FromResult(LoadAndroid<T>(fileName, folderName));
        }

        public Task<bool> Save(object saveObject, string fileName, string folderName)
        {
            return Task.FromResult(SaveAndroid(saveObject, fileName, folderName));
        }

        public Task<bool> SaveExists(string fileName, string folderName)
        {
            return Task.FromResult(SaveExistsAndroid(fileName, folderName));
        }

        /// <summary>
        /// Delete save data
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        public static void DeleteSaveAndroid(string fileName, string folderName)
        {
            string savePath = DetermineSavePath(folderName);
            string saveFileName = fileName;
            File.Delete(savePath + saveFileName);
        }

        /// <summary>
        /// Check saved data android
        /// </summary>
        /// <param name="saveName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static bool SaveExistsAndroid(string saveName, string folderName)
        {
            string savePath = DetermineSavePath(folderName);

            if (File.Exists(savePath + saveName))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Save android
        /// </summary>
        public static bool SaveAndroid(object saveObject, string fileName, string folderName)
        {
            string savePath = DetermineSavePath(folderName);
            string saveFileName = fileName;

            // if the directory doesn't already exist, we create it
            if (!Directory.Exists(savePath))
            {
                Directory.CreateDirectory(savePath);
            }

            try
            {
                // we serialize and write our object into a file on disk
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream saveFile = File.Create(savePath + saveFileName);
                formatter.Serialize(saveFile, saveObject);
                saveFile.Close();
                return true;
            }
            catch
            {
                return false;
                throw new SaveLoadException(SaveLoadException.Type.IOException, $"Error deserializing file with name {saveFileName}");
            }
        }

        /// <summary>
        /// Load android
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="fileName"></param>
        /// <param name="folderName"></param>
        /// <returns></returns>
        public static T LoadAndroid<T>(string fileName, string folderName)
        {
            string savePath = DetermineSavePath(folderName);
            string saveFileName = savePath + fileName;

            T returnObject = default(T);

            // if the MMSaves directory or the save file doesn't exist, there's nothing to load, we do nothing and exit
            if (!Directory.Exists(savePath) || !File.Exists(saveFileName))
            {
                throw new SaveLoadException(SaveLoadException.Type.NotFound, $"File with name {saveFileName} not found");
            }
            try
            {
                BinaryFormatter formatter = new BinaryFormatter();
                FileStream saveFile = File.Open(saveFileName, FileMode.Open, FileAccess.Read, FileShare.Read);
                returnObject = (T)formatter.Deserialize(saveFile);
                saveFile.Close();
            }
            catch
            {
                throw new SaveLoadException(SaveLoadException.Type.IOException, $"Error deserializing file with name {saveFileName}");
            }

            return returnObject;
        }

        /// <summary>
        /// Determines the save path to use when loading and saving a file based on a folder name.
        /// </summary>
        /// <returns>The save path.</returns>
        /// <param name="folderName">Folder name.</param>
        protected static string DetermineSavePath(string folderName)
        {
            string savePath = string.Empty;
#if UNITY_EDITOR
            savePath = Application.dataPath;
#elif UNITY_ANDROID
            savePath = Application.persistentDataPath;
#endif
            savePath = savePath + folderName + "/";
            return savePath;
        }
    }
}
