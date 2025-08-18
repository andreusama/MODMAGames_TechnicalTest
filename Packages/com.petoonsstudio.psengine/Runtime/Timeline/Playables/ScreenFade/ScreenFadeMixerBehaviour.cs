using PetoonsStudio.PSEngine.EnGUI;
using UnityEngine;
using UnityEngine.Playables;

namespace PetoonsStudio.PSEngine.Timeline
{
    public class ScreenFadeMixerBehaviour : PlayableBehaviour
    {
        bool m_PostStateActive;
        string m_FaderType;

        private int m_InputCount;
        private float m_TotalWeight;
        private Color m_BlendedColor;
        private float m_BlendedFadeWeight;

        public bool PostStateActive
        {
            get { return m_PostStateActive; }
            set { m_PostStateActive = value; }
        }

        public string FaderType
        {
            get { return m_FaderType; }
            set { m_FaderType = value; }
        }

        // NOTE: This function is called at runtime and edit time. Keep that in mind when setting the values of properties.
        public override void ProcessFrame(Playable playable, FrameData info, object playerData)
        {
            if (!Application.isPlaying) return;

            m_InputCount = playable.GetInputCount();
            if (m_InputCount == 0) return;

            m_TotalWeight = 0f;
            m_BlendedColor = Color.clear;
            m_BlendedFadeWeight = 0f;

            for (int i = 0; i < m_InputCount; i++)
            {
                float inputWeight = playable.GetInputWeight(i);
                m_TotalWeight += inputWeight;
                ScriptPlayable<ScreenFadeBehaviour> inputPlayable = (ScriptPlayable<ScreenFadeBehaviour>)playable.GetInput(i);
                ScreenFadeBehaviour input = inputPlayable.GetBehaviour();

                if (inputWeight > 0f)
                {
                    m_BlendedColor += input.Color * inputWeight;
                    m_BlendedFadeWeight += inputWeight;
                }
            }

            if (m_TotalWeight > 0f)
            {
                m_BlendedColor = new Color(m_BlendedColor.r, m_BlendedColor.g, m_BlendedColor.b, 1f);
                m_BlendedFadeWeight = Mathf.Clamp01(m_BlendedFadeWeight);

                EnGUIManager.Instance.UpdateFader(m_FaderType, m_BlendedFadeWeight, m_BlendedColor);
            }
        }

        public override void OnPlayableDestroy(Playable playable)
        {
            if (!Application.isPlaying) return;

            if (!m_PostStateActive)
            {
                EnGUIManager.Instance.UpdateFader(m_FaderType, 0f, m_BlendedColor);
            }
        }
    }
}
