using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System;
using PetoonsStudio.PSEngine.Gameplay;
using UnityEngine.InputSystem;
using System.Linq;
using CrossSceneReference;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CutsceneBinderGuid : MonoBehaviour, ICutsceneBinder
    {
        [Serializable]
        public struct RealtimeBinding
        {
            public string Name;
            public GuidReference Value;
        }

        [SerializeField] private RealtimeBinding[] m_RealtimeBindings;

        private List<GuidComponent> m_CachedGuids;

        public IEnumerator SetTrackBindings(PlayableDirector director)
        {
            m_CachedGuids = new List<GuidComponent>();

            TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
            var trackAssets = timelineAsset.outputs;

            foreach (var realtimeBinding in m_RealtimeBindings)
            {
                foreach (var binding in trackAssets)
                {
                    TrackAsset track = binding.sourceObject as TrackAsset;
                    var group = track.GetGroup();

                    if ((group != null && group.name.StartsWith(realtimeBinding.Name))
                        || binding.streamName.StartsWith(realtimeBinding.Name))
                    {
                        if (binding.outputTargetType == typeof(Transform))
                        {
                            SetBinding<Transform>(director, realtimeBinding, binding);
                        }
                        else if (binding.outputTargetType == typeof(Animator))
                        {
                            SetBinding<Animator>(director, realtimeBinding, binding);
                        }
                        else
                        {
                            SetBinding(director, realtimeBinding, binding);
                        }
                    }

                    if (track is ControlTrack controlTrack)
                        yield return RebindControlTrack(controlTrack, director);
                }
            }

            yield return null;
        }

        private void SetBinding<T>(PlayableDirector director, RealtimeBinding realtimeBinding, PlayableBinding binding) where T : Component
        {
            if (realtimeBinding.Value.TryGetComponent(out T obj))
                director.SetGenericBinding(binding.sourceObject, obj);
            else
            {
                /// Look for cached guids
                foreach (var guid in m_CachedGuids)
                {
                    if (guid.GetGuid() == realtimeBinding.Value.GUID)
                    {
                        director.SetGenericBinding(binding.sourceObject, guid.transform.GetComponent<T>());
                        return;
                    }
                }

                /// Find all GuidComponents
                var guids = FindObjectsOfType<GuidComponent>(true);
                foreach (var guid in guids)
                {
                    if (guid.GetGuid() == realtimeBinding.Value.GUID)
                    {
                        /// Store as cached guid
                        if (!m_CachedGuids.Contains(guid))
                            m_CachedGuids.Add(guid);

                        director.SetGenericBinding(binding.sourceObject, guid.transform.GetComponent<T>());
                        break;
                    }
                }
            }
        }

        private void SetBinding(PlayableDirector director, RealtimeBinding realtimeBinding, PlayableBinding binding)
        {
            if (realtimeBinding.Value.gameObject != null)
                director.SetGenericBinding(binding.sourceObject, realtimeBinding.Value.gameObject);
            else
            {
                /// Look for cached guids
                foreach (var guid in m_CachedGuids)
                {
                    if (guid.GetGuid() == realtimeBinding.Value.GUID)
                    {
                        director.SetGenericBinding(binding.sourceObject, guid.gameObject);
                        return;
                    }
                }

                /// Find all GuidComponents
                var guids = FindObjectsOfType<GuidComponent>(true);
                foreach (var guid in guids)
                {
                    if (guid.GetGuid() == realtimeBinding.Value.GUID)
                    {
                        /// Store as cached guid
                        if (!m_CachedGuids.Contains(guid))
                            m_CachedGuids.Add(guid);

                        director.SetGenericBinding(binding.sourceObject, guid.gameObject);
                        break;
                    }
                }
            }
        }

        private IEnumerator RebindControlTrack(ControlTrack track, PlayableDirector director)
        {
            foreach (var clip in track.GetClips())
            {
                var asset = (clip.asset as ControlPlayableAsset);

                if (asset.updateDirector)
                {
                    UnityEngine.Object assetObj = director.GetReferenceValue(asset.sourceGameObject.exposedName, out bool valid);
                    GameObject gameObject = assetObj as GameObject;

                    if (valid)
                        yield return SetTrackBindings(gameObject.GetComponent<PlayableDirector>());
                }
            }
        }
    }
}