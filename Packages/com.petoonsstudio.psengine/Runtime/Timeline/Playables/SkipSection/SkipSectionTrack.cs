using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(0.2866249f, 0.259434f, 1f)]
    [TrackClipType(typeof(SkipSectionClip))]
    public class SkipSectionTrack : TrackAsset
    {
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var skipClip = clip.asset as SkipSectionClip;
                if (skipClip)
                {
                    skipClip.SetClipData(clip);
                }
            }

            return ScriptPlayable<SkipSectionMixerBehaviour>.Create(graph, inputCount);
        }
    }
}
