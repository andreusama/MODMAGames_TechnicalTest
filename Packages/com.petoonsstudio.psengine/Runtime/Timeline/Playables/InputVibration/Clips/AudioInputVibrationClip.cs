using PetoonsStudio.PSEngine.Timeline;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine
{
    [System.Serializable]
    public class AudioInputVibrationClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private AudioInputVibrationBehaviour template = new AudioInputVibrationBehaviour();
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<AudioInputVibrationBehaviour>.Create(graph, template);
            return playable;
        }
    }
}
