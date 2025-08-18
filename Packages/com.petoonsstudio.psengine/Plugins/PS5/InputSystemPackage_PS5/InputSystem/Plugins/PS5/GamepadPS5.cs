#if UNITY_EDITOR || UNITY_PS5 || PACKAGE_DOCS_GENERATION
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using System;
using System.Runtime.InteropServices;
using Unity.Collections.LowLevel.Unsafe;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.PS5.LowLevel;


namespace UnityEngine.InputSystem.PS5
{

/* example of how to set controller trigger effect

            var pad = UnityEngine.InputSystem.PS5.DualSenseGamepad.GetBySlotIndex(0);
            var effect = new  UnityEngine.InputSystem.PS5.TriggerEffectParam( UnityEngine.InputSystem.PS5.TriggerEffectMask.L2 | UnityEngine.InputSystem.PS5.TriggerEffectMask.R2);

            effect.left.mode =UnityEngine.InputSystem.PS5.TriggerEffectMode.Feedback;
            effect.left.feedback.position=2;
            effect.left.feedback.strength=8;

            effect.right.mode = UnityEngine.InputSystem.PS5.TriggerEffectMode.Weapon;
            effect.right.weapon.startPosition= 2;
            effect.right.weapon.endPosition= 8;
            effect.right.weapon.strength= 8;

            pad.SetTriggerEffect(effect);
*/

    public enum TriggerEffectMode
    {
        Off, Feedback,Weapon,Vibration,MultiplePositionFeedback,SlopeFeedback,MultiplePositionVibration
    }


    [Flags] public enum TriggerEffectMask : byte
    {
    L2 = 1,
    R2 = 2
    }


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectFeedbackParam
    {
        [FieldOffset(0)] public byte position;
        [FieldOffset(1)] public byte strength;
    }



    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectVibrationParam
    {
        [FieldOffset(0)] public byte position;
        [FieldOffset(1)] public byte amplitude;
        [FieldOffset(2)] public byte frequency;
    }



    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectWeaponParam
    {
        [FieldOffset(0)] public byte startPosition;
        [FieldOffset(1)] public byte endPosition;
        [FieldOffset(2)] public byte strength;
    }


    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectMultiplePositionFeedbackParam
    {
        [FieldOffset(0)] public byte strength0;
        [FieldOffset(1)] public byte strength1;
        [FieldOffset(2)] public byte strength2;
        [FieldOffset(3)] public byte strength3;
        [FieldOffset(4)] public byte strength4;
        [FieldOffset(5)] public byte strength5;
        [FieldOffset(6)] public byte strength6;
        [FieldOffset(7)] public byte strength7;
        [FieldOffset(8)] public byte strength8;
        [FieldOffset(9)] public byte strength9;
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectSlopeFeedbackParam
    {
        [FieldOffset(0)] public byte startPosition;
        [FieldOffset(1)] public byte endPosition;
        [FieldOffset(2)] public byte startStrength;
        [FieldOffset(3)] public byte endStrength;
    }

    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public struct TriggerEffectMultiplePositionVibrationParam
    {
        [FieldOffset(0)] public byte frequency;
        [FieldOffset(1)] public byte amplitude0;
        [FieldOffset(2)] public byte amplitude1;
        [FieldOffset(3)] public byte amplitude2;
        [FieldOffset(4)] public byte amplitude3;
        [FieldOffset(5)] public byte amplitude4;
        [FieldOffset(6)] public byte amplitude5;
        [FieldOffset(7)] public byte amplitude6;
        [FieldOffset(8)] public byte amplitude7;
        [FieldOffset(9)] public byte amplitude8;
        [FieldOffset(10)] public byte amplitude9;
    }

    [StructLayout(LayoutKind.Explicit, Size = 56)]
    public struct TriggerEffectCommand
    {
        [FieldOffset(0)] public TriggerEffectMode mode;
        [FieldOffset(8)] public TriggerEffectWeaponParam weapon;
        [FieldOffset(8)] public TriggerEffectVibrationParam vibration;
        [FieldOffset(8)] public TriggerEffectFeedbackParam feedback;
        [FieldOffset(8)] public TriggerEffectMultiplePositionFeedbackParam multiplePositionFeedback;
        [FieldOffset(8)] public TriggerEffectSlopeFeedbackParam slopeFeedback;
        [FieldOffset(8)] public TriggerEffectMultiplePositionVibrationParam multiplePositionVibration;
    }


    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    public struct TriggerEffectParam : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('T', 'R', 'I', 'G'); } }


        internal const int kSize = InputDeviceCommand.BaseCommandSize + 8+56+56;

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize +0)]  public TriggerEffectMask triggerMask;
        [FieldOffset(InputDeviceCommand.BaseCommandSize +8)]  public TriggerEffectCommand left;
        [FieldOffset(InputDeviceCommand.BaseCommandSize +8+56)]  public TriggerEffectCommand right;


        public FourCC typeStatic
        {
            get { return Type; }
        }

        public TriggerEffectParam(TriggerEffectMask mask)
        {
             this= default(TriggerEffectParam);
             baseCommand = new InputDeviceCommand(Type, kSize);
             triggerMask=mask;
        }

        public static TriggerEffectParam Create()
        {
            return new TriggerEffectParam
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }
}

