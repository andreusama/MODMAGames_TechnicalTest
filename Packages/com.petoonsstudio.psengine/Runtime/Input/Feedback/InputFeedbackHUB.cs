using PetoonsStudio.PSEngine.Utils;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.DualShock;
using UnityEngine.InputSystem.Haptics;

namespace PetoonsStudio.PSEngine.Input.Feedback
{
    public class InputFeedbackHUB : PersistentSingleton<InputFeedbackHUB>
    {
        public const int GAMEPADS_MAX_SLOTS = 4;

        [SerializeField, ReadOnly]
        private SerializedDictionary<int, DeviceFeedbackData> GamepadsData = new();
#if UNITY_PS5
        private SerializedDictionary<int, AudioSource> AudioSources = new();
#endif

        private AudioClip m_DefaulAudioVibrationClip = null;

        private const string DEFAULT_VIBRATO_SOUND = "SineWaveStereoMid";

#if UNITY_PS5
        protected override void Awake()
        {
            base.Awake();
            if (_instance != this) return;

            m_DefaulAudioVibrationClip = Resources.Load<AudioClip>(DEFAULT_VIBRATO_SOUND);

            for (int i = 0; i < GAMEPADS_MAX_SLOTS; i++)
            {
                InstantiateVibrationAudiosource(i);
            }
        }

        private void OnDestroy()
        {
            Resources.UnloadAsset(m_DefaulAudioVibrationClip);
        }
#endif

        internal AudioSource PlayDeviceAudioVibration(int deviceId, int gamepadSlot, AudioVibrationParams vibrationParams)
        {
#if UNITY_PS5
            var device = GetDevice(deviceId);

            if (device == null || device is not Gamepad)
                return null;

            Gamepad gamepad = device as Gamepad;

            return PlayGamepadAudioVibration(gamepad, gamepadSlot, vibrationParams);
#else
            Debug.LogError("Audio Vibration only supported on PlayStation5");
            return null;
#endif
        }
        internal void SetDeviceVibration(int deviceId, VibrationParams vibrationData)
        {
            var device = GetDevice(deviceId);

            if (device == null)
                return;

            switch (device)
            {
#if UNITY_SWITCH
                case UnityEngine.InputSystem.Switch.INPadRumble hapticDevice:
                    {
                        float multiplier = InputFeedbackConfiguration.Instance.GetMultiplier(hapticDevice);
                        float rigtWeight = MMMaths.Remap(vibrationData.Position.x, -1f, 1f, 0f, 1f);
                        float leftWeight = 1 - rigtWeight;

                        float rightLowFrequency = rigtWeight * vibrationData.LowFrequency;
                        float rightHighFrequency = rigtWeight * vibrationData.HighFrequency;

                        float leftLowFrequency = leftWeight * vibrationData.LowFrequency;
                        float leftHighFrequency = leftWeight * vibrationData.HighFrequency;

                        hapticDevice.SetMotorSpeedRight(multiplier * rightLowFrequency, 160f, multiplier * rightHighFrequency, 320f);
                        hapticDevice.SetMotorSpeedLeft(multiplier * leftLowFrequency, 160f, multiplier * leftHighFrequency, 320f);
                    }
                    break;
#elif UNITY_PS5
                case UnityEngine.InputSystem.PS5.DualSenseGamepad hapticDevice:
                    {
                        float multiplier = InputFeedbackConfiguration.Instance.GetMultiplier(hapticDevice);
                        PlayGamepadAudioVibration(hapticDevice, hapticDevice.slotIndex, new AudioVibrationParams(m_DefaulAudioVibrationClip, vibrationData.Intensity * multiplier, vibrationData.Position));             
                    }
                    break;
#endif
                case IDualMotorRumble hapticDevice:
                    {
                        float multiplier = InputFeedbackConfiguration.Instance.GetMultiplier(hapticDevice);
                        hapticDevice.SetMotorSpeeds(multiplier * vibrationData.LowFrequency, multiplier * vibrationData.HighFrequency);
                    }
                    break;
                default:
                    break;
            }
        }
        internal void SetDeviceColor(int deviceId, Color color)
        {
            var device = GetDevice(deviceId);

            if (device == null)
                return;

            switch (device)
            {
                case IDualShockHaptics hapticDevice:
                    {
                        hapticDevice.SetLightBarColor(color);
                        GetData<ColorDeviceFeedbackData>(deviceId).Color = color;
                    }
                    break;
                default:
                    break;
            }
        }

#if UNITY_PS5
        internal AudioSource PlayGamepadAudioVibration(Gamepad gamepad, int gamepadSlot, AudioVibrationParams vibrationParams)
        {
            Debug.Assert(vibrationParams.AudioClip != null, $"Vibration audioclip is null!");

            float volume = Mathf.Clamp(vibrationParams.Intensity, 0.0f, 1f);

            vibrationParams.Position.y = Mathf.Clamp(vibrationParams.Position.y, -1f, 1f);
            float pitch = MMMaths.Remap(vibrationParams.Position.y, -1f, 1f, 0.5f, 1.5f);

            float panStereo = Mathf.Clamp(vibrationParams.Position.x, -1f, 1f);

            Debug.Log($"PitchValue: {pitch} Volume: {volume} panStereo: {panStereo}");

            AudioSource audioSource = AudioSources[gamepadSlot];

            switch (gamepad)
            {
                case UnityEngine.InputSystem.PS5.DualSenseGamepad:
                    audioSource.clip = vibrationParams.AudioClip;
                    audioSource.volume = volume;
                    audioSource.pitch = pitch;
                    audioSource.panStereo = panStereo;
                    audioSource.PlayOnGamepad(gamepadSlot);

                    break;
                default:
                    break;
            }

            return audioSource;
        }
#endif

