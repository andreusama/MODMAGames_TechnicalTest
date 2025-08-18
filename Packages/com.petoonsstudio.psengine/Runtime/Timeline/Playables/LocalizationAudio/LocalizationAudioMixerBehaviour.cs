using MoreMountains.Feedbacks;
using System;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class LocalizationAudioMixerBehaviour : PlayableBehaviour
    {
        private LocalizationAudioBehaviour m_LastBehaviour;

        // NOTE: This function is called at runtime and edit time.  Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying)
                return;

            AudioSource trackBinding = playerData as AudioSource;

            if (!trackBinding)
                return;

            int inputCount = playable.GetInputCount();
            bool hasInput = false;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<LocalizationAudioBehaviour> inputPlayable = (ScriptPlayable<LocalizationAudioBehaviour>)playable.GetInput(i);
                LocalizationAudioBehaviour input = inputPlayable.GetBehaviour();

                // Use the above variables to process each frame of this playable.

                if (inputWeight > 0)
                {
                    hasInput = true;

                    if (m_LastBehaviour != input)
                    {
                        trackBinding.clip = input.LocalizedClip != null ? input.LocalizedClip : input.DefaultClip;
                        trackBinding.volume = input.Volume;
                        trackBinding.loop = input.Loop;
                        trackBinding.spatialBlend = input.SpatialBlend;

                        Debug.Log($"NEW CLIP IS {input.LocalizedClip}");

#if UNITY_PS5 || UNITY_PS4
                        if (input.PlayOnGamepad)
                        {
                            trackBinding.PlayOnGamepad(input.GamepadIndex);
                            trackBinding.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Speaker;
                        }
#endif
                        trackBinding.Play();
#if UNITY_PS5 || UNITY_PS4
                        if (input.PlayOnGamepad)
                        {
                            trackBinding.DisableGamepadOutput();
                        }
#endif
                        m_LastBehaviour = input;
                    }

                    break;
                }
            }

            if (!hasInput)
            {
                if (trackBinding.isPlaying)
                    trackBinding.Stop();
                m_LastBehaviour = null;
            } 
        }
    }
}