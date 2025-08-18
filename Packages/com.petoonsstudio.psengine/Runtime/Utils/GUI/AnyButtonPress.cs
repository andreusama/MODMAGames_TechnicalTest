using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.UI;

namespace PetoonsStudio.PSEngine.Utils
{
    public class AnyButtonPress : Selectable
    {
        public UnityEvent StartEvent;

        protected override void OnEnable()
        {
            base.OnEnable();

            InputSystem.onEvent += InputSystem_onEvent;
        }

        protected override void OnDisable()
        {
            base.OnDisable();

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
                    StartEvent.Invoke();
                    break;
                }
            }
        }
    }
}