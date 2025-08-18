using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.Video;

namespace PetoonsStudio.PSEngine.Utils
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoSequencer : MonoBehaviour
    {
        [SerializeField]
        private List<string> m_ClipSequence;

        [SerializeField]
        private UnityEvent m_OnSequenceEnd;

        [SerializeField, Self] private VideoPlayer m_VideoPlayer;
        private int m_CurrentIndex;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public void StartSequence()
        {
            if (m_ClipSequence.Count == 0)
            {
                OnVideoEnd();
            }
            else
            {
                PlayVideo();
            }
        }

        private void PlayVideo()
        {
            StartCoroutine(PlayVideoInternal());
        }

        private IEnumerator PlayVideoInternal()
        {
            var video = Resources.Load(m_ClipSequence[m_CurrentIndex]) as VideoClip;

            m_VideoPlayer.clip = video;
            if (m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource && m_VideoPlayer.GetTargetAudioSource(0) == null)
            {
                m_VideoPlayer.SetTargetAudioSource(0, m_VideoPlayer.GetComponent<AudioSource>());
            }
            m_VideoPlayer.Prepare();
            m_VideoPlayer.Play();

            yield return new WaitUntil(() => m_VideoPlayer.isPlaying);
            yield return new WaitUntil(() => m_VideoPlayer.isPlaying == false);


            OnVideoEnd();
        }

        private void OnVideoEnd()
        {
            m_CurrentIndex++;

            if (m_CurrentIndex > m_ClipSequence.Count - 1)
            {
                m_OnSequenceEnd.Invoke();
            }
            else
            {
                PlayVideo();
            }
        }
    }
}

