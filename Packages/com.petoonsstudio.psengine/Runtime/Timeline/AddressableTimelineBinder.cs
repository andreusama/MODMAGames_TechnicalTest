using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Playables;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    /// <summary>
    /// Used to load and addressable timeline to the PlayableDirector, and manage their bindings.
    /// In order keep the bindings, the non-addressable timeline asset must be set in the PlayableDirector.playableAsset attribute in editor.
    /// Then the OnValidate() method will serialize the needed data. After this happens, the non-addressable asset should be removed.
    /// https://forum.unity.com/threads/timelines-loaded-from-addressables-lose-track-bindings.955959/
    /// </summary>
    [RequireComponent(typeof(PlayableDirector))]
    public class AddressableTimelineBinder : MonoBehaviour, ICutsceneBinder
    {
        [System.Serializable]
        struct TrackBindingData
        {
            public string TimelineName; // the name of the timeline. Assumes they are unique.
            public int TrackIndex; // the index in the output array.
            public Object BindingObject; // the scene object - gameObject or component, bound to the original track

            public TrackBindingData(string timelineName, int trackIndex, UnityEngine.Object bindingObject)
            {
                TimelineName = timelineName;
                TrackIndex = trackIndex;
                BindingObject = bindingObject;
            }
        }

        [SerializeField] bool m_LoadTimelineOnEnable = false;
        [SerializeField] AssetReferenceT<TimelineAsset> m_TimelineAssetReference;

        [Header ("SerializedBindings")]
        [NonReorderable] [SerializeField] List<TrackBindingData> m_TrackBindingDatas;

#if UNITY_EDITOR
        public bool RemoveDirectorTimelineAfterValidation = true;
#endif

        private PlayableDirector m_Director;
        private AsyncOperationHandle<TimelineAsset> m_AssetAsyncOp;
        private bool m_HasBeenLoaded;

        #region Timeline Addressable Loader

        private void OnEnable()
        {
            if (m_LoadTimelineOnEnable)
            {
                LoadTimelineAsset();
            }
        }

        private void OnDisable()
        {
            if (m_HasBeenLoaded)
            {
                m_TimelineAssetReference.ReleaseAsset();
                m_HasBeenLoaded = false;
            }
        }

        /// <summary>
        /// Loads the Timeline Addressable to the PlayableDirector
        /// </summary>
        [ContextMenu("Reasign Timeline")]
        public void LoadTimelineAsset()
        {
            if (!m_HasBeenLoaded && m_TimelineAssetReference.RuntimeKeyIsValid() && m_Director.playableAsset == null && m_AssetAsyncOp.Task == null)
            {
                m_AssetAsyncOp = m_TimelineAssetReference.LoadAssetAsync();
                m_AssetAsyncOp.Completed += OnLoadAssetCompleted;
            }
        }

        /// <summary>
        /// Forces to complete loading the Timeline Addressable
        /// </summary>
        public void CompleteLoadingTimeline()
        {
            if (!m_HasBeenLoaded && m_TimelineAssetReference.RuntimeKeyIsValid() && m_Director.playableAsset == null && m_AssetAsyncOp.Task != null)
            {
                m_AssetAsyncOp.Completed -= OnLoadAssetCompleted;
                m_Director.playableAsset = m_AssetAsyncOp.WaitForCompletion();
                m_HasBeenLoaded = true;
            }
        }

        private void OnLoadAssetCompleted(AsyncOperationHandle<TimelineAsset> obj)
        {
            if (obj.Status == AsyncOperationStatus.Succeeded)
            {
                m_AssetAsyncOp.Completed -= OnLoadAssetCompleted;
                m_Director.playableAsset = obj.Result;
                m_HasBeenLoaded = true;
            }
        }

        #endregion

        #region Binder

        public IEnumerator SetTrackBindings(PlayableDirector director)
        {
            if (m_Director == null) m_Director = director;

            LoadTimelineAsset();
            CompleteLoadingTimeline();

            yield return AssignBindings();
        }

        protected IEnumerator AssignBindings()
        {
            yield return new WaitUntil(() => m_HasBeenLoaded);

            TimelineAsset timelineAsset = (TimelineAsset)m_Director.playableAsset;

            int index = 0;
            foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            {
                foreach (var binding in track.outputs)
                {
                    foreach(TrackBindingData trackBindingInfo in m_TrackBindingDatas)
                    {
                        if (trackBindingInfo.TimelineName == timelineAsset.name && trackBindingInfo.TrackIndex == index)
                        {
                            m_Director.SetGenericBinding(binding.sourceObject, trackBindingInfo.BindingObject);
                        }
                    }
                }
                index++;
            }
        }

        #endregion

        #region Editor binding serialization

        protected void OnValidate()
        {
            m_Director = GetComponent<PlayableDirector>();
            TimelineAsset timelineAsset = (TimelineAsset)m_Director.playableAsset;
            if (timelineAsset == null) return;
            
            m_TrackBindingDatas = new List<TrackBindingData>();
            int index = 0;
            foreach (TrackAsset track in timelineAsset.GetOutputTracks())
            {
                Object bindedObject = m_Director.GetGenericBinding(track);
                if (bindedObject != null)
                {
                    m_TrackBindingDatas.Add(new TrackBindingData(timelineAsset.name, index, bindedObject));
                }
                index++;
            }

#if UNITY_EDITOR
            if (RemoveDirectorTimelineAfterValidation)
            {
                m_Director.playableAsset = null;
            }
#endif
        }
    }

    #endregion
}
