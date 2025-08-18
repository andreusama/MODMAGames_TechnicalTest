using PetoonsStudio.PSEngine.Input.Feedback;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PetoonsStudio.PSEngine.Framework
{
    public class VibrationManager : PersistentSingleton<VibrationManager>, PSEventListener<PSPauseEvent>
    {
        public struct CurrentVibrationData
        {
            public Coroutine Coroutine;
            public VibrationPreset Preset;

            public CurrentVibrationData(Coroutine coroutine, VibrationPreset preset)
            {
                Coroutine = coroutine;
                Preset = preset;
            }
        }

        protected Dictionary<int, CurrentVibrationData> m_CurrentVibrations = new Dictionary<int, CurrentVibrationData>();

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this)
                return;

            m_CurrentVibrations = new Dictionary<int, CurrentVibrationData>();
        }

        void OnEnable()
        {
            this.PSEventStartListening();
        }

        private void OnDisable()
        {
            this.PSEventStopListening();
        }

        public void Vibrate(VibrationPreset preset, int index = -1)
        {
            if (index < 0)
            {
                foreach (var user in PlayerInput.all)
                    Vibrate(preset, user.playerIndex);
            }
            else
            {
                if (m_CurrentVibrations.TryGetValue(index, out CurrentVibrationData data))
                {
                    if (data.Preset.Priority < preset.Priority)
                    {
                        StopVibration(index);
                        var coroutine = StartCoroutine(Vibrate_Internal(preset, index));
                        m_CurrentVibrations[index] = new CurrentVibrationData(coroutine, preset);
                    }
                }
                else
                {
                    var coroutine = StartCoroutine(Vibrate_Internal(preset, index));
                    m_CurrentVibrations.Add(index, new CurrentVibrationData(coroutine, preset));
                }
            }
        }

        public void StopVibration(int index = -1)
        {
            if (index < 0)
            {
                foreach (var user in PlayerInput.all)
                    StopVibration(user.playerIndex);
            }
            else
            {
                if (m_CurrentVibrations.TryGetValue(index, out CurrentVibrationData data))
                {
                    StopCoroutine(data.Coroutine);
                    m_CurrentVibrations.Remove(index);
                }
            }
        }

        protected IEnumerator Vibrate_Internal(VibrationPreset preset, int index)
        {
            try
            {
                VibrationParams vibratonParams = new(preset.LowFrequency, preset.HighFrequency);
                InputFeedbackDispatcher.Instance.SetUserVibration(index, vibratonParams);
                yield return new WaitForSecondsRealtime(preset.Duration);
            }
            finally
            {
                VibrationParams vibratonParams = new(0f, 0f);
                InputFeedbackDispatcher.Instance.SetUserVibration(index, vibratonParams);
                m_CurrentVibrations.Remove(index);
            }
        }

        public void OnPSEvent(PSPauseEvent eventType)
        {
            if (eventType.EventName == PSPauseEvent.Type.PAUSED)
            {
                foreach (var key in m_CurrentVibrations.Keys)
                {
                    if (m_CurrentVibrations[key].Preset.StopOnPause)
                        StopVibration(key);
                }
            }
        }
    }
}
