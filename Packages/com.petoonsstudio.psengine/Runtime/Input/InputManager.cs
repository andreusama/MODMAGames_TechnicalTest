using UnityEngine;
using System;
using UnityEngine.InputSystem;
using UnityEngine.AddressableAssets;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Controls;
using PetoonsStudio.PSEngine.Utils;
using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Multiplatform;
using PetoonsStudio.PSEngine.Multiplatform.Switch;
using static PetoonsStudio.PSEngine.Input.TMPTagProvider;
using UnityEngine.Windows;

#if UNITY_STANDALONE
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Switch;
#elif UNITY_XBOX
using UnityEngine.InputSystem.XInput;
#elif UNITY_PS4
using UnityEngine.InputSystem.DualShock;
#elif UNITY_SWITCH
using UnityEngine.InputSystem.Switch;
#endif

namespace PetoonsStudio.PSEngine.Input
{
    public struct NewDeviceEvent
    {
        public InputDevice NewDevice;

        public NewDeviceEvent(InputDevice newDevice)
        {
            NewDevice = newDevice;
        }
    }

    [DefaultExecutionOrder(-50)]
    public class InputManager : PersistentSingleton<InputManager>, PSEventListener<MMRebindEvent>, PSEventListener<MMBindingsResetEvent>
    {
        public enum Mode { Single, Multiplayer }

        public InputActionAsset InputAsset;
        public Mode InputMode;

        public RebindController RebindController;
        public InputMapsController InputMapsController;

        protected InputDevice m_CurrentDevice;

        public InputDevice CurrentDevice => m_CurrentDevice;

        public const string HANDHELD_ID = "Handheld";
        public const string SWITCH_CONTROLLER = "Npad";
        public const string XBOXONE_CONTROLLER = "XboxOneGamepad";
        public const string GAMECORE_CONTROLLER = "GXDKGamepad";
        public const string PS4_CONTROLLER = "PS4DualShockGamepad";
        public const string PS5_CONTROLLER = "PS5DualSenseGamepad";
        public const string KEYBOARD = "Keyboard";
        public const string AIR_CONSOLE = "AirConsole";

        public static readonly string INPUT_SCHEME_SWITCH = "Switch";
        public static readonly string INPUT_SCHEME_PS4 = "Ps4";
        public static readonly string INPUT_SCHEME_PS5 = "Ps5";
        public static readonly string INPUT_SCHEME_XBOX = "Xbox_GameCore";
        public static readonly string INPUT_SCHEME_PC_GAMEPAD = "PC_Gamepad";
        public static readonly string INPUT_SCHEME_PC_KEYBOARD = "PC_Keyboard";

        protected override void Awake()
        {
            base.Awake();

            if (Instance != this) return;

            IconServiceProvider.Instance.ProviderRequestDone = true;

            ChangeCurrentDevice(GetDefaultInputDevice());
        }

        protected virtual void OnEnable()
        {
            StartListeningEvents();
        }

        protected virtual void OnDisable()
        {
            StopListeningEvents();
        }

        public virtual void StartListeningEvents()
        {
            InputSystem.onEvent += OnDeviceStickOrAnyButtonPress;
            InputSystem.onDeviceChange += OnDeviceChange;
            this.PSEventStartListening<MMRebindEvent>();
            this.PSEventStartListening<MMBindingsResetEvent>();
        }

        public virtual void StopListeningEvents()
        {
            InputSystem.onEvent -= OnDeviceStickOrAnyButtonPress;
            InputSystem.onDeviceChange -= OnDeviceChange;
            this.PSEventStopListening<MMRebindEvent>();
            this.PSEventStopListening<MMBindingsResetEvent>();
        }

        protected virtual void OnDeviceStickOrAnyButtonPress(InputEventPtr eventPtr, InputDevice device)
        {
            // Ignore anything that isn't a state event.
            if (!eventPtr.IsA<StateEvent>() && !eventPtr.IsA<DeltaStateEvent>())
                return;

            if (m_CurrentDevice == device)
            {
                return;
            }

            if (device is Gamepad gamepad)
            {
                var leftStickValue = gamepad.leftStick.ReadValueFromEvent(eventPtr);
                if (leftStickValue != Vector2.zero)
                {
                    OnDeviceAnyButtonPress(device);
                    return;
                }

                var rightStickValue = gamepad.rightStick.ReadValueFromEvent(eventPtr);
                if (rightStickValue != Vector2.zero)
                {
                    OnDeviceAnyButtonPress(device);
                    return;
                }
            }

            foreach (var control in eventPtr.EnumerateChangedControls(device: device, magnitudeThreshold: InputSystem.settings.defaultButtonPressPoint))
            {
                if (control is ButtonControl)
                {
                    OnDeviceAnyButtonPress(device);
                    return;
                }
            }
        }

        protected virtual void OnDeviceAnyButtonPress(InputDevice arg1)
        {
            if (InputMode == Mode.Single)
            {
                if (m_CurrentDevice != arg1)
                {
                    ChangeCurrentDevice(arg1);
                }
            }
        }