        public InputDevice GetDevice(int deviceId)
        {
            return InputSystem.GetDeviceById(deviceId);
        }
        public TData GetData<TData>(int deviceId) where TData : DeviceFeedbackData
        {
            if (!GamepadsData.ContainsKey(deviceId))
                AddData(deviceId);

            return GamepadsData[deviceId] as TData;
        }

        public void AddData(int deviceId)
        {
            InputDevice device = GetDevice(deviceId);

            DeviceFeedbackData data;

            if(ColorSupported(device))
            {
                data = new ColorDeviceFeedbackData(deviceId);
            }
            else
            {
                data = new DeviceFeedbackData(deviceId);
            }

            GamepadsData.Add(deviceId, data);
        }

#region SUPPORT
        public bool AudioVibrationSupported(InputDevice device)
        {
            //TODO: AudioVibration for switch
            switch (device)
            {
#if UNITY_PS5
                case UnityEngine.InputSystem.PS5.DualSenseGamepad:
                    return true;
#endif
                default:
                    return false;
            }
        }
        public bool VibrationSupported(InputDevice device)
        {
            switch (device)
            {
#if UNITY_SWITCH
                case UnityEngine.InputSystem.Switch.INPadRumble:
                    return true;
#endif
                case IDualMotorRumble:
                    return true;
                default:
                    return false;
            }
        }
        public bool ColorSupported(InputDevice device)
        {
            switch(device)
            {
                case IDualShockHaptics:
                    return true;
                default:
                    return false;
            }
        }
#endregion

#region HELPERS
#if UNITY_PS5
        protected AudioSource InstantiateVibrationAudiosource(int slot)
        {
            var holder = new GameObject($"{slot}_VibrationAudioSource", typeof(AudioSource));
            holder.transform.SetParent(transform);

            var audioSource = holder.GetComponent<AudioSource>();
            audioSource.gamepadSpeakerOutputType = GamepadSpeakerOutputType.Vibration;
            audioSource.playOnAwake = false;
            AudioSources.Add(slot, audioSource);

            return audioSource;
        }
#endif
#endregion
    }

    public class VibrationParams
    {
        public VibrationParams(float frequency = 0f, Vector2 position = default) : this(frequency, frequency, position)
        {
        }
        public VibrationParams(float lowFrequency = 0f, float highFrequency = 0f, Vector2 positon = default)
        {
            LowFrequency = lowFrequency;
            HighFrequency = highFrequency;
            Position = positon;
        }

        public float Intensity { get { return (LowFrequency * 0.5f) + (HighFrequency * 0.5f); } }

        public float LowFrequency;
        public float HighFrequency;

        public Vector2 Position;
    }

    public class AudioVibrationParams
    {
        public AudioVibrationParams(AudioClip audioClip, float intensity = 0.5f, Vector2 position = default)
        {
            AudioClip = audioClip;
            Intensity = intensity;
            Position = position;
        }

        public AudioClip AudioClip;
        public float Intensity;
        public Vector2 Position;
    }
}
