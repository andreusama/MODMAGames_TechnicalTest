using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Utils
{
    [RequireComponent(typeof(AudioSource))]
    public class OutputAudiosourceToGamepad : MonoBehaviour
    {
        [SerializeField]
        private AudioSource m_AudioSource;
        [SerializeField, Information("Only PS4 and PS4 support.", InformationAttribute.InformationType.Warning, false)]
        private int m_GamepadIndex = 0;

#if UNITY_PS5 || UNITY_PS4
        protected virtual void Awake()
        {
            m_AudioSource.PlayOnGamepad(m_GamepadIndex);
            m_AudioSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Speaker;
        }
#endif

#if UNITY_EDITOR
        protected virtual void OnValidate()
        {
            if (m_AudioSource == null)
                m_AudioSource = GetComponent<AudioSource>();
        }
#endif
    }
}
