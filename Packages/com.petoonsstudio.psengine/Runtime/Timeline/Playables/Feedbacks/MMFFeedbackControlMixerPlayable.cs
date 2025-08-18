using MoreMountains.Feedbacks;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class MMFFeedbackControlMixerPlayable : PlayableBehaviour
    {
        MMFFeedbackControlTrack.PostPlaybackState m_PostPlaybackState;
        bool m_BoundGameObjectInitialStateIsActive;

        private MMF_Player m_BoundGameObject;

        public static ScriptPlayable<MMFFeedbackControlMixerPlayable> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<MMFFeedbackControlMixerPlayable>.Create(graph, inputCount);
        }

        public MMFFeedbackControlTrack.PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_BoundGameObject == null)
                return;

            switch (m_PostPlaybackState)
            {
                case MMFFeedbackControlTrack.PostPlaybackState.Active:
                    if (m_BoundGameObject.InScriptDrivenPause)
                        m_BoundGameObject?.ResumeFeedbacks();
                    else
                        m_BoundGameObject?.PlayFeedbacks();
                    break;
                case MMFFeedbackControlTrack.PostPlaybackState.Inactive:
                    m_BoundGameObject?.StopFeedbacks();
                    break;
                case MMFFeedbackControlTrack.PostPlaybackState.Revert:
                    if (m_BoundGameObjectInitialStateIsActive)
                    {
                        if (m_BoundGameObject.InScriptDrivenPause)
                            m_BoundGameObject?.ResumeFeedbacks();
                        else
                            m_BoundGameObject?.PlayFeedbacks();
                    }
                    else
                        m_BoundGameObject?.StopFeedbacks();
                    break;
                case MMFFeedbackControlTrack.PostPlaybackState.LeaveAsIs:
                default:
                    break;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_BoundGameObject == null)
            {
                m_BoundGameObject = (playerData as GameObject).GetComponent<MMF_Player>();
                m_BoundGameObjectInitialStateIsActive = m_BoundGameObject != null && m_BoundGameObject.IsPlaying;
            }

            if (m_BoundGameObject == null)
                return;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;
            for (int i = 0; i < inputCount; i++)
            {
                if (playable.GetInputWeight(i) > 0)
                {
                    hasInput = true;
                    break;
                }
            }

            if (hasInput)
            {
                if (!m_BoundGameObject.IsPlaying)
                {
                    if (m_BoundGameObject.InScriptDrivenPause)
                        m_BoundGameObject?.ResumeFeedbacks();
                    else
                        m_BoundGameObject?.PlayFeedbacks();
                }
            }
            else
            {
                if (m_BoundGameObject.IsPlaying)
                    m_BoundGameObject?.PauseFeedbacks();
            }
        }
    }
}