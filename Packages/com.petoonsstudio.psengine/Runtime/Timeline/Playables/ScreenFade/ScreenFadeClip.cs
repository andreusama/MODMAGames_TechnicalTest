using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class ScreenFadeClip : PlayableAsset, ITimelineClipAsset
    {
        public Color Color = Color.black;

        [HideInInspector]
        public ScreenFadeBehaviour template = new ScreenFadeBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.Blending; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<ScreenFadeBehaviour>.Create(graph, template);
            ScreenFadeBehaviour clone = playable.GetBehaviour();
            clone.Color = Color;
            return playable;
        }

        public void SetClipData(TimelineClip clipRef)
        {
            //SkipToSecond = SkipToEndOfClip ? clipRef.end : SkipToSecond;
        }
    }
}
