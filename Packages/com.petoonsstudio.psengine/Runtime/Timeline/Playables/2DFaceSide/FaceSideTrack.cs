using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(0, 1, 0)]
    [TrackBindingType(typeof(GameObject))]
    [TrackClipType(typeof(FaceSideClip))]
    public class FaceSideTrack : TrackAsset
    {
        public enum PostPlaybackState
        {
            Right, Left
        }

        [SerializeField]
        PostPlaybackState m_PostPlaybackState = PostPlaybackState.Right;

        FaceSideMixerBehaviour m_ActivationMixer;

        public PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; UpdateTrackMode(); }
        }

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            var mixer = ScriptPlayable<FaceSideMixerBehaviour>.Create(graph, inputCount);
            m_ActivationMixer = mixer.GetBehaviour();
            m_ActivationMixer.Track = this;

            UpdateTrackMode();

            return mixer;
        }

        internal void UpdateTrackMode()
        {
            if (m_ActivationMixer != null)
                m_ActivationMixer.postPlaybackState = m_PostPlaybackState;
        }
    }
}