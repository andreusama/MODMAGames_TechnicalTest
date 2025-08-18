using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using PetoonsStudio.PSEngine.Timeline;
using UnityEngine.Timeline;
using System;
using PetoonsStudio.PSEngine.Gameplay;
using UnityEngine.InputSystem;
using System.Linq;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CutsceneBinderGeneric : MonoBehaviour, ICutsceneBinder
    {
        [Serializable]
        public struct RealtimeBinding
        {
            public string Name;
            public UnityEngine.Object Value;
        }

        [SerializeField] private RealtimeBinding[] m_RealtimeBindings;

        public IEnumerator SetTrackBindings(PlayableDirector director)
        {
            TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
            var trackAssets = timelineAsset.outputs;

            foreach (var binding in trackAssets)
            {
                foreach (var realtimeBinding in m_RealtimeBindings)
                {
                    if (binding.streamName == realtimeBinding.Name)
                        director.SetGenericBinding(binding.sourceObject, realtimeBinding.Value);
                }
            }

            yield return null;
        }
    }
}