using PetoonsStudio.PSEngine.Input.Feedback;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    [System.Serializable]
    public class AudioInputVibrationBehaviour : InputFeedbackBehaviour, ITimelineClipAsset
    {
        public AudioClip VibrationClip = null;
        [Range(0f, 1f)]
        public float Intensity = 0.5f;
        [VectorRange(-1f, 1f, -1f, 1f, true)]
        public Vector2 Position = default;

        public ClipCaps clipCaps => ClipCaps.None;

        public override void OnBehaviourPlay(Playable playable, FrameData info)
        {
            base.OnBehaviourPlay(playable, info);

            if (!Application.isPlaying) return;

            AudioVibrationParams vibrationParams = new(VibrationClip);
            if (m_SingleMode)
            {
                InputFeedbackDispatcher.Instance.PlayUserAudioVibration(m_UserIndex, vibrationParams);
            }
            else
            {
                InputFeedbackDispatcher.Instance.PlayAudioVibration(vibrationParams);
            }
        }
    }
}
