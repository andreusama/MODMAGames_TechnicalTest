using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(1, 0.5f, 0.5f)]
    [TrackBindingType(typeof(Animator))]
    [TrackClipType(typeof(StopAnimatorPlayableClip))]
    public class StopAnimatorPlayableTrack : TrackAsset
    {
        internal bool CanCompileClips()
        {
            return !hasClips;
        }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = StopAnimatorPlayableBehaviour.Create(graph, inputCount);
            return mixer;
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Stop Animator Gameplay Logic";
            base.OnCreateClip(clip);
        }
    }
}