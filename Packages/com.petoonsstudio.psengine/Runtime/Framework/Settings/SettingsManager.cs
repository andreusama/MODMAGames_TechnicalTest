using MoreMountains.Tools;
using Newtonsoft.Json;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization.Settings;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Framework
{
    public struct SettingsManagerInitializationDoneEvent
    {

    }

    /// <summary>
    /// @Author: Alejandro Cortes Cabrejas
    /// </summary>
    public abstract class SettingsManager : PersistentSingleton<SettingsManager>
    {
        [System.Serializable]
        public struct DefaultSettingsByPlatform
        {
            public Platform Platform;
            public AssetReferenceT<SettingsSO> Settings;
        }

        [SerializeField] protected DefaultSettingsByPlatform[] m_PlatformSettings;

        protected SettingsSO m_SettingsSO;
        protected AsyncOperationHandle<SettingsSO> m_AsyncOperation;

        public SettingsSO SettingsSO => m_SettingsSO;

        protected bool m_InitializationDone;
        protected bool m_ShouldCheckQuality = true;

        protected abstract void Start();

        public static bool IsReady => InstanceExists && Instance.m_InitializationDone;
        public static bool ShouldCheckQuality => InstanceExists && Instance.m_ShouldCheckQuality;

        void OnDestroy()
        {
            if (m_AsyncOperation.IsValid())
                Addressables.Release(m_AsyncOperation);
        }

        protected void Initialize<T>() where T : SerializableSettings
        {
            StartCoroutine(Initialize_Internal<T>());
        }

        protected virtual IEnumerator Initialize_Internal<T>() where T : SerializableSettings
        {
            yield return new WaitUntil(() => PlatformManager.Initialized);
            yield return new WaitUntil(() => LocalizationSettings.InitializationOperation.IsDone);

            var platformSettings = GetDefaultSettings();

            m_AsyncOperation = Addressables.LoadAssetAsync<SettingsSO>(platformSettings.Settings);
            yield return m_AsyncOperation;

            m_SettingsSO = Instantiate(m_AsyncOperation.Result);
            m_SettingsSO.ApplyDefaults();

            LoadSettings<T>();
        }

        public DefaultSettingsByPlatform GetDefaultSettings()
        {
            if (m_PlatformSettings.Any(x => x.Platform.HasFlag(PlatformManager.Instance.CurrentPlatform)))
            {
                return m_PlatformSettings.FirstOrDefault(x => x.Platform.HasFlag(PlatformManager.Instance.CurrentPlatform));
            }
            else
            {
                Debug.LogWarning($"No settings found for {PlatformManager.Instance.CurrentPlatform}, returning standalone settings");
                return m_PlatformSettings.FirstOrDefault(x => x.Platform.HasFlag(Platform.Standalone));
            }
        }

        public virtual async void LoadSettings<T>() where T : SerializableSettings
        {
            if (!await PersistenceManager.Instance.SaveExists(m_SettingsSO.SaveFileName + m_SettingsSO.SaveFileExtension, m_SettingsSO.SaveFolderName))
            {
                m_InitializationDone = true;
                PSEventManager.TriggerEvent(new SettingsManagerInitializationDoneEvent());
            }
            else
            {
                try
                {
                    var settingsJson = await PersistenceManager.Instance.Load<string>(m_SettingsSO.SaveFileName + m_SettingsSO.SaveFileExtension, m_SettingsSO.SaveFolderName);

                    if (!string.IsNullOrEmpty(settingsJson))
                    {
                        var settings = new JsonSerializerSettings
                        {
                            TypeNameHandling = TypeNameHandling.Auto
                        };

                        T savedSettings = JsonConvert.DeserializeObject<T>(settingsJson, settings);

                        Deserialize(savedSettings);
                        m_ShouldCheckQuality = false;
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError(ex.Message);
                }
                finally
                {
                    m_InitializationDone = true;
                    PSEventManager.TriggerEvent(new SettingsManagerInitializationDoneEvent());
                }
            }
        }

        [ContextMenu("Save settings")]
        public virtual Task SaveCurrentSettings<T>() where T : SerializableSettings
        {
            var SerializerSettings = new JsonSerializerSettings
            {
                TypeNameHandling = TypeNameHandling.Auto
            };

            T savedSettings = Activator.CreateInstance<T>();
            savedSettings.Initialize();

            string jsonString = JsonConvert.SerializeObject(savedSettings, SerializerSettings);

            m_ShouldCheckQuality = false;

            return PersistenceManager.Instance.Save(jsonString, m_SettingsSO.SaveFileName + m_SettingsSO.SaveFileExtension, m_SettingsSO.SaveFolderName);
        }

        public virtual void Deserialize(SerializableSettings data)
        {
            data.GameSettings.Deserialize(m_SettingsSO.GameSettingsSO.Settings as GameSettings);
            m_SettingsSO.GameSettingsSO.LoadSettings(m_SettingsSO.GameSettingsSO.Settings as GameSettings);

            data.GraphicsSettings.Deserialize(m_SettingsSO.GraphicsSettingsSO.Settings as GraphicsSettings);
            m_SettingsSO.GraphicsSettingsSO.LoadSettings(m_SettingsSO.GraphicsSettingsSO.Settings as GraphicsSettings);

            data.AccessibilitySettings.Deserialize(m_SettingsSO.AccessibilitySettingsSO.Settings as AccessibilitySettings);
            m_SettingsSO.AccessibilitySettingsSO.LoadSettings(m_SettingsSO.AccessibilitySettingsSO.Settings as AccessibilitySettings);

            data.SoundSettings.Deserialize(m_SettingsSO.SoundSettingsSO.Settings);
            m_SettingsSO.SoundSettingsSO.LoadSettings(m_SettingsSO.SoundSettingsSO.Settings);
        }
    }

    [System.Serializable]
    public abstract class SerializableSettings
    {
        public SerializableGameSettings GameSettings;
        public SerializableGraphicsGameSettings GraphicsSettings;
        public SerializableAccesibilitySettings AccessibilitySettings;
        public SerializableSoundManagerSettings SoundSettings;

        public abstract void Initialize();
    }
}