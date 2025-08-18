using System;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using UnityEngine.ResourceManagement.AsyncOperations;
using PetoonsStudio.PSEngine.Utils;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class LocalizationAudioBehaviour : PlayableBehaviour
    {
        public string TableReference;
        public string TableEntryReference;
        public AudioClip DefaultClip;
        [Range(0f,1f)] public float Volume = 1;
        [Range(0f, 1f)] public float SpatialBlend = 0;
        public bool Loop;
        public bool PlayOnGamepad;
        [ConditionalHide("PlayOnGamepad"), SerializeField] public int GamepadIndex;

        private Locale m_CurrentLocale;
        private AsyncOperationHandle<AudioClip> m_AsyncOperation;
   
        public AudioClip LocalizedClip { get; private set; }

        public override void OnPlayableCreate (Playable playable)
        {
            Debug.Log("SE CREA EL PLAYABLE");

            if (Application.isPlaying)
            {
                m_CurrentLocale = LocalizationSettings.Instance.GetSelectedLocale();
                m_AsyncOperation = LocalizationSettings.AssetDatabase.GetLocalizedAssetAsync<AudioClip>(TableReference, TableEntryReference, m_CurrentLocale);
                m_AsyncOperation.Completed += OnAssetLoad;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            Debug.Log("SE DESTRUYE EL PLAYABLE");

            if (Application.isPlaying)
            {
                if (m_AsyncOperation.IsValid())
                    UnityEngine.AddressableAssets.Addressables.Release(m_AsyncOperation);

                LocalizationSettings.AssetDatabase.GetTable(TableReference, m_CurrentLocale).ReleaseAsset(TableEntryReference);
            }
        }

        private void OnAssetLoad(AsyncOperationHandle<AudioClip> obj)
        {
            m_AsyncOperation.Completed -= OnAssetLoad;
            LocalizedClip = obj.Result;
        }
    }
}

