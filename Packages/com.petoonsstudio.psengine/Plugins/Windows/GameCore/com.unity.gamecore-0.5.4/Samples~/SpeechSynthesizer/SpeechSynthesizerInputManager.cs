// GENERATED AUTOMATICALLY FROM 'Assets/SpeechSynthesizerInputManager.inputactions'

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Utilities;

public class @SpeechSynthesizerInputManager : IInputActionCollection, IDisposable
{
    public InputActionAsset asset { get; }
    public @SpeechSynthesizerInputManager()
    {
        asset = InputActionAsset.FromJson(@"{
    ""name"": ""SpeechSynthesizerInputManager"",
    ""maps"": [
        {
            ""name"": ""UI"",
            ""id"": ""51a27b11-e251-4323-a18a-34cce5c74077"",
            ""actions"": [
                {
                    ""name"": ""Joystick"",
                    ""type"": ""PassThrough"",
                    ""id"": ""451f2fb0-2ffd-4f2a-b3f9-01dc32740196"",
                    ""expectedControlType"": ""Vector2"",
                    ""processors"": """",
                    ""interactions"": """"
                },
                {
                    ""name"": ""Fire1"",
                    ""type"": ""Button"",
                    ""id"": ""425d8001-7ad6-48c0-8cd1-3af04a9d7207"",
                    ""expectedControlType"": """",
                    ""processors"": """",
                    ""interactions"": """"
                }
            ],
            ""bindings"": [
                {
                    ""name"": ""2D Vector"",
                    ""id"": ""bc6476c3-a10d-4cf1-914f-6572e1ca7479"",
                    ""path"": ""2DVector"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Joystick"",
                    ""isComposite"": true,
                    ""isPartOfComposite"": false
                },
                {
                    ""name"": ""up"",
                    ""id"": ""d2f09926-94cc-47dd-870d-cc0ac9ba59a4"",
                    ""path"": ""<Gamepad>/leftStick/up"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Joystick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""down"",
                    ""id"": ""97c898e4-275a-47ca-b276-9c0eed40b8ef"",
                    ""path"": ""<Gamepad>/leftStick/down"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Joystick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""left"",
                    ""id"": ""0c64becf-cdd1-458d-b1dd-d90bd329fea0"",
                    ""path"": ""<Gamepad>/leftStick/left"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Joystick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": ""right"",
                    ""id"": ""ed019250-5aff-4543-acf7-fd4f34202d0b"",
                    ""path"": ""<Gamepad>/leftStick/right"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Joystick"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": true
                },
                {
                    ""name"": """",
                    ""id"": ""5ea0975d-0859-44d2-9c4f-57f5b1d7f68a"",
                    ""path"": ""<Gamepad>/buttonSouth"",
                    ""interactions"": """",
                    ""processors"": """",
                    ""groups"": """",
                    ""action"": ""Fire1"",
                    ""isComposite"": false,
                    ""isPartOfComposite"": false
                }
            ]
        }
    ],
    ""controlSchemes"": [
        {
            ""name"": ""GamePad"",
            ""bindingGroup"": ""GamePad"",
            ""devices"": [
                {
                    ""devicePath"": ""<Gamepad>"",
                    ""isOptional"": false,
                    ""isOR"": false
                }
            ]
        }
    ]
}");
        // UI
        m_UI = asset.FindActionMap("UI", throwIfNotFound: true);
        m_UI_Joystick = m_UI.FindAction("Joystick", throwIfNotFound: true);
        m_UI_Fire1 = m_UI.FindAction("Fire1", throwIfNotFound: true);
    }

    public void Dispose()
    {
        UnityEngine.Object.Destroy(asset);
    }

    public InputBinding? bindingMask
    {
        get => asset.bindingMask;
        set => asset.bindingMask = value;
    }

    public ReadOnlyArray<InputDevice>? devices
    {
        get => asset.devices;
        set => asset.devices = value;
    }

    public ReadOnlyArray<InputControlScheme> controlSchemes => asset.controlSchemes;

    public bool Contains(InputAction action)
    {
        return asset.Contains(action);
    }

    public IEnumerator<InputAction> GetEnumerator()
    {
        return asset.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public void Enable()
    {
        asset.Enable();
    }

    public void Disable()
    {
        asset.Disable();
    }

    // UI
    private readonly InputActionMap m_UI;
    private IUIActions m_UIActionsCallbackInterface;
    private readonly InputAction m_UI_Joystick;
    private readonly InputAction m_UI_Fire1;
    public struct UIActions
    {
        private @SpeechSynthesizerInputManager m_Wrapper;
        public UIActions(@SpeechSynthesizerInputManager wrapper) { m_Wrapper = wrapper; }
        public InputAction @Joystick => m_Wrapper.m_UI_Joystick;
        public InputAction @Fire1 => m_Wrapper.m_UI_Fire1;
        public InputActionMap Get() { return m_Wrapper.m_UI; }
        public void Enable() { Get().Enable(); }
        public void Disable() { Get().Disable(); }
        public bool enabled => Get().enabled;
        public static implicit operator InputActionMap(UIActions set) { return set.Get(); }
        public void SetCallbacks(IUIActions instance)
        {
            if (m_Wrapper.m_UIActionsCallbackInterface != null)
            {
                @Joystick.started -= m_Wrapper.m_UIActionsCallbackInterface.OnJoystick;
                @Joystick.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnJoystick;
                @Joystick.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnJoystick;
                @Fire1.started -= m_Wrapper.m_UIActionsCallbackInterface.OnFire1;
                @Fire1.performed -= m_Wrapper.m_UIActionsCallbackInterface.OnFire1;
                @Fire1.canceled -= m_Wrapper.m_UIActionsCallbackInterface.OnFire1;
            }
            m_Wrapper.m_UIActionsCallbackInterface = instance;
            if (instance != null)
            {
                @Joystick.started += instance.OnJoystick;
                @Joystick.performed += instance.OnJoystick;
                @Joystick.canceled += instance.OnJoystick;
                @Fire1.started += instance.OnFire1;
                @Fire1.performed += instance.OnFire1;
                @Fire1.canceled += instance.OnFire1;
            }
        }
    }
    public UIActions @UI => new UIActions(this);
    private int m_GamePadSchemeIndex = -1;
    public InputControlScheme GamePadScheme
    {
        get
        {
            if (m_GamePadSchemeIndex == -1) m_GamePadSchemeIndex = asset.FindControlSchemeIndex("GamePad");
            return asset.controlSchemes[m_GamePadSchemeIndex];
        }
    }
    public interface IUIActions
    {
        void OnJoystick(InputAction.CallbackContext context);
        void OnFire1(InputAction.CallbackContext context);
    }
}
