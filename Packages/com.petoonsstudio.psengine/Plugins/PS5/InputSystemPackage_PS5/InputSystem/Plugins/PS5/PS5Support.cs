using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.Controls;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Utilities;
using UnityEngine.InputSystem.DualShock;
using System.Runtime.InteropServices;

#if UNITY_EDITOR
using UnityEditor;
#endif

[assembly: AlwaysLinkAssembly]

namespace UnityEngine.InputSystem.PS5
{
    /// <summary>
    /// Adds support for PS5 controllers.
    /// </summary>
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif
#if UNITY_DISABLE_DEFAULT_INPUT_PLUGIN_INITIALIZATION
    public
#else
    internal
#endif
    static class PS5Support
    {
        static PS5Support()
        {

#if UNITY_EDITOR || !UNITY_PS5
            // through the editor
            InputSystem.RegisterLayout<DualSenseGamepadPC>(
                matches: new InputDeviceMatcher()
                    .WithInterface("HID")
                    // by having these extra values we choose this layout in preference with the dualshock4 one in class DualShockSupport
                    .WithManufacturer("Sony.+Entertainment")
                    .WithProduct("Wireless Controller")

                    .WithCapability("vendorId", 0x54C) // Sony Entertainment.
                    .WithCapability("productId", 0xCE6));
#endif
#if UNITY_EDITOR || UNITY_PS5
#if UNITY_INPUT_SYSTEM_CAN_SET_RUN_IN_BACKGROUND
            InputSystem.runInBackground = true;
#endif
            // on the device itself
            InputSystem.RegisterLayout<PS5TouchControl>("PS5Touch");

            // old naming. will be deprecated
            InputSystem.RegisterLayout<DualSenseGamepad>("PS5DualSenseGamepad",
                matches: new InputDeviceMatcher()
                    .WithInterface("PS5")
                    .WithDeviceClass("PS5DualShockGamepad"));
#endif

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterAssembliesLoaded)]
        private static void InitializeInPlayer() {}
    }






    /// <summary>
    /// Structure of HID input reports for PS5 DualSense controllers. for values of "format" see UnityEngine.InputSystem.LowLevel.InputStateBlock
    /// </summary>
    [StructLayout(LayoutKind.Explicit, Size = 48)]
    public unsafe struct DualSenseHIDInputReport : IInputStateTypeInfo
    {


        [FieldOffset(0)]
        public fixed byte rawHIDInputData[48];

        [FieldOffset(0)] public byte reportId;

        [InputControl(name = "leftStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "leftStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "leftStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "leftStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "leftStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(1)] public byte leftStickX;
        [FieldOffset(2)] public byte leftStickY;

        [InputControl(name = "rightStick", layout = "Stick", format = "VC2B")]
        [InputControl(name = "rightStick/x", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/left", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/right", offset = 0, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1")]
        [InputControl(name = "rightStick/y", offset = 1, format = "BYTE", parameters = "invert,normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5")]
        [InputControl(name = "rightStick/up", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0,clampMax=0.5,invert")]
        [InputControl(name = "rightStick/down", offset = 1, format = "BYTE", parameters = "normalize,normalizeMin=0,normalizeMax=1,normalizeZero=0.5,clamp=1,clampMin=0.5,clampMax=1,invert=false")]
        [FieldOffset(3)] public byte rightStickX;
        [FieldOffset(4)] public byte rightStickY;

        [InputControl(name = "leftTrigger", format = "BYTE")]
        [FieldOffset(5)] public byte leftTrigger;   // should be 5


        [InputControl(name = "rightTrigger", format = "BYTE")]
        [FieldOffset(6)] public byte rightTrigger;  // should be 6


        // offset 7 seems to be some kind of counter


        [InputControl(name = "dpad", format = "BIT", layout = "Dpad", sizeInBits = 4, defaultState = 8)]
        [InputControl(name = "dpad/up", format = "BIT", layout = "DiscreteButton", parameters = "minValue=7,maxValue=1,nullValue=8,wrapAtValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/right", format = "BIT", layout = "DiscreteButton", parameters = "minValue=1,maxValue=3", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/down", format = "BIT", layout = "DiscreteButton", parameters = "minValue=3,maxValue=5", bit = 0, sizeInBits = 4)]
        [InputControl(name = "dpad/left", format = "BIT", layout = "DiscreteButton", parameters = "minValue=5, maxValue=7", bit = 0, sizeInBits = 4)]
        [InputControl(name = "buttonWest", displayName = "Square", bit = 4)]
        [InputControl(name = "buttonSouth", displayName = "Cross", bit = 5)]
        [InputControl(name = "buttonEast", displayName = "Circle", bit = 6)]
        [InputControl(name = "buttonNorth", displayName = "Triangle", bit = 7)]
        [FieldOffset(8)] public byte buttons1;


        [InputControl(name = "leftShoulder", bit = 0)]
        [InputControl(name = "rightShoulder", bit = 1)]
        [InputControl(name = "leftTriggerButton", layout = "Button", bit = 2)]
        [InputControl(name = "rightTriggerButton", layout = "Button", bit = 3)]
        [InputControl(name = "select", displayName = "Share", bit = 4)]
        [InputControl(name = "start", displayName = "Options", bit = 5)]
        [InputControl(name = "leftStickPress", bit = 6)]
        [InputControl(name = "rightStickPress", bit = 7)]
        [FieldOffset(9)] public byte buttons2;

        [InputControl(name = "systemButton", layout = "Button", displayName = "System", bit = 0)]
        [InputControl(name = "touchpadButton", layout = "Button", displayName = "Touchpad Press", bit = 1)]
        [FieldOffset(10)] public byte buttons3; // mute button is bit 2

        [FieldOffset(12)] public int timestamp;



        //16,17,18,19,20,21,22,23,24,25,26,27,28 are movement values
              ////FIXME: gyro and accelerometer aren't read out correctly yet
        [InputControl(name = "acceleration", layout = "Vector3", format = "VC3S")]
        [InputControl(name = "acceleration/x", format = "USHT", offset = 0)]
        [InputControl(name = "acceleration/y", format = "USHT", offset = 2)]
        [InputControl(name = "acceleration/z", format = "USHT", offset = 4)]
        [FieldOffset(16)] public short accelerationX;
        [FieldOffset(18)] public short accelerationY;
        [FieldOffset(20)] public short accelerationZ;

        [InputControl(name = "gyro", layout = "Vector3", format = "VC3S")]
        [InputControl(name = "gyro/x", format = "USHT", offset = 0)]
        [InputControl(name = "gyro/y", format = "USHT", offset = 2)]
        [InputControl(name = "gyro/z", format = "USHT", offset = 4)]
        [FieldOffset(22)] public short gyroX;
        [FieldOffset(24)] public short gyroY;
        [FieldOffset(26)] public short gyroZ;






        [InputControl(name = "touch0", layout = "Integer", format = "UINT")]
        [FieldOffset(33)] public byte touch0data0;
        [FieldOffset(34)] public byte touch0data1;  // lowest 2 nibbles of x
        [FieldOffset(35)] public byte touch0data2;  // lowest nibble of y and highest nibble of x
        [FieldOffset(36)] public byte touch0data3;  // highest 2 nibbles of y

        public bool touch0pressed { get { return (touch0data0&0x80)==0; } }
        public int touch0id { get { return (touch0data0&0x7f); } }
        public int touch0x { get { return (((int)touch0data2&0xf)<<8)|(int)touch0data1; } }
        public int touch0y { get { return (((int)touch0data3)<<4)| ((int)touch0data2&0xf0)>>4; } }


        [InputControl(name = "touch1", layout = "Integer", format = "UINT")]
        [FieldOffset(37)] public byte touch1data0;
        [FieldOffset(38)] public byte touch1data1;  // lowest 2 nibbles of x
        [FieldOffset(39)] public byte touch1data2;  // lowest nibble of y and highest nibble of x
        [FieldOffset(40)] public byte touch1data3;  // highest 2 nibbles of y

        public bool touch1pressed { get { return (touch1data0&0x80)==0; } }
        public int touch1id { get { return (touch1data0&0x7f); } }
        public int touch1x { get { return (((int)touch1data2&0xf)<<8)|(int)touch1data1; } }
        public int touch1y { get { return (((int)touch1data3)<<4)| ((int)touch1data2&0xf0)>>4; } }


        public FourCC format => new FourCC('H', 'I', 'D');
    }


    /// <summary>
    /// PS5 DualSense controller that is interfaced to a HID backend. This is used when running games in the editor with a controller plugged into the PC
    /// </summary>
    [InputControlLayout(stateType = typeof(DualSenseHIDInputReport), displayName = "PS5 DualSense (on PC)")]
    [Scripting.Preserve]
    public class DualSenseGamepadPC : DualShockGamepad
    {
       public Vector3Control acceleration { get; private set; }
        public QuaternionControl orientation { get; private set; }
        public Vector3Control angularVelocity { get; private set; }

 //       public ReadOnlyArray<PS5TouchControl> touches { get; private set; }

        public new static ReadOnlyArray<DualSenseGamepadPC> all => new ReadOnlyArray<DualSenseGamepadPC>(s_Devices);

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
                UpdatePadSettingsIfNeeded();  // 2018.3 uses IOCTL to read this. not required in later versions
                return m_SlotId;
            }
        }

        public int ps5UserId
        {
            get
            {
                UpdatePadSettingsIfNeeded(); // 2018.2 uses IOCTL to read this. not required in later versions
                return m_PS5UserId;
            }
        }


        public static DualSenseGamepadPC GetBySlotIndex(int slotIndex)
        {
            if (slotIndex < 0 || slotIndex >= s_Devices.Length)
                throw new System.ArgumentException("Slot index out of range: " + slotIndex, "slotIndex");

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
            Debug.LogWarning($"s_Devices.Length:{s_Devices.Length}\n{System.Environment.StackTrace}");
            var index = slotIndex;
            if (index >= 0 && index < s_Devices.Length)
            {
                if (s_Devices[index] != null)
                {
                    m_SlotId = GenerateSlotId();
                    s_Devices[m_SlotId] = this;

                }
                else
                {
                    s_Devices[index] = this;
                }
            }
        }

        private void RemoveDeviceFromList()
        {
            if (m_SlotId == -1)
                return;

            var index = slotIndex;
            if (index >= 0 && index < s_Devices.Length && s_Devices[index] == this)
                s_Devices[index] = null;
        }



        protected override void FinishSetup()
        {
            base.FinishSetup();

        // TODO ....
        //    acceleration = GetChildControl<Vector3Control>("acceleration");
        //    orientation = GetChildControl<QuaternionControl>("orientation");
        //    angularVelocity = GetChildControl<Vector3Control>("angularVelocity");

         //   var touchArray = new PS5TouchControl[2];

        //    touchArray[0] = GetChildControl<PS5TouchControl>("touch0");
        //    touchArray[1] = GetChildControl<PS5TouchControl>("touch1");

         //   touches = new ReadOnlyArray<PS5TouchControl>(touchArray);

            var capabilities = description.capabilities;

            var deviceDescriptor = PS5InputDeviceDescriptor.FromJson(capabilities);

            if (deviceDescriptor != null)
            {
                m_SlotId = GenerateSlotId();
                m_DefaultColorId = (int)deviceDescriptor.defaultColorId;
                m_PS5UserId = (int)deviceDescriptor.userId;

                if (!m_LightBarColor.HasValue)
                {
                    m_LightBarColor = PS4ColorIdToColor(m_DefaultColorId);
                }
            }

        }

        // DualSenseGamepadPC is used in editor mode. We don't get a slotId from logged in users like we do
        // in the PS5Player so we need to generate a slotId
        private int GenerateSlotId()
        {
            for (int i = 0; i < s_Devices.Length; i++)
            {
                if (s_Devices[i] == null)
                {
                    return i;
                }
            }
            Debug.LogWarning("Too many gamepads connected, gamepad at slotIndex 0 will be overwritten.");
            return 0;
        }

        public override void PauseHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = DualSenseHIDOutputReport.Create();
            command.SetMotorSpeeds(0f, 0f);
            if (m_LightBarColor.HasValue)
                command.SetColor(Color.black);

            ExecuteCommand(ref command);
        }

        public override void ResetHaptics()
        {
            if (!m_LargeMotor.HasValue && !m_SmallMotor.HasValue && !m_LightBarColor.HasValue)
                return;

            var command = DualSenseHIDOutputReport.Create();
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

            var command = DualSenseHIDOutputReport.Create();

            if (m_LargeMotor.HasValue || m_SmallMotor.HasValue)
                command.SetMotorSpeeds(m_LargeMotor.Value, m_SmallMotor.Value);
            if (m_LightBarColor.HasValue)
                command.SetColor(m_LightBarColor.Value);

            ExecuteCommand(ref command);
        }

        public override void SetLightBarColor(Color color)
        {
             var command = DualSenseHIDOutputReport.Create();
             command.SetColor(color);

             ExecuteCommand(ref command);
             m_LightBarColor = color;
        }

        public void SetTriggerEffect(TriggerEffectParam effect)
        {
             var command = DualSenseHIDOutputReport.Create();
             command.SetTriggerEffect(effect);
             ExecuteCommand(ref command);
        }


        public override void SetMotorSpeeds(float lowFrequency, float highFrequency)
        {
            var command = DualSenseHIDOutputReport.Create();
            command.SetMotorSpeeds(lowFrequency, highFrequency);

            ExecuteCommand(ref command);

            m_LargeMotor = lowFrequency;
            m_SmallMotor = highFrequency;
        }

        private float? m_LargeMotor;
        private float? m_SmallMotor;
        private Color? m_LightBarColor;

        // Slot id for the gamepad. Once set will never change.
        private int m_SlotId = -1;
        private int m_DefaultColorId = -1;
        private int m_PS5UserId = -1;

        private static DualSenseGamepadPC[] s_Devices = new DualSenseGamepadPC[4];

        private void UpdatePadSettingsIfNeeded()
        {
            // if (m_SlotId == -1)
            // {
            //     var command = QueryPS5ControllerInfo.Create();

            //     if (ExecuteCommand(ref command) > 0)
            //     {
            //         m_SlotId = command.slotIndex;
            //         m_DefaultColorId = command.defaultColorId;
            //         m_PS5UserId = command.userId;

            //         if (!m_LightBarColor.HasValue)
            //         {
            //             m_LightBarColor = PS4ColorIdToColor(m_DefaultColorId);
            //         }
            //     }
            // }
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

    [StructLayout(LayoutKind.Explicit, Size = kSize)]
    internal unsafe struct DualSenseHIDOutputReport : IInputDeviceCommandInfo
    {
        public static FourCC Type => new FourCC('H', 'I', 'D', 'O');

         internal const int kSize = InputDeviceCommand.BaseCommandSize + 48;
        internal const int kReportId = 2;   //5;

 //     [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "Flags", Justification = "No better term for underlying data.")]
 //     [Flags]
        public enum Flags
        {
            Rumble = 0x1,
            VibrationModeCompatible = 0x2
        }

        [FieldOffset(0)] public InputDeviceCommand baseCommand;

        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public byte reportId;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1726:UsePreferredTerms", MessageId = "flags", Justification = "No better term for underlying data.")]
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 1)] public byte flags;    // 0x04=triggerRight 0x08=triggerLeft   2=SCE_PAD_VIBRATION_MODE_COMPATIBLE
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 2)] public byte flags2;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 3)] public byte highFrequencyMotorSpeed;   // requires flags = 0x3
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 4)] public byte lowFrequencyMotorSpeed;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2d)] public byte redColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2e)] public byte greenColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 0x2f)] public byte blueColor;
        [FieldOffset(InputDeviceCommand.BaseCommandSize + 9)] public fixed byte unknown2[2];

          [FieldOffset(InputDeviceCommand.BaseCommandSize + 0)] public fixed byte rawdata[48];
        const int kTriggerEffectSize = 11;


        const byte kVibration = 0x26;
        const byte kWeapon = 0x25;
        const byte kFeedback = 0x21;

        const byte kSlopeFeedback = 0x22;


        const byte kNone = 0x05;

        public FourCC typeStatic => Type;

        public void SetMotorSpeeds(float lowFreq, float highFreq)
        {
            flags |= (byte)Flags.VibrationModeCompatible;
            flags |= (byte)Flags.Rumble;

            lowFrequencyMotorSpeed = (byte)Mathf.Clamp(lowFreq * 255, 0, 255);
            highFrequencyMotorSpeed = (byte)Mathf.Clamp(highFreq * 255, 0, 255);
        }

        private void ProcessParameter(int index, byte value, ref int val, ref int val2)
        {
            if (value!=0)
            {
                val = val|(1<<index);
                val2 = val2|((value-1)<<(index*3));
            }
        }

        private void SetTriggerEffect(TriggerEffectCommand effect, int index)
        {
            for (int i=0;i<kTriggerEffectSize;i++)
                rawdata[index+i]=0;

            switch(effect.mode)
            {
                case TriggerEffectMode.Off:
                    rawdata[index]=kNone;
                    break;
                case TriggerEffectMode.Feedback:
                {
                    int val =(0x400)-(1<<effect.feedback.position);
                    long val2 = (((long)effect.feedback.strength-1)*(long)0x3fffffff)/7;

                    rawdata[index]=kFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                }
                    break;
                case TriggerEffectMode.Weapon:
                {
                    int val = (1<<effect.weapon.startPosition) | (1<<effect.weapon.endPosition);
                    rawdata[index]=kWeapon;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(effect.weapon.strength-1);
                }
                    break;
                case TriggerEffectMode.Vibration:
                {
                    int val1 = 0x400-(1<<effect.vibration.position);
                    int val2 = 0x1000000  - (1<<(effect.vibration.position*3));
                    rawdata[index]=kVibration;
                    rawdata[index+1]=(byte)(val1&0xff);
                    rawdata[index+2]=(byte)((val1&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((effect.vibration.amplitude-1)<<3) ;
                    rawdata[index+7]=0;
                    rawdata[index+8]=0;
                    rawdata[index+9]=(byte)effect.vibration.frequency;
                    rawdata[index+10]=0;
                }
                    break;
                 case TriggerEffectMode.SlopeFeedback:
                {
                    int val =(1<<effect.slopeFeedback.startPosition)|(1<<effect.slopeFeedback.endPosition);
                    int val2 = ((effect.slopeFeedback.endStrength-1)*8)| (effect.slopeFeedback.startStrength-1);

                    rawdata[index]=kSlopeFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);

                }
                    break;
                case TriggerEffectMode.MultiplePositionFeedback:
                {
                    int val =0x0;
                    int val2 =0x0;

                    ProcessParameter(0,effect.multiplePositionFeedback.strength0, ref val, ref val2);
                    ProcessParameter(1,effect.multiplePositionFeedback.strength1, ref val, ref val2);
                    ProcessParameter(2,effect.multiplePositionFeedback.strength2, ref val, ref val2);
                    ProcessParameter(3,effect.multiplePositionFeedback.strength3, ref val, ref val2);
                    ProcessParameter(4,effect.multiplePositionFeedback.strength4, ref val, ref val2);
                    ProcessParameter(5,effect.multiplePositionFeedback.strength5, ref val, ref val2);
                    ProcessParameter(6,effect.multiplePositionFeedback.strength6, ref val, ref val2);
                    ProcessParameter(7,effect.multiplePositionFeedback.strength7, ref val, ref val2);
                    ProcessParameter(8,effect.multiplePositionFeedback.strength8, ref val, ref val2);
                    ProcessParameter(9,effect.multiplePositionFeedback.strength9, ref val, ref val2);

                    rawdata[index]=kFeedback;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                }
                break;

                case TriggerEffectMode.MultiplePositionVibration:
                {
                    int val =0x0;
                    int val2 = 0x0;

                    ProcessParameter(0,effect.multiplePositionVibration.amplitude0, ref val, ref val2);
                    ProcessParameter(1,effect.multiplePositionVibration.amplitude1, ref val, ref val2);
                    ProcessParameter(2,effect.multiplePositionVibration.amplitude2, ref val, ref val2);
                    ProcessParameter(3,effect.multiplePositionVibration.amplitude3, ref val, ref val2);
                    ProcessParameter(4,effect.multiplePositionVibration.amplitude4, ref val, ref val2);
                    ProcessParameter(5,effect.multiplePositionVibration.amplitude5, ref val, ref val2);
                    ProcessParameter(6,effect.multiplePositionVibration.amplitude6, ref val, ref val2);
                    ProcessParameter(7,effect.multiplePositionVibration.amplitude7, ref val, ref val2);
                    ProcessParameter(8,effect.multiplePositionVibration.amplitude8, ref val, ref val2);
                    ProcessParameter(9,effect.multiplePositionVibration.amplitude9, ref val, ref val2);

                    rawdata[index]=kVibration;
                    rawdata[index+1]=(byte)(val&0xff);
                    rawdata[index+2]=(byte)((val&0xff00)>>8);
                    rawdata[index+3]=(byte)(val2&0xff);
                    rawdata[index+4]=(byte)((val2&0xff00)>>8);
                    rawdata[index+5]=(byte)((val2&0xff0000)>>16);
                    rawdata[index+6]=(byte)((val2&0xff000000)>>24);
                    rawdata[index+9]=(byte)(effect.multiplePositionVibration.frequency&0xff);

                }
                break;
            }
        }

        public void SetTriggerEffect(TriggerEffectParam data)
        {
            const int kHidLeftTriggerDataOffset = 22;
            const int kHidRightTriggerDataOffset = 11;
            if ((data.triggerMask & TriggerEffectMask.L2) != 0)
            {
                flags|=8;
                SetTriggerEffect(data.left, kHidLeftTriggerDataOffset);
            }
            if ((data.triggerMask & TriggerEffectMask.R2) != 0)
            {
                flags|=4;
                SetTriggerEffect(data.right, kHidRightTriggerDataOffset);
            }
        }





        public void SetColor(Color color)
        {
            //flags |= (byte)Flags.Color;
            flags = 0;
            flags2  = 4;
            redColor = (byte)Mathf.Clamp(color.r * 255, 0, 255);
            greenColor = (byte)Mathf.Clamp(color.g * 255, 0, 255);
            blueColor = (byte)Mathf.Clamp(color.b * 255, 0, 255);
        }

        public void ResetColor()
        {
            //flags |= (byte)Flags.Color;
            flags = 0;
            flags2  = 4;
            redColor = 0;
            greenColor = 0;
            blueColor = 0;
        }

        public static DualSenseHIDOutputReport Create()
        {
            return new DualSenseHIDOutputReport
            {
                baseCommand = new InputDeviceCommand(Type, kSize),
                reportId = kReportId,
            };
        }
    }


}
