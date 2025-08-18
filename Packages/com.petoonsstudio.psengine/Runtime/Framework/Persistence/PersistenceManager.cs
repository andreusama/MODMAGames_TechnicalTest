using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using PetoonsStudio.PSEngine.Multiplatform;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

#if UNITY_EDITOR
using UnityEditor;
#endif
#if STANDALONE_STEAM
using Steamworks;
#endif

namespace PetoonsStudio.PSEngine.Framework
{
    public class SaveLoadException : Exception
    {
        public enum Type { NotFound, CorruptedData, NoDiskSpace, IOException }
        public Type ExceptionType;

        public SaveLoadException(Type exceptionType, string message) : base(message)
        {
            ExceptionType = exceptionType;
        }
    }

    public struct SaveStateEvent
    {
        public enum OperationType { Save, Load }
        public enum Type { Start, End }

        public OperationType Operation;
        public Type PhaseType;

        public SaveStateEvent(OperationType operation, Type saveType)
        {
            Operation = operation;
            PhaseType = saveType;
        }
    }

    /// <summary>
    /// @Author: Alejandro Cortes Cabrejas
    /// Handles persistence data
    /// </summary>
    public class PersistenceManager : PersistentSingleton<PersistenceManager>
    {
        [SerializeField] private bool m_ShowIcon;
        [SerializeField] private AssetReferenceGameObject m_IconCanvas;
        [SerializeField] private float m_MinOperationTime = 0f;

        private bool m_IsWorking;
        private AsyncOperationHandle<GameObject> m_CanvasOperation;

        public bool Working => m_IsWorking;

        void OnEnable()
        {
            if (_instance != this)
                return;

            m_CanvasOperation = Addressables.InstantiateAsync(m_IconCanvas, transform);
        }

        void OnDisable()
        {
            if (_instance != this)
                return;

            Addressables.ReleaseInstance(m_CanvasOperation);
        }

        /// <summary>
        /// There is a previous saved data
        /// </summary>
        /// <returns></returns>
        public async Task<bool> SaveExists(string saveName, string folderName)
        {
            m_IsWorking = true;

            try
            {
                return await PlatformManager.Instance.Storage.SaveExists(saveName, folderName);
            }
            catch (Exception e)
            {
                return false;
                throw e;
            }
            finally
            {
                m_IsWorking = false;
            }
        }

        /// <summary>
        /// Save the specified saveObject, fileName and folderName into a file on disk.
        /// </summary>
        /// <param name="saveObject">Save object.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Foldername.</param>
        public async Task<bool> Save(object saveObject, string fileName, string folderName)
        {
            m_IsWorking = true;

            float startTime = Time.time;

            try
            {
                PSEventManager.TriggerEvent(new SaveStateEvent(SaveStateEvent.OperationType.Save, SaveStateEvent.Type.Start));
                return await PlatformManager.Instance.Storage.Save(saveObject, fileName, folderName);
            }
            catch
            {
                return false;
            }
            finally
            {
                float currentTime = Time.time - startTime;

                if (currentTime < m_MinOperationTime)
                {
                    int awaitTime = (int)((m_MinOperationTime - currentTime) * 1000.0f);
                    await Task.Delay(awaitTime);
                }

                PSEventManager.TriggerEvent(new SaveStateEvent(SaveStateEvent.OperationType.Save, SaveStateEvent.Type.End));

                m_IsWorking = false;
            }
        }

        /// <summary>
        /// Load the specified file based on a file name into a specified folder
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Foldername.</param>
        public async Task<T> Load<T>(string fileName, string folderName)
        {
            m_IsWorking = true;

            T returnObject = default(T);

            float startTime = Time.time;

            try
            {
                PSEventManager.TriggerEvent(new SaveStateEvent(SaveStateEvent.OperationType.Load, SaveStateEvent.Type.Start));
                return await PlatformManager.Instance.Storage.Load<T>(fileName, folderName);
            }
            catch
            {
                return returnObject;
            }
            finally
            {
                float currentTime = Time.time - startTime;

                if (currentTime < m_MinOperationTime)
                {
                    int awaitTime = (int)((m_MinOperationTime - currentTime) * 1000.0f);
                    await Task.Delay(awaitTime);
                }

                PSEventManager.TriggerEvent(new SaveStateEvent(SaveStateEvent.OperationType.Load, SaveStateEvent.Type.End));

                m_IsWorking = false;
            }
        }

        /// <summary>
        /// Removes a save from disk
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderName">Folder name.</param>
        public async Task DeleteSave(string fileName, string folderName)
        {
            m_IsWorking = true;

            try
            {
                await PlatformManager.Instance.Storage.DeleteSave(fileName, folderName);
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                m_IsWorking = false;
            }
        }
    }
}
