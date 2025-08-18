using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class ActivationSortingGroupMixerPlayable : PlayableBehaviour
    {
        ActivationSortingGroupTrack.PostPlaybackState m_PostPlaybackState;
        bool m_BoundGameObjectInitialStateIsActive;

        private SortingGroup m_BoundGameObject;


        public static ScriptPlayable<ActivationSortingGroupMixerPlayable> Create(PlayableGraph graph, int inputCount)
        {
            return ScriptPlayable<ActivationSortingGroupMixerPlayable>.Create(graph, inputCount);
        }

        public ActivationSortingGroupTrack.PostPlaybackState postPlaybackState
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
                case ActivationSortingGroupTrack.PostPlaybackState.Active:
                    m_BoundGameObject.enabled = true;
                    break;
                case ActivationSortingGroupTrack.PostPlaybackState.Inactive:
                    m_BoundGameObject.enabled = false;
                    break;
                case ActivationSortingGroupTrack.PostPlaybackState.Revert:
                    m_BoundGameObject.enabled = m_BoundGameObjectInitialStateIsActive;
                    break;
                case ActivationSortingGroupTrack.PostPlaybackState.LeaveAsIs:
                default:
                    break;
            }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (m_BoundGameObject == null)
            {
                m_BoundGameObject = playerData as SortingGroup;
                m_BoundGameObjectInitialStateIsActive = m_BoundGameObject != null && m_BoundGameObject.enabled;
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

            m_BoundGameObject.enabled = !hasInput;
        }
    }
}