#if UNITY_EDITOR || UNITY_GAMECORE || PACKAGE_DOCS_GENERATION
using System.Runtime.InteropServices;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.XInput.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System;
using System.Collections.Generic;

////TODO: player ID

namespace UnityEngine.InputSystem.XInput.LowLevel
{
    // IMPORTANT: State layout must match with GamepadInputStateGXDK in native.
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct GXDKGamepadState : IInputStateTypeInfo
    {
        public static FourCC kFormat => new FourCC('G', 'X', 'G', 'P');

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Justification = "Values mandated by device")]
        public enum Button
        {
            Menu = 0,
            View = 1,
            A = 2,
            B = 3,
            X = 4,
            Y = 5,
            DPadUp = 6,
            DPadDown = 7,
            DPadLeft = 8,
            DPadRight = 9,
            LeftShoulder = 10,
            RightShoulder = 11,
            LeftThumbstick = 12,
            RightThumbstick = 13
        }

        [InputControl(name = "start", bit = (uint)Button.Menu, displayName = "Menu")]
        [InputControl(name = "select", bit = (uint)Button.View, displayName = "View")]
        [InputControl(name = "buttonWest", bit = (uint)Button.X, displayName = "X")]
        [InputControl(name = "buttonSouth", bit = (uint)Button.A, displayName = "A")]
        [InputControl(name = "buttonEast", bit = (uint)Button.B, displayName = "B")]
        [InputControl(name = "buttonNorth", bit = (uint)Button.Y, displayName = "Y")]
        [InputControl(name = "leftShoulder", bit = (uint)Button.LeftShoulder)]
        [InputControl(name = "rightShoulder", bit = (uint)Button.RightShoulder)]
        [InputControl(name = "leftStickPress", bit = (uint)Button.LeftThumbstick)]
        [InputControl(name = "rightStickPress", bit = (uint)Button.RightThumbstick)]
        [InputControl(name = "dpad", layout = "Dpad", sizeInBits = 4, bit = (uint)Button.DPadUp)]
        [InputControl(name = "dpad/up", bit = (uint)Button.DPadUp)]
        [InputControl(name = "dpad/right", bit = (uint)Button.DPadRight)]
        [InputControl(name = "dpad/down", bit = (uint)Button.DPadDown)]
        [InputControl(name = "dpad/left", bit = (uint)Button.DPadLeft)]
        [FieldOffset(0)]
        public uint buttons;

        /// <summary>
        /// Left stick position.
        /// </summary>
        [InputControl(layout = "Stick")]
        [FieldOffset(4)]
        public Vector2 leftStick;

        /// <summary>
        /// Right stick position.
        /// </summary>
        [InputControl(layout = "Stick")]
        [FieldOffset(12)]
        public Vector2 rightStick;

        /// <summary>
        /// Position of the left trigger.
        /// </summary>
        [InputControl]
        [FieldOffset(20)]
        public float leftTrigger;

        /// <summary>
        /// Position of the right trigger.
        /// </summary>
        [InputControl]
        [FieldOffset(24)]
        public float rightTrigger;

        public FourCC format
        {
            get { return kFormat; }
        }

        public GXDKGamepadState WithButton(Button button)
        {
            buttons |= (uint)1 << (int)button;
            return this;
        }
    }

    /// <summary>
    /// GXDK output report sent as command to backend.
    /// </summary>
    // IMPORTANT: Struct must match the GamepadOutputReport in native
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct GXDKGamepadRumbleCommand : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('X', '1', 'G', 'O'); } }

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 16;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public float leftMotor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)] public float rightMotor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 8)] public float leftTriggerMotor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 12)] public float rightTriggerMotor;

        public FourCC typeStatic
        {
            get { return Type; }
        }

        public void SetMotorSpeeds(float leftMotorLevel, float rightMotorLevel, float leftTriggerMotorLevel, float rightTriggerMotorLevel)
        {
            leftMotor = Mathf.Clamp(leftMotorLevel, 0.0f, 1.0f);
            rightMotor = Mathf.Clamp(rightMotorLevel, 0.0f, 1.0f);
            leftTriggerMotor = Mathf.Clamp(leftTriggerMotorLevel, 0.0f, 1.0f);
            rightTriggerMotor = Mathf.Clamp(rightTriggerMotorLevel, 0.0f, 1.0f);
        }

        public static GXDKGamepadRumbleCommand Create()
        {
            return new GXDKGamepadRumbleCommand
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }

    /// <summary>
    /// Retrieve the slot index, default color and user ID of the controller.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct QueryGXDKControllerInfo : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('I', 'N', 'F', 'O'); } }

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 16;

        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public ulong gamepadId;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 8)]
        public ulong userId;

        public FourCC typeStatic
        {
            get { return Type; }
        }

        public QueryGXDKControllerInfo WithGamepadId(ulong id)
        {
            gamepadId = id;
            return this;
        }

        public static QueryGXDKControllerInfo Create()
        {
            return new QueryGXDKControllerInfo()
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                gamepadId = 0,
                userId = 0
            };
        }
    }
}

