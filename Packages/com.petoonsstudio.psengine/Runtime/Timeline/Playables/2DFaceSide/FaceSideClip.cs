using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class FaceSideClip : PlayableAsset, ITimelineClipAsset
    {
        public FaceSidePlayableBehaviour template = new FaceSidePlayableBehaviour();

        public ClipCaps clipCaps
        {
            get
            {
                return ClipCaps.None;
            }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<FaceSidePlayableBehaviour>.Create(graph, template);

            return playable;
        }

              
    }
}