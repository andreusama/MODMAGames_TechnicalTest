using KBCore.Refs;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Video;

namespace PetoonsStudio.PSEngine.Utils
{
    [RequireComponent(typeof(VideoPlayer))]
    public class VideoSequencerAddressables : MonoBehaviour
    {
        [SerializeField]
        private List<AssetReference> m_ClipSequence;

        [SerializeField]
        private UnityEvent m_OnSequenceEnd;

        private int m_CurrentIndex;
        [SerializeField, Self] private VideoPlayer m_VideoPlayer;
        private AsyncOperationHandle m_VideoHandle;

        private void OnValidate()
        {
            this.ValidateRefs();
        }

        public IEnumerator LoadSequence()
        {
            if (m_ClipSequence.Count == 0)
            {
                yield break;
            }

            m_ClipSequence.ForEach(x => x.LoadAssetAsync<VideoClip>());

            yield return new WaitUntil(() => IsSecuenceLoaded());
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
            m_VideoHandle = m_ClipSequence[m_CurrentIndex].OperationHandle;

            m_VideoPlayer.clip = m_VideoHandle.Result as VideoClip;

            if (m_VideoPlayer.audioOutputMode == VideoAudioOutputMode.AudioSource && m_VideoPlayer.GetTargetAudioSource(0) == null)
            {
                m_VideoPlayer.SetTargetAudioSource(0, m_VideoPlayer.GetComponent<AudioSource>());
            }

            m_VideoPlayer.Prepare();

            m_VideoPlayer.prepareCompleted += LaunchVideo;
        }

        private void LaunchVideo(VideoPlayer source)
        {
            StartCoroutine(LaunchVideo_Internal());
        }

        private IEnumerator LaunchVideo_Internal()
        {
            m_VideoPlayer.prepareCompleted -= LaunchVideo;
            m_VideoPlayer.Play();

            yield return new WaitUntil(() => {
                return m_VideoPlayer.isPlaying;
            });
            yield return new WaitUntil(() => {
                return m_VideoPlayer.isPlaying == false;
            });

            if (m_VideoHandle.IsValid())
            {
                Addressables.Release(m_VideoHandle);
            }

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

        private bool IsSecuenceLoaded()
        {
            foreach (var item in m_ClipSequence)
            {
                if (!item.OperationHandle.IsDone) return false;
            }

            return true;
        }
    }
}