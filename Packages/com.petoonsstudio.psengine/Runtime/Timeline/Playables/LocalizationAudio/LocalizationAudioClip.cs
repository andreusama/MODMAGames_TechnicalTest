using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [Serializable]
    public class LocalizationAudioClip : PlayableAsset, ITimelineClipAsset
    {
        public override double duration { get { return template.DefaultClip ? template.DefaultClip.length : 1; } }

        public LocalizationAudioBehaviour template = new LocalizationAudioBehaviour();

        public ClipCaps clipCaps
        {
            get { return ClipCaps.None; }
        }

        public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
        {
            var playable = ScriptPlayable<LocalizationAudioBehaviour>.Create(graph, template);
            LocalizationAudioBehaviour clone = playable.GetBehaviour();

            return playable;
        }
    }
}