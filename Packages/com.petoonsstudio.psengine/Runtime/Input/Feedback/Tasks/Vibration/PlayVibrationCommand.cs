using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class PlayVibrationCommand : InputFeedbackAsyncCommand
    {
        protected int m_UserIndex;
        protected AudioVibrationParams m_Params;
        public PlayVibrationCommand(DeviceFeedbackData data, int userIndex, AudioVibrationParams vibrationParams) : base(data)
        {
            m_UserIndex = userIndex;
            m_Params = vibrationParams;
        }

        protected virtual async Task Vibrate()
        {
            try
            {
                float currentDuration = 0;

                AudioSource audioSource = PlayVibration(m_UserIndex, m_Params);
                while (m_Active && audioSource.isPlaying)
                {
                    await Task.Delay(MILISECONDS_PER_TICK);
                    currentDuration += SECONDS_PER_TICK;
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
            await Vibrate();
            End();
        }
    }
}