#pragma warning disable 0649
namespace UnityEngine.InputSystem.PS5.LowLevel
{
    // IMPORTANT: State layout must match with GamepadInputStatePS5 in native.
    [StructLayout(LayoutKind.Explicit, Size = 4)]
    internal struct GamepadStatePS5 : IInputStateTypeInfo
    {
        public static FourCC kFormat => new FourCC('P', '4', 'G', 'P');

        private enum Button
        {
            L3 = 1,
            R3 = 2,
            Options = 3,
            DpadUp = 4,
            DpadRight = 5,
            DpadDown = 6,
            DpadLeft = 7,
            L2 = 8,
            R2 = 9,
            L1 = 10,
            R1 = 11,
            Triangle = 12,
            Circle = 13,
            Cross = 14,
            Square = 15,
            TouchPad = 20,
        }

        [InputControl(name = "leftStickPress", bit = (uint)Button.L3, displayName = "L3")]
        [InputControl(name = "rightStickPress", bit = (uint)Button.R3, displayName = "R3")]
        [InputControl(name = "start", layout = "Button", bit = (uint)Button.Options)]
        [InputControl(name = "dpad", layout = "Dpad", sizeInBits = 4, bit = 4)]
        [InputControl(name = "dpad/up", bit = (uint)Button.DpadUp)]
        [InputControl(name = "dpad/right", bit = (uint)Button.DpadRight)]
        [InputControl(name = "dpad/down", bit = (uint)Button.DpadDown)]
        [InputControl(name = "dpad/left", bit = (uint)Button.DpadLeft)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = (uint)Button.L2, displayName = "L2")]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = (uint)Button.R2, displayName = "R2")]
        [InputControl(name = "leftShoulder", bit = (uint)Button.L1, displayName = "L1")]
        [InputControl(name = "rightShoulder", bit = (uint)Button.R1, displayName = "R1")]
        [InputControl(name = "buttonWest", bit = (uint)Button.Square, displayName = "Square")]
        [InputControl(name = "buttonSouth", bit = (uint)Button.Cross, displayName = "Cross")]
        [InputControl(name = "buttonEast", bit = (uint)Button.Circle, displayName = "Circle")]
        [InputControl(name = "buttonNorth", bit = (uint)Button.Triangle, displayName = "Triangle")]
        [InputControl(name = "touchpadButton", layout = "Button", bit = (uint)Button.TouchPad, displayName = "TouchPad")]
        [InputControl(name = "select", layout = "Button", bit = 21, displayName = "Select")]	// dummy value to override base layout "select" value in struct GamepadState in Gamepad.cs. Failure to have this results in a "Button West" InputAction.performed delegate triggering an additional "Select" InputAction.performed delegate due to both being configured as bit15
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

        [InputControl(name = "acceleration", noisy = true)]
        [FieldOffset(28)]
        public Vector3 acceleration;

        [InputControl(name = "orientation", noisy = true)]
        [FieldOffset(40)]
        public Quaternion orientation;

        [InputControl(name = "angularVelocity", noisy = true)]
        [FieldOffset(56)]
        public Vector3 angularVelocity;

        [InputControl]
        [FieldOffset(68)]
        public PS5Touch touch0;

        [InputControl]
        [FieldOffset(80)]
        public PS5Touch touch1;

        public FourCC format
        {
            get { return kFormat; }
        }
    }




    /// <summary>
    /// PS5 output report sent as command to backend.
    /// </summary>
    // IMPORTANT: Struct must match the DualShockPS5OutputReport in native
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct PS5GamepadOuputCommand : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('P', 'S', 'G', 'O'); } }

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 6;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "No better term for underlying data.")]
        [Flags]
        public enum Flags
        {
            Rumble = 0x1,
            Color = 0x2,
            ResetColor = 0x4,
            ResetOrientation = 0x8
        }

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "No better term for underlying data.")]
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte flags;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public byte largeMotorSpeed;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)] public byte smallMotorSpeed;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 3)] public byte redColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)] public byte greenColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 5)] public byte blueColor;

        public FourCC typeStatic
        {
            get { return Type; }
        }

        public void SetMotorSpeeds(float largeMotor, float smallMotor)
        {
            flags |= (byte)Flags.Rumble;
            largeMotorSpeed = (byte)Mathf.Clamp(largeMotor * 255, 0, 255);
            smallMotorSpeed = (byte)Mathf.Clamp(smallMotor * 255, 0, 255);
        }

        public void SetColor(Color color)
        {
            flags |= (byte)Flags.Color;
            redColor = (byte)Mathf.Clamp(color.r * 255, 0, 255);
            greenColor = (byte)Mathf.Clamp(color.g * 255, 0, 255);
            blueColor = (byte)Mathf.Clamp(color.b * 255, 0, 255);
        }

        public void ResetColor()
        {
            flags |= (byte)Flags.ResetColor;
        }

        public void ResetOrientation()
        {
            flags |= (byte)Flags.ResetOrientation;
        }

        public static PS5GamepadOuputCommand Create()
        {
            return new PS5GamepadOuputCommand
            {
                baseCommand = new InputDeviceCommand(Type, kSize)
            };
        }
    }

    ////REVIEW: this is probably better transmitted as part of the InputDeviceDescription
    /// <summary>
    /// Retrieve the slot index, default color and user ID of the controller.
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal struct QueryPS5ControllerInfo : IInputDeviceCommandInfo
    {
        public static FourCC Type { get { return new FourCC('S', 'L', 'I', 'D'); } }

        internal const int kSize = InputDeviceCommand.BaseCommandSize + 12;

        [FieldOffset(0)]
        public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize)]
        public int slotIndex;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)]
        public int defaultColorId;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 8)]
        public int userId;

        public FourCC typeStatic
        {
            get { return Type; }
        }

        public QueryPS5ControllerInfo WithSlotIndex(int index)
        {
            slotIndex = index;
            return this;
        }

        public static QueryPS5ControllerInfo Create()
        {
            return new QueryPS5ControllerInfo()
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                slotIndex = -1,
                defaultColorId = -1,
                userId = -1
            };
        }
    }

    // IMPORTANT: State layout must match with GamepadInputTouchStatePS5 in native.
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    public struct PS5Touch : IInputStateTypeInfo
    {
        public static FourCC kFormat => new FourCC('P', '4', 'T', 'C');

        public FourCC format
        {
            get { return kFormat; }
        }

        [FieldOffset(0)] public int touchId;
        [FieldOffset(4)] public Vector2 position;
    }
}

