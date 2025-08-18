using PetoonsStudio.PSEngine.Input.Feedback;
using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.Timeline;
using UnityEngine.Playables;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Timeline
{
    [System.Serializable]
    public class InputVibrationBehaviour : InputFeedbackBehaviour
    {
        [NotKeyable]
        public bool AutoStopVibration = true;
        [Header("Vibration Configuration")]
        [Range(0f, 1f)]
        public float LowFrequency = 0.5f;
        [Range(0f, 1f)]
        public float HighFrequency = 0.5f;
        [VectorRange(-1f,1f,-1f,1f,true)]
        public Vector2 Position = default;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            base.ProcessFrame(playable, info, playerData);

            VibrationParams vibrationParams = new(LowFrequency, HighFrequency, Position);

            if (m_SingleMode)
            {
                InputFeedbackDispatcher.Instance.SetUserVibration(m_UserIndex, vibrationParams);
            }
            else
            {
                foreach(var user in PlayerInput.all)
                    InputFeedbackDispatcher.Instance.SetUserVibration(user.playerIndex, vibrationParams);
            }
        }

        public override void OnBehaviourPause(Playable playable, FrameData info)
        {
            base.OnBehaviourPause(playable, info);

            if (AutoStopVibration)
            {
                VibrationParams vibrationParams = new(0f, 0f, Vector2.zero);

                if (m_SingleMode)
                {
                    InputFeedbackDispatcher.Instance.SetUserVibration(m_UserIndex, vibrationParams);
                }
                else
                {
                    foreach (var user in PlayerInput.all)
                        InputFeedbackDispatcher.Instance.SetUserVibration(user.playerIndex, vibrationParams);
                }
            }
        }
    }
}
