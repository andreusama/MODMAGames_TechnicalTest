using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System.Threading.Tasks;
using UnityEngine;

#if MICROSOFT_GAME_CORE
using XGamingRuntime;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.WindowsStore
{
    public class WSStorage : IStorage
    {
        public async Task<bool> DeleteSave(string fileName, string folderName)
        {
#if MICROSOFT_GAME_CORE
            var hResult = HR.E_INVALIDARG;
            var finished = false;

            WSManager.Helpers.GameSaveHelper.Delete(fileName, fileName, (hr) =>
            {
                hResult = hr;
                finished = true;
            });

            await TaskUtils.WaitUntil(() => finished);

            if (HR.FAILED(hResult))
                Debug.LogError("There was an error deleting data!");

            return HR.SUCCEEDED(hResult);
#else
            return await Task.FromResult(true);            
#endif
        }

        public async Task<T> Load<T>(string fileName, string folderName)
        {
#if MICROSOFT_GAME_CORE
            T loadedData = default;
            var hResult = HR.E_INVALIDARG;
            var finished = false;

            WSManager.Helpers.GameSaveHelper.Load(fileName, fileName, (hr, data) =>
            {
                hResult = hr;
                loadedData = (T)WSManager.ByteArrayToObject(data);
                finished = true;
            });

            await TaskUtils.WaitUntil(() => finished);

            if (HR.SUCCEEDED(hResult))
                return loadedData;
            else
            {
                Debug.LogError("There was an error loading data!");
                return default;
            }
#else
            return await Task.FromResult((T)default);
#endif
        }

        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
#if MICROSOFT_GAME_CORE
            var hResult = HR.E_INVALIDARG;
            var finished = false;

            WSManager.Helpers.GameSaveHelper.Save(fileName, fileName, WSManager.ObjectToByteArray(saveObject), (hr) =>
            {
                hResult = hr;
                finished = true;
            });

            await TaskUtils.WaitUntil(() => finished);

            if (HR.FAILED(hResult))
                Debug.LogError("There was an error saving the data!");

            return HR.SUCCEEDED(hResult);
#else
            return await Task.FromResult(false);
#endif
        }

        public async Task<bool> SaveExists(string fileName, string folderName)
        {
            var fileExists = false;
#if MICROSOFT_GAME_CORE
            var hResult = HR.E_INVALIDARG;
            var finished = false;

            WSManager.Helpers.GameSaveHelper.QueryContainerBlobs(fileName, (hr, blobs) =>
            {
                hResult = hr;
                fileExists = blobs.ContainsKey(fileName);
                finished = true;
            });

            await TaskUtils.WaitUntil(() => finished);

            if (!HR.SUCCEEDED(hResult))
                Debug.LogError("There was an error getting the data!");
#endif
            return await Task.FromResult(fileExists);
        }
    }

}
