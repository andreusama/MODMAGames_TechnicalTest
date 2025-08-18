using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [TrackColor(0.2866249f, 0.259434f, 1f)]
    [TrackClipType(typeof(ScreenFadeClip))]
    public class ScreenFadeTrack : TrackAsset
    {
        [SerializeField]
        bool m_PostStateActive = false;
        [SerializeField]
        string m_FaderType = "AlphaFader";

        ScreenFadeMixerBehaviour m_ActivationMixer;

        public override Playable CreateTrackMixer(PlayableGraph graph, GameObject go, int inputCount)
        {
            foreach (var clip in GetClips())
            {
                var skipClip = clip.asset as ScreenFadeClip;
                if (skipClip)
                {
                    skipClip.SetClipData(clip);
                }
            }

            var mixer = ScriptPlayable<ScreenFadeMixerBehaviour>.Create(graph, inputCount);
            m_ActivationMixer = mixer.GetBehaviour();

            UpdateTrackMode();

            return mixer;
        }

        internal void UpdateTrackMode()
        {
            if (m_ActivationMixer != null)
            {
                m_ActivationMixer.PostStateActive = m_PostStateActive;
                m_ActivationMixer.FaderType = m_FaderType;
            }
        }
    }
}
