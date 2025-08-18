using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;

namespace PetoonsStudio.PSEngine.Utils
{
    public struct AFKEvent
    {

    }

    [AddComponentMenu("Petoons Studio/PSEngine/Utils/Behaviours/AFK Controller")]
    public class AFKController : MonoBehaviour
    {
        [SerializeField] private float m_EventTime = 10f;
        [SerializeField] private UnityEvent m_Event;
        
        private float m_CurrentAfkTime;

        void OnEnable()
        {
            InputSystem.onEvent += InputSystem_onEvent;
        }

        void OnDisable()
        {
            InputSystem.onEvent -= InputSystem_onEvent;
        }

        private void InputSystem_onEvent(InputEventPtr eventPtr, InputDevice device)
        {
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;
            var controls = device.allControls;
            var buttonPressPoint = InputSystem.settings.defaultButtonPressPoint;
            for (var i = 0; i < controls.Count; ++i)
            {
                var control = controls[i] as ButtonControl;
                if (control == null || control.synthetic || control.noisy)
                    continue;
                if (control.ReadValueFromEvent(eventPtr, out var value) && value >= buttonPressPoint)
                {
                    m_CurrentAfkTime = 0f;

                    break;
                }
            }
        }

        /// <summary>
        /// Update
        /// </summary>
        void Update()
        {
            m_CurrentAfkTime += Time.deltaTime;

            if (m_CurrentAfkTime > m_EventTime)
            {
                m_Event?.Invoke();

                m_CurrentAfkTime = 0f;
            }
        }
    }
}