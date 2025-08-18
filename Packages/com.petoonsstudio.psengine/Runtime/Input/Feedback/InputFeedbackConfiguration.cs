using PetoonsStudio.PSEngine.Framework;
using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Haptics;

namespace PetoonsStudio.PSEngine.Utils
{
    [CreateAssetMenu(fileName = "InputFeedbackConfiguration", menuName = "Petoons Studio/Input/Feedback/InputFeedbackConfiguration")]
    public class InputFeedbackConfiguration : SingletonScriptableObject<InputFeedbackConfiguration>
    {
        public const float DEFAULT_VIBRATION_MULTIPLIER = 1f;
        public List<VibrationDeviceProfile> VibrationDevicesProfiles = new();

        public float GetMultiplier(IDualMotorRumble hapticDevice)
        {
            if (SettingsManager.IsReady
                && (SettingsManager.Instance.SettingsSO.AccessibilitySettingsSO.Settings as AccessibilitySettings).GetVibration() == false)
                return 0f;

            VibrationDeviceProfile profile = null;
            Type type = hapticDevice.GetType();

            while (profile == null && typeof(IDualMotorRumble).IsAssignableFrom(type))
            {
                profile = GetProfile(type);
                type = type.BaseType;
            }

            return profile == null ? DEFAULT_VIBRATION_MULTIPLIER : profile.VibrationMultiplier;
        }

        public VibrationDeviceProfile GetProfile(Type type)
        {
            return VibrationDevicesProfiles.Find(x => x.InputDevice == type.AssemblyQualifiedName);
        }
    }

    [Serializable]
    public class VibrationDeviceProfile
    {
        [ClassSelector(typeof(IDualMotorRumble))]
        public string InputDevice;

        public float VibrationMultiplier = 1f;
    }
}
