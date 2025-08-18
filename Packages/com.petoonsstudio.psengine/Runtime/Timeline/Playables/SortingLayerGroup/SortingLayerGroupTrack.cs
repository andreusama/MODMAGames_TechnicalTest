using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(1, 1, 0)]
    [TrackBindingType(typeof(SortingGroup))]
    [TrackClipType(typeof(SortingLayerGroupClip))]
    public class SortingLayerGroupTrack : TrackAsset
    {
        [SerializeField]
        private PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;

        private PlayableDirector m_Director;

        /// <summary>
        /// Specify what state to leave the GameObject in after the Timeline has finished playing.
        /// </summary>
        public enum PostPlaybackState
        {
            /// <summary>
            /// Leave the GameObject in the state it was when the Timeline was stopped.
            /// </summary>
            LeaveAsIs,
            /// <summary>
            /// Revert the GameObject to the state in was in before the Timeline was playing.
            /// </summary>
            Revert
        }

        internal bool CanCompileClips()
        {
            return !hasClips;
        }

        /// <summary>
        /// Specifies what state to leave the GameObject in after the Timeline has finished playing.
        /// </summary>
        public PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; }
        }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            m_Director = go.GetComponent<PlayableDirector>();

            foreach (TimelineClip clip in GetClips())
            {
                if (clip.asset is SortingLayerGroupClip sortingGroup)
                    sortingGroup.template.SortingTrack = this;
            }

            return base.CreateTrackMixer(graph, go, inputCount);
        }

        public override void GatherProperties(PlayableDirector director, IPropertyCollector driver)
        {
            var gameObject = director.GetGenericBinding(this) as SortingGroup;
            if (gameObject != null)
            {
                driver.AddFromName(gameObject, "m_SortingLayerID");
                driver.AddFromName(gameObject, "m_SortingOrder");
            }
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Sorting Group Order";
            base.OnCreateClip(clip);
        }

        public SortingGroup GetSortingGroupBinding() => m_Director.GetGenericBinding(this) as SortingGroup;
    }
}
