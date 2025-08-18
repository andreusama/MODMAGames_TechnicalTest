using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [System.Serializable]
    public class InputVibrationClip : PlayableAsset, ITimelineClipAsset
    {
        [SerializeField]
        private InputVibrationBehaviour template = new InputVibrationBehaviour();
        public ClipCaps clipCaps => ClipCaps.None;

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<InputVibrationBehaviour>.Create(graph,template);
            return playable;
        }
    }
}
