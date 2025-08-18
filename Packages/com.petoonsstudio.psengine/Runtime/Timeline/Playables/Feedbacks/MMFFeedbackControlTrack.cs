using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    /// <summary>
    /// Track that can be used to control the active state of a GameObject.
    /// </summary>
    [Serializable]
    [TrackClipType(typeof(MMFFeedbackControlPlayableAsset))]
    [TrackBindingType(typeof(GameObject))]
    [TrackColor(1, 1, 0)]
    public class MMFFeedbackControlTrack : TrackAsset
    {
        [SerializeField]
        PostPlaybackState m_PostPlaybackState = PostPlaybackState.LeaveAsIs;
        MMFFeedbackControlMixerPlayable m_ActivationMixer;

        /// <summary>
        /// Specify what state to leave the GameObject in after the Timeline has finished playing.
        /// </summary>
        public enum PostPlaybackState
        {
            /// <summary>
            /// Set the GameObject to active.
            /// </summary>
            Active,

            /// <summary>
            /// Set the GameObject to Inactive.
            /// </summary>
            Inactive,

            /// <summary>
            /// Revert the GameObject to the state in was in before the Timeline was playing.
            /// </summary>
            Revert,

            /// <summary>
            /// Leave the GameObject in the state it was when the Timeline was stopped.
            /// </summary>
            LeaveAsIs
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
            set { m_PostPlaybackState = value; UpdateTrackMode(); }
        }

        /// <inheritdoc/>
        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = MMFFeedbackControlMixerPlayable.Create(graph, inputCount);
            m_ActivationMixer = mixer.GetBehaviour();

            UpdateTrackMode();

            return mixer;
        }

        internal void UpdateTrackMode()
        {
            if (m_ActivationMixer != null)
                m_ActivationMixer.postPlaybackState = m_PostPlaybackState;
        }

        /// <inheritdoc/>
        protected override void OnCreateClip(TimelineClip clip)
        {
            clip.displayName = "Play";
            base.OnCreateClip(clip);
        }
    }
}