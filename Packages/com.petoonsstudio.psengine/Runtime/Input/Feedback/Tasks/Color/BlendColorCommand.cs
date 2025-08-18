using System;
using System.Threading.Tasks;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class BlendColorCommand : InputFeedbackAsyncCommand
    {
        protected Color m_NewColor = Color.black;
        protected float m_BlendDuration = 1f;

        public BlendColorCommand(DeviceFeedbackData data, Color color, float blendDuration):base(data)
        {
            m_NewColor = color;
            m_BlendDuration = RestrictValueByTick(blendDuration);
        }
        protected virtual async Task Blend()
        {
            try
            {
                float currentDuration = 0;
                Color previousColor = (m_Data as ColorDeviceFeedbackData).Color;
                while (m_Active && m_BlendDuration > currentDuration)
                {
                    Color blendColor = Color.Lerp(previousColor, m_NewColor, currentDuration / m_BlendDuration);
                    SetColor(blendColor);
                    await Task.Delay(MILISECONDS_PER_TICK);
                    currentDuration += SECONDS_PER_TICK;
                }

                if (m_Active)
                {
                    SetColor(m_NewColor);
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
            await Blend();
            End();
        }
    }
}
