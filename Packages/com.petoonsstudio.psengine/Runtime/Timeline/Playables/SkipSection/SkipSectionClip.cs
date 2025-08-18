using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class SkipSectionClip : PlayableAsset, ITimelineClipAsset
    {
        public bool SkipToEndOfClip = true;
        public double SkipToSecond;

        [HideInInspector]
        public SkipSectionBehaviour template = new SkipSectionBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<SkipSectionBehaviour>.Create(graph, template);
            SkipSectionBehaviour clone = playable.GetBehaviour();
            clone.SkipToSecond = SkipToSecond;
            return playable;
        }

        public void SetClipData(TimelineClip clipRef)
        {
            SkipToSecond = SkipToEndOfClip ? clipRef.end : SkipToSecond;
        }
    }
}