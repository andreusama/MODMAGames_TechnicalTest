using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class FaceSideMixerBehaviour : PlayableBehaviour
    {
        FaceSideTrack.PostPlaybackState m_PostPlaybackState;

        public FaceSideTrack Track { get; set; }

        private FaceSidePlayableBehaviour m_Behaviour;
        private Transform m_TrackBinding;

        public FaceSideTrack.PostPlaybackState postPlaybackState
        {
            get { return m_PostPlaybackState; }
            set { m_PostPlaybackState = value; }
        }

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (playerData == null)
            {
                return;
            }

            m_TrackBinding = (playerData as GameObject).transform;

            if (m_TrackBinding == null)
                return;

            int inputCount = playable.GetInputCount();
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                ScriptPlayable<FaceSidePlayableBehaviour> playableInput = (ScriptPlayable<FaceSidePlayableBehaviour>)playable.GetInput(i);
                m_Behaviour = playableInput.GetBehaviour();

                float inputWeight = playable.GetInputWeight(i);

                if (inputWeight > 0f)
                {
                    switch (m_Behaviour.FacingDirection)
                    {
                        case FaceSideTrack.PostPlaybackState.Left:
                            m_TrackBinding.localScale = new Vector3(-Mathf.Abs(m_TrackBinding.localScale.x), m_TrackBinding.localScale.y, m_TrackBinding.localScale.z);
                            break;
                        case FaceSideTrack.PostPlaybackState.Right:
                            m_TrackBinding.localScale = new Vector3(Mathf.Abs(m_TrackBinding.localScale.x), m_TrackBinding.localScale.y, m_TrackBinding.localScale.z);
                            break;
                    }
                }

                totalWeight += inputWeight;
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (m_TrackBinding == null)
                return;

            switch (m_PostPlaybackState)
            {
                case FaceSideTrack.PostPlaybackState.Left:
                    m_TrackBinding.localScale = new Vector3(-Mathf.Abs(m_TrackBinding.localScale.x), m_TrackBinding.localScale.y, m_TrackBinding.localScale.z);
                    break;
                case FaceSideTrack.PostPlaybackState.Right:
                    m_TrackBinding.localScale = new Vector3(Mathf.Abs(m_TrackBinding.localScale.x), m_TrackBinding.localScale.y, m_TrackBinding.localScale.z);
                    break;
                default:
                    break;
            }
        }
    }
}