namespace UnityEngine.InputSystem.XInput
{
    [InputControlLayout(stateType = typeof(GXDKGamepadState), displayName = "GXDK Controller")]
    /// <summary>
    /// A GXDK Gamepad.
    /// </summary>
    [Scripting.Preserve]
    public class GXDKGamepad : XInputController, IXboxOneRumble
    {
        private ulong m_GamepadId = 0;
        private ulong m_GXDKUserId = 0;

        private static GXDKGamepad[] s_Devices;
        private static List<GXDKGamepad> s_DeviceList = new List<GXDKGamepad>(4);

        public new static GXDKGamepad current { get; set; }

        public override void MakeCurrent()
        {
            base.MakeCurrent();
            current = this;
        }

        public ulong gamepadId
        {
            get
            {
                UpdatePadSettings();
                return m_GamepadId;
            }
        }

        public ulong gxdkUserId
        {
            get
            {
                UpdatePadSettings();
                return m_GXDKUserId;
            }
        }

        public new static ReadOnlyArray<GXDKGamepad> all
        {
            get { return new ReadOnlyArray<GXDKGamepad>(s_Devices); }
        }

        public static GXDKGamepad GetByGamepadId(ulong gamepadId)
        {
            if (s_Devices == null)
                return null;

            for (int i = 0; i < s_Devices.Length; i++)
            {
                if (s_Devices[i] != null && s_Devices[i].gamepadId == gamepadId)
                {
                    return s_Devices[i];
                }
            }

            return null;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            s_DeviceList.Add(this);

            s_Devices = s_DeviceList.ToArray();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            s_DeviceList.Remove(this);

            s_Devices = s_DeviceList.ToArray();
        }

        private void UpdatePadSettings()
        {
            var command = QueryGXDKControllerInfo.Create();

            if (ExecuteCommand(ref command) > 0)
            {
                m_GamepadId = command.gamepadId;
                m_GXDKUserId = command.userId;
            }
        }

        public override void PauseHaptics()
        {
            if (!m_LeftMotor.HasValue && !m_RightMotor.HasValue && !m_LeftTriggerMotor.HasValue && !m_RightTriggerMotor.HasValue)
                return;

            var command = GXDKGamepadRumbleCommand.Create();
            command.SetMotorSpeeds(0f, 0f, 0f, 0f);

            ExecuteCommand(ref command);
        }

        public override void ResetHaptics()
        {
            if (!m_LeftMotor.HasValue && !m_RightMotor.HasValue && !m_LeftTriggerMotor.HasValue && !m_RightTriggerMotor.HasValue)
                return;

            var command = GXDKGamepadRumbleCommand.Create();
            command.SetMotorSpeeds(0f, 0f, 0f, 0f);

            ExecuteCommand(ref command);

            m_LeftMotor = null;
            m_RightMotor = null;
            m_LeftTriggerMotor = null;
            m_RightTriggerMotor = null;
        }

        public override void ResumeHaptics()
        {
            if (!m_LeftMotor.HasValue && !m_RightMotor.HasValue && !m_LeftTriggerMotor.HasValue && !m_RightTriggerMotor.HasValue)
                return;

            var command = GXDKGamepadRumbleCommand.Create();

            if (m_LeftMotor.HasValue || m_RightMotor.HasValue || m_LeftTriggerMotor.HasValue || m_RightTriggerMotor.HasValue)
                command.SetMotorSpeeds(m_LeftMotor.Value, m_RightMotor.Value, m_LeftTriggerMotor.Value, m_RightTriggerMotor.Value);

            ExecuteCommand(ref command);
        }

        // Handle calls to global SetMotorSpeeds
        // Note currently this only supports dual motor haptics, left/right trigger motors will default to 0.0f
        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            SetMotorSpeeds(lowFrequency, highFrequency, 0.0f, 0.0f);
        }

        public void SetMotorSpeeds(float lowFrequency, float highFrequency, float leftTrigger, float rightTrigger)
        {
            var command = GXDKGamepadRumbleCommand.Create();
            command.SetMotorSpeeds(lowFrequency, highFrequency, leftTrigger, rightTrigger);

            ExecuteCommand(ref command);

            m_LeftMotor = lowFrequency;
            m_RightMotor = highFrequency;
            m_LeftTriggerMotor = leftTrigger;
            m_RightTriggerMotor = rightTrigger;
        }

        private float? m_LeftMotor;
        private float? m_RightMotor;
        private float? m_LeftTriggerMotor;
        private float? m_RightTriggerMotor;
    }
}
#endif // UNITY_EDITOR || UNITY_GAMECORE