        /// <summary>
        /// User has connected/disconnected the device and we need to get the new device to play.
        /// </summary>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        protected virtual void OnDeviceChange(InputDevice arg1, InputDeviceChange arg2)
        {
            switch (arg2)
            {
                case InputDeviceChange.Added:
#if UNITY_SWITCH
                    if (InputMode == Mode.Single)
                    {
                        SwitchManager.Instance.ShowApplet();
                        if (CurrentDevice == null)
                            ChangeCurrentDevice(GetDefaultInputDevice());
                    }
                    else
                    {
                        if (CurrentDevice == null)
                        {
                            SwitchManager.Instance.ShowApplet();
                            ChangeCurrentDevice(GetDefaultInputDevice());
                        }
                    }
#endif
                    break;
                case InputDeviceChange.Disconnected:
                    InputSystem.FlushDisconnectedDevices();
                    break;
            }
        }

        public static string GetDeviceName(InputDevice device)
        {
            string name = string.Empty;
#if UNITY_SWITCH
            name = SWITCH_CONTROLLER;
#elif UNITY_XBOXONE
            name = XBOXONE_CONTROLLER;
#elif UNITY_GAMECORE
            name = GAMECORE_CONTROLLER;
#elif UNITY_PS4
            name = PS4_CONTROLLER;
#elif UNITY_PS5
            name = PS5_CONTROLLER;
#elif UNITY_STANDALONE

            if (device == null || device is Keyboard)
            {
                name = KEYBOARD;
            }
            else if (device.name.Contains("AirConsole"))
            {
                name = AIR_CONSOLE;
            }
            else
            {
                string desc = device.description.ToString();

                if (desc.Contains("Sony") || desc.Contains("PlayStation"))
                {
                    if (device.name.Contains("DualSense"))
                    {
                        name = PS5_CONTROLLER;
                    }
                    else
                    {
                        name = PS4_CONTROLLER;
                    }
                }
                else if (desc.Contains("Nintendo"))
                {
                    name = SWITCH_CONTROLLER;
                }
                else
                {
                    name = GAMECORE_CONTROLLER;
                }
            }
#endif
            return name;
        }

        /// <summary>
        /// Get the Input action asset from a PlayerInput input given the index of all existing PlayerInput.
        /// </summary>
        /// <param name="playerID">Index of the array of PlayerInput.all</param>
        /// <returns></returns>
        public virtual InputActionAsset GetInputAssetFromPlayerID(int playerID)
        {

            if (PlayerInput.all.Count <= 0)
                return InputAsset;

            foreach (var playerInput in PlayerInput.all)
            {
                if (playerInput.playerIndex == playerID)
                {
                    return playerInput.actions;
                }
            }

            Debug.Log($"No player:{playerID} found returning default Input action asset.");

            return InputAsset;
        }

        public void UpdateInputAssetWithRebindChanges(InputActionAsset inputAsset, int playerID = 0)
        {
            if (RebindController == null)
                return;

            RebindController.UpdateInputAssetWithRebindChanges(inputAsset, playerID);
        }

        protected InputDevice GetDefaultInputDevice()
        {
#if UNITY_SWITCH
            foreach (var device in InputSystem.devices)
            {
                if (device is Gamepad) return device;
            }
#else
            foreach (var device in InputSystem.devices)
            {
                if (!(device is Mouse)) return device;
            }
#endif
            return null;
        }

        protected void ChangeCurrentDevice(InputDevice device)
        {
            if (device is Mouse || (device != null && device == m_CurrentDevice))
                return;

            m_CurrentDevice = device;

            if (IconServiceProvider.Instance.ProviderRequestDone)
            {
                if (device == null)
                {
                    IconServiceProvider.Instance.LoadDefaultProvider();
                }
                else
                {
                    IconServiceProvider.Instance.UpdateDeviceProvider(CurrentDevice);
                }
            }

            UpdateBindingMask();
            PSEventManager.TriggerEvent(new NewDeviceEvent(CurrentDevice));
        }

        /// <summary>
        /// Given an Input binding returns the control path, without the device string at the beggining.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static string GetBindingControlPath(InputBinding binding)
        {
            return binding.effectivePath.Substring(binding.effectivePath.IndexOf('/') + 1);
        }

        /// <summary>
        /// Given a Composite Input binding returns the sum of control path, without the device string at the beggining.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static string GetCompositeBindingControlPath(BindingStruct binding)
        {
            string path = "";
            for (int i = 0; i < binding.CompositeParts.Count; i++)
            {
                path += binding.CompositeParts[i].effectivePath.Substring(binding.CompositeParts[i].effectivePath.IndexOf('/') + 1);
            }
            return path;
        }

        /// <summary>
        /// Given a Composite Input binding returns the sum of control path, without the device string at the beggining.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public static string[] GetCompositeBindingControlPaths(BindingStruct binding)
        {
            string[] path = new string[binding.CompositeParts.Count];
            for (int i = 0; i < binding.CompositeParts.Count; i++)
            {
                path[i] = binding.CompositeParts[i].effectivePath.Substring(binding.CompositeParts[i].effectivePath.IndexOf('/') + 1);
            }
            return path;
        }


