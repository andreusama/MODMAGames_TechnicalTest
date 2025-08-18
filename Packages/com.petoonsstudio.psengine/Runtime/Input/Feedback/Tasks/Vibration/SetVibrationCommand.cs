using PetoonsStudio.PSEngine.Input.Feedback;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class SetVibrationCommand : InputFeedbackCommand
    {
        protected VibrationParams m_VibrationData;
        protected float m_LowFrequency = 0f;
        protected float m_HighFrequency = 0f;
        public SetVibrationCommand(DeviceFeedbackData data, VibrationParams vibrationData) : base(data)
        {
            m_VibrationData = vibrationData;
        }

        public override void Execute()
        {
            SetVibration(m_VibrationData);
            End();
        }
    }
}
