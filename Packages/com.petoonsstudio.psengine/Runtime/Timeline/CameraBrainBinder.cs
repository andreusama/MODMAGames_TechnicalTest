using Cinemachine;
using PetoonsStudio.PSEngine.Timeline;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class CameraBrainBinder : MonoBehaviour, ICutsceneBinder
    {
        [ReadOnly, SerializeField] private string m_TrackName = "Camera Brain";

        public IEnumerator SetTrackBindings(PlayableDirector director)
        {
            var cameraBrain = Camera.main.GetComponent<CinemachineBrain>();

            TimelineAsset timelineAsset = (TimelineAsset)director.playableAsset;
            var trackAssets = timelineAsset.outputs;

            foreach (var binding in trackAssets)
            {
                if (binding.streamName == m_TrackName)
                    director.SetGenericBinding(binding.sourceObject, cameraBrain);
            }

            yield return null;
        }
    }
}