        /// <summary>
        /// Returns wheter a binding belongs to the current scheme.
        /// </summary>
        /// <param name="binding"></param>
        /// <returns></returns>
        public bool IsBindingMatchingCurrentScheme(InputBinding binding)
        {
            return binding.groups.Contains(GetSchemeName());
        }

        /// <summary>
        /// Fix the path for consoles
        /// </summary>
        /// <param name="basePath"></param>
        /// <returns></returns>
        public string FixPath(string basePath)
        {
#if !UNITY_STANDALONE && !UNITY_EDITOR
            var deviceName = GetDeviceName(CurrentDevice);
            string path = basePath.Replace("<Gamepad>", $"<{deviceName}>");
            return path;
#else
            string path = basePath.Replace("<DualShock4GamepadHID>", "<Gamepad>");
            return path;
#endif
        }

        /// <summary>
        /// Update the Biniding mask of a given Input asset, if no Input Action asset is provided, the base on will be updated.
        /// </summary>
        /// <param name="inputAsset"></param>
        public void UpdateBindingMask(InputActionAsset inputAsset = null)
        {
            var scheme = GetSchemeName();
            if (scheme != null)
            {
                ChangeBindingMask(scheme, inputAsset);
            }
        }

        /// <summary>
        /// Change the binding mask of an Input Action asset by the provided.
        /// Pass an input action asset to apply to another one that the generic one.
        /// </summary>
        /// <param name="platformSchemeName"></param>
        /// <param name="inputAsset"></param>
        public void ChangeBindingMask(string platformSchemeName, InputActionAsset inputAsset = null)
        {
            var choosenInputAsset = (inputAsset != null) ? inputAsset : InputAsset;
            choosenInputAsset.bindingMask = InputBinding.MaskByGroup(platformSchemeName);
        }

        /// <summary>
        /// Returns the expected scheme name for our platforms to mask the bindings in the Input Action Asset
        /// </summary>
        /// <returns></returns>
        public string GetSchemeName()
        {
#if UNITY_SWITCH
            return INPUT_SCHEME_SWITCH;
#elif UNITY_GAMECORE
            return INPUT_SCHEME_XBOX;
#elif UNITY_PS4
            return INPUT_SCHEME_PS4;
#elif UNITY_PS5
            return INPUT_SCHEME_PS5;
#elif UNITY_STANDALONE
            if (CurrentDevice is Keyboard)
                return INPUT_SCHEME_PC_KEYBOARD;
            else if (CurrentDevice is Gamepad)
                return INPUT_SCHEME_PC_GAMEPAD;
            else
                return INPUT_SCHEME_PC_GAMEPAD;
#else
            return null;
#endif
        }

        /// <summary>
        /// Returns the generic name of a control, wheter it belongs to a Gamepad or a Keyboard
        /// </summary>
        /// <returns></returns>
        public static string GetControlGenericName()
        {
            if (Instance.CurrentDevice is Keyboard)
            {
                return "<Keyboard>";
            }
            else if (Instance.CurrentDevice is Gamepad)
            {
                return "<Gamepad>";
            }
            return "<Gamepad>";
        }

        /// <summary>
        /// Returns if PlayerInput has specific device
        /// </summary>
        /// <param name="input"></param>
        /// <param name="device"></param>
        /// <returns></returns>
        public static bool HasDevice(PlayerInput input, InputDevice device)
        {
            foreach (var dev in input.devices)
            {
                if (dev.device == device)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns if player input has device
        /// </summary>
        /// <param name="device"></param>
        /// <returns></returns>
        public static PlayerInput GetInputWithDevice(InputDevice device)
        {
            foreach (var player in PlayerInput.all)
            {
                foreach (var dev in player.devices)
                {
                    if (dev.device == device)
                    {
                        return player;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// Use to get a .json string with the current bindings setup of th InputActionAsset.
        /// </summary>
        /// <returns></returns>
        public virtual string SaveBindings()
        {
            if (RebindController != null)
                return RebindController.SaveRebindChanges();
            else
                return "";
        }

        /// <summary>
        /// Use to apply a .json string to current Input Action asset with modifications.
        /// </summary>
        /// <param name="overrides"></param>
        public virtual void LoadBindings(string overrides)
        {
            if (RebindController != null)
            {
                RebindController.LoadRebindChanges(overrides);
                RebindController.UpdateInputAssetWithRebindChanges(InputAsset);
            }
        }

        /// <summary>
        /// Respond to Rebind event updating the Input Action asset
        /// </summary>
        /// <param name="eventType"></param>
        public void OnPSEvent(MMRebindEvent eventType)
        {
            if (eventType.PlayerID < 1)
                RebindController.UpdateInputAssetWithRebindChanges(InputAsset);
        }

        /// <summary>
        /// Respond to Reset of binding removing all overrides of base input action asset
        /// </summary>
        /// <param name="eventType"></param>
        public void OnPSEvent(MMBindingsResetEvent eventType)
        {
            if (eventType.PlayerID < 1)
                InputAsset.RemoveAllBindingOverrides();
        }
    }
}