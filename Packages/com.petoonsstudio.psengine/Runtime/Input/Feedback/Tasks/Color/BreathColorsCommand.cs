using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class BreathColorsCommand : BlendColorCommand
    {
        protected List<Color> m_Colors;
        protected int m_CurrentColor;
        protected float m_BreathDelay;
        protected float m_Duration;

        public BreathColorsCommand(DeviceFeedbackData data, List<Color> colors, float blendDuration = 1f, float breathDelay = 0f, float duration = -1) : base(data, Color.black, blendDuration)
        {
            m_Colors = colors;
            m_BreathDelay = RestrictValueByTick(breathDelay);
            m_Duration = duration;
        }

        protected async virtual Task ColorBreath()
        {
            try
            {
                float currentDuration = 0;
                while (m_Active && (m_Duration < 0 || m_Duration > currentDuration))
                {
                    m_NewColor = m_Colors[m_CurrentColor];
                    await Blend();
                    await Task.Delay((int)m_BreathDelay * MILISECONDS_PER_SECOND);
                    m_CurrentColor = ++m_CurrentColor >= m_Colors.Count ? 0 : m_CurrentColor;
                    currentDuration += m_Duration < 0 ? m_Duration : SECONDS_PER_TICK;
                }
            }
            catch (Exception e)
            {
                Debug.LogException(e);
                throw;
            }
        }

        public override async void Execute()
        {
            await ColorBreath();
            End();
        }
    }
}
