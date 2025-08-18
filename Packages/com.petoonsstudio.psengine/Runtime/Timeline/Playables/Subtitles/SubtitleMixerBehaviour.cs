using PetoonsStudio.PSEngine.EnGUI;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class SubtitleMixerBehaviour : PlayableBehaviour
    {
        bool m_FirstFrameHappened;

        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!EnGUIManager.InstanceExists || EnGUIManager.Instance.GUISubtitles == null)
                return;

            if (!m_FirstFrameHappened)
            {
                m_FirstFrameHappened = true;
            }

            int inputCount = playable.GetInputCount();

            float totalWeight = 0f;
            float greatestWeight = 0f;
            int currentInputs = 0;

            for (int i = 0; i < inputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                ScriptPlayable<SubtitleBehaviour> inputPlayable = (ScriptPlayable<SubtitleBehaviour>)playable.GetInput(i);
                SubtitleBehaviour input = inputPlayable.GetBehaviour();

                totalWeight += inputWeight;

                if (inputWeight > greatestWeight)
                {
                    SubtitlesController.Instance.ShowText(input.TextLocalized);
                    
                    greatestWeight = inputWeight;
                }

                if (!Mathf.Approximately(inputWeight, 0f))
                    currentInputs++;
            }

            if (currentInputs != 1 && 1f - totalWeight > greatestWeight)
            {
                SubtitlesController.Instance.HideText();
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            m_FirstFrameHappened = false;

            SubtitlesController.Instance.HideText();
        }

        public override void OnGraphStart(Playable playable)
        {
            SubtitlesController.Instance.CreateGUI();
        }

        public override void OnGraphStop(Playable playable)
        {
            SubtitlesController.Instance.DestroyGUI();
        }
    }
}