namespace UnityEngine.InputSystem.PS5
{
    //Sync to PS5InputDeviceDefinition in sixaxis.cpp
    [Serializable]
    class PS5InputDeviceDescriptor
    {
        public uint slotId;
        public bool isAimController;
        public uint defaultColorId;
        public uint userId;

        internal string ToJson()
        {
            return JsonUtility.ToJson(this);
        }

        internal static PS5InputDeviceDescriptor FromJson(string json)
        {
            return JsonUtility.FromJson<PS5InputDeviceDescriptor>(json);
        }
    }

    ////TODO: Unify this with general touch support
    [InputControlLayout(hideInUI = true)]
    [Scripting.Preserve]
    public class PS5TouchControl : InputControl<PS5Touch>
    {
        /// <summary>
        /// The ID of the touch contact as reported by the underlying system.
        /// </summary>
        /// </remarks>
        [Scripting.Preserve]
        [InputControl(alias = "touchId", offset = 0)]
        public IntegerControl touchId { get; private set; }
        [Scripting.Preserve]
        [InputControl(usage = "Position", offset = 4)]
        public Vector2Control position { get; private set; }

        public PS5TouchControl()
        {
            m_StateBlock.format = new FourCC('P', '4', 'T', 'C');
        }

        protected override void FinishSetup()
        {
            touchId = GetChildControl<IntegerControl>("touchId");
            position = GetChildControl<Vector2Control>("position");
            base.FinishSetup();
        }

        ////FIXME: this suffers from the same problems that TouchControl has in that state layout is hardcoded

        public override unsafe PS5Touch ReadUnprocessedValueFromState(void* statePtr)
        {
            var valuePtr = (PS5Touch*)(byte*)statePtr + (int)m_StateBlock.byteOffset;
            return *valuePtr;
        }

        public override unsafe void WriteValueIntoState(PS5Touch value, void* statePtr)
        {
            var valuePtr = (PS5Touch*)(byte*)statePtr + (int)m_StateBlock.byteOffset;
            UnsafeUtility.MemCpy(valuePtr, UnsafeUtility.AddressOf(ref value), UnsafeUtility.SizeOf<PS5Touch>());
        }
    }

    [InputControlLayout(stateType = typeof(GamepadStatePS5), displayName = "PS5 DualSense (on PS5)")]
    [Scripting.Preserve]
    public class DualSenseGamepad : DualShockGamepad
    {
        public Vector3Control acceleration { get; private set; }
        public QuaternionControl orientation { get; private set; }
        public Vector3Control angularVelocity { get; private set; }

        public ReadOnlyArray<PS5TouchControl> touches { get; private set; }

        public new static ReadOnlyArray<DualSenseGamepad> all => new ReadOnlyArray<DualSenseGamepad>(s_Devices);

        public static ReadOnlyArray<DualSenseGamepad> allAimDevices => new ReadOnlyArray<DualSenseGamepad>(s_AimDevices);

        public Color lightBarColor
        {
            get
            {
                if (m_LightBarColor.HasValue == false)
                {
                    return PS4ColorIdToColor(m_DefaultColorId);
                }

                return m_LightBarColor.Value;
            }
        }

        public int slotIndex
        {
            get
            {
#if !UNITY_2019_1_OR_NEWER
                UpdatePadSettingsIfNeeded();  // 2018.3 uses IOCTL to read this. not required in later versions
#endif

                return m_SlotId;
            }
        }

        public int ps5UserId
        {
            get
            {
#if !UNITY_2019_1_OR_NEWER
                UpdatePadSettingsIfNeeded(); // 2018.2 uses IOCTL to read this. not required in later versions
#endif
                return m_PS5UserId;
            }
        }

        public bool isAimController
        {
            get
            {
                return m_IsAimController;
            }
        }


        public static DualSenseGamepad GetBySlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= s_Devices.Length)
                throw new ArgumentException("Slot index out of range: " + slotIndex, "slotIndex");

            if (s_Devices[slotIndex] != null && s_Devices[slotIndex].slotIndex == slotIndex)
            {
                return s_Devices[slotIndex];
            }

            return null;
        }

        protected override void OnAdded()
        {
            base.OnAdded();

            AddDeviceToList();
        }

        protected override void OnRemoved()
        {
            base.OnRemoved();

            RemoveDeviceFromList();
        }

        private void AddDeviceToList()
        {
            DualSenseGamepad[] deviceList;

            if (m_IsAimController == true)
            {
                deviceList = s_AimDevices;
            }
            else
            {
                deviceList = s_Devices;
            }

            var index = slotIndex;
            if (index >= 0 && index < deviceList.Length)
            {
                Debug.Assert(deviceList[index] == null, "PS4 gamepad with same slotIndex already added");
                deviceList[index] = this;
            }
        }

        private void RemoveDeviceFromList()
        {
            if (m_SlotId == -1)
                return;

            DualSenseGamepad[] deviceList;

            if (m_IsAimController == true)
            {
                deviceList = s_AimDevices;
            }
            else
            {
                deviceList = s_Devices;
            }

            var index = slotIndex;
            if (index >= 0 && index < deviceList.Length && deviceList[index] == this)
                deviceList[index] = null;
        }

        protected override void FinishSetup()
        {
            base.FinishSetup();

            acceleration = GetChildControl<Vector3Control>("acceleration");
            orientation = GetChildControl<QuaternionControl>("orientation");
            angularVelocity = GetChildControl<Vector3Control>("angularVelocity");

            var touchArray = new PS5TouchControl[2];

            touchArray[0] = GetChildControl<PS5TouchControl>("touch0");
            touchArray[1] = GetChildControl<PS5TouchControl>("touch1");

            touches = new ReadOnlyArray<PS5TouchControl>(touchArray);

            var capabilities = description.capabilities;
            var deviceDescriptor = PS5InputDeviceDescriptor.FromJson(capabilities);

            if (deviceDescriptor != null)
            {
                m_SlotId = (int)deviceDescriptor.slotId;
                m_DefaultColorId = (int)deviceDescriptor.defaultColorId;
                m_PS5UserId = (int)deviceDescriptor.userId;

                m_IsAimController = deviceDescriptor.isAimController;

                if (!m_LightBarColor.HasValue)
                {
                    m_LightBarColor = PS4ColorIdToColor(m_DefaultColorId);
                }
            }
        }

        public override void PauseHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(0f, 0f);
            if (m_LightBarColor.HasValue)
                command.SetColor(Color.black);

            ExecuteCommand(ref command);
        }

        public override void ResetHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(0f, 0f);

            if (m_LightBarColor.HasValue)
                command.ResetColor();

            ExecuteCommand(ref command);

            m_LargeMotor = null;
            m_SmallMotor = null;
            m_LightBarColor = null;
        }

        public override void ResumeHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = PS5GamepadOuputCommand.Create();

            if (m_LargeMotor.HasValue || m_SmallMotor.HasValue)
                command.SetMotorSpeeds(m_LargeMotor.Value, m_SmallMotor.Value);
            if (m_LightBarColor.HasValue)
                command.SetColor(m_LightBarColor.Value);

            ExecuteCommand(ref command);
        }

        public override void SetLightBarColor(Color color)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetColor(color);

            ExecuteCommand(ref command);

            m_LightBarColor = color;
        }

        public void ResetLightBarColor()
        {
            var command = PS5GamepadOuputCommand.Create();
            command.ResetColor();

            ExecuteCommand(ref command);

            m_LightBarColor = null;
        }

        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            var command = PS5GamepadOuputCommand.Create();
            command.SetMotorSpeeds(lowFrequency, highFrequency);

            ExecuteCommand(ref command);

            m_LargeMotor = lowFrequency;
            m_SmallMotor = highFrequency;
        }

        public void ResetOrientation()
        {
            var command = PS5GamepadOuputCommand.Create();
            command.ResetOrientation();

            ExecuteCommand(ref command);
        }

        public void SetTriggerEffect(TriggerEffectParam data)
        {
            ExecuteCommand(ref data);
        }



        private float? m_LargeMotor;
        private float? m_SmallMotor;
        private Color? m_LightBarColor;

        // Slot id for the gamepad. Once set will never change.
        private int m_SlotId = -1;
        private int m_DefaultColorId = -1;
        private int m_PS5UserId = -1;
        private bool m_IsAimController;

        private static DualSenseGamepad[] s_Devices = new DualSenseGamepad[4];
        private static DualSenseGamepad[] s_AimDevices = new DualSenseGamepad[4];


        private void UpdatePadSettingsIfNeeded()
        {
            if (m_SlotId == -1)
            {
                var command = QueryPS5ControllerInfo.Create();

                if (ExecuteCommand(ref command) > 0)
                {
                    m_SlotId = command.slotIndex;
                    m_DefaultColorId = command.defaultColorId;
                    m_PS5UserId = command.userId;

                    if (!m_LightBarColor.HasValue)
                    {
                        m_LightBarColor = PS4ColorIdToColor(m_DefaultColorId);
                    }
                }
            }
        }

        private static Color PS4ColorIdToColor(int colorId)
        {
            switch (colorId)
            {
                case 0:
                    return Color.blue;
                case 1:
                    return Color.red;
                case 2:
                    return Color.green;
                case 3:
                    return Color.magenta;
                default:
                    return Color.black;
            }
        }
    }
}
#endif // UNITY_EDITOR || UNITY_PS4
