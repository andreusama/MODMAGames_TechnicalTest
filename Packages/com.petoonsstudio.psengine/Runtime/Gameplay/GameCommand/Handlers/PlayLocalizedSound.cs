using MoreMountains.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Audio;
using UnityEngine.Localization;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace PetoonsStudio.PSEngine.Gameplay
{
    public class PlayLocalizedSound : GameCommandReceiver
    {
        [SerializeField] protected LocalizedAudioClip m_Audio;
        [SerializeField] protected MMSoundManagerPlayOptions m_Options;

        private AsyncOperationHandle<AudioClip> m_AudioOperation;

        protected override void OnEnable()
        {
            base.OnEnable();

            m_AudioOperation = Addressables.LoadAssetAsync<AudioClip>(m_Audio);
        }

        protected override void OnDisable()
        {
            base.OnDisable();

            Addressables.Release(m_AudioOperation);
        }

        public override void Execute()
        {
            MMSoundManagerSoundPlayEvent.Trigger(m_AudioOperation.Result, m_Options);
            EndAction();
        }
    }
}

