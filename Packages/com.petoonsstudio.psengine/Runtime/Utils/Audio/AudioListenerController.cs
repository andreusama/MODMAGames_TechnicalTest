using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace PetoonsStudio.PSEngine.Utils
{
    public class AudioListenerController : MonoBehaviour
    {
        [SerializeField, Child] private AudioListener m_Listener;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        private void OnEnable()
        {
            HandleOtherAudioListener(false);

            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        private void OnDisable()
        {
            HandleOtherAudioListener(true);

            SceneManager.sceneLoaded -= OnSceneLoaded;
        }

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            HandleOtherAudioListener(false);
        }

        private void HandleOtherAudioListener(bool enable)
        {
            var audioListeners = FindObjectsOfType<AudioListener>();

            foreach (var audioListener in audioListeners)
            {
                if (audioListener != m_Listener)
                    audioListener.enabled = enable;
            }
        }
    }
}