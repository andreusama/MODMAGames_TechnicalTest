using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;
using UnityEngine.Timeline;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class SkipSectionMixerBehaviour : PlayableBehaviour
    {
        private Playable m_Playable;
        private double m_SkipToSecond;

        // NOTE: This function is called at runtime and edit time. Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;

            int inputCount = playable.GetInputCount();
            float totalWeight = 0f;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                totalWeight += inputWeight;
                ScriptPlayable<SkipSectionBehaviour> inputPlayable = (ScriptPlayable<SkipSectionBehaviour>)playable.GetInput(i);
                SkipSectionBehaviour input = inputPlayable.GetBehaviour();

                if (inputWeight > 0.5f)
                {
                    m_Playable = playable;
                    m_SkipToSecond = input.SkipToSecond;
                    SkipCutscene();
                }
            }
        }

        private void SkipCutscene()
        {
            PlayableDirector director = m_Playable.GetGraph().GetResolver() as PlayableDirector;
            director.time = m_SkipToSecond;
        }
    }
}