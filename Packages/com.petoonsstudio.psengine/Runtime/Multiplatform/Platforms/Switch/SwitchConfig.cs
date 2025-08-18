using UnityEngine;
using UnityEditor;
using System;

#if UNITY_SWITCH
using PetoonsStudio.PSEngine.Utils;
using nn.hid;
#endif

namespace PetoonsStudio.PSEngine.Multiplatform.Switch
{
    [CreateAssetMenu(menuName = "Petoons Studio/PSEngine/Multiplatform/Switch/Configuration", fileName = "SwitchConfiguration")]
    public class SwitchConfig : PlatformBaseConfiguration
    {
#if UNITY_SWITCH
        [Header("Applet")]
        [Tooltip("Single/Multi player to launch applet")]
        public bool isSinglePlayer = true;

        [Tooltip("Supported Npad Styles for Switch")]
        public NpadStyle SupportedNpadStyles = NpadStyle.FullKey | NpadStyle.Handheld | NpadStyle.JoyDual;

#if UNITY_EDITOR
        [Tooltip("Enable touchscreen, this field will also setup the Player Settings/Enable Touch Screen in Editor.")]
        public bool EnableTouchScreen = false;
#endif

        [Tooltip("Launch the applet when the games start")]
        public bool ShowAppletOnStart = true;

        [Tooltip("Seconds cooldown to avoid launching the applet repeatedly")]
        public float AppletCooldown = 0.1f;

        [Header("File System")]
        [Tooltip("Define the mount name used to save/load files, max number of characters is 10. Don't use special characters as space.")]
        [Information(" This field can't be longer than 10 characters.",InformationAttribute.InformationType.Info,true)]
        public string MountName = "GameName";

        [Tooltip("Size in Bytes of every save file.")]
        public int JournalSaveDataSize = 32768;

        [Header("Features")]
        [Header("CPU Boost Mode")]
        [Tooltip("Enable to add CPU Boost script capabilities.")]
        public bool AllowCPUBoostMode = false;

        [ConditionalHide(nameof(AllowCPUBoostMode))]
        [Tooltip("Max seconds allowed, after this time, CPU Boost mode will be deactivated. Use 0 to disable deactivation by time.")]
        public float MaxTimeBoostMode = 0f;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if(MountName.Length > 10)
            {
                Debug.LogError("Switch Configuration: MountName should have 10 or less charactes!\n Mount Name will be shorted to ensure correctly functioning.");
                MountName = MountName.Substring(0,10);
            }

            if(Enum.TryParse(SupportedNpadStyles.ToString(), out PlayerSettings.Switch.SupportedNpadStyle sup))
            {
                PlayerSettings.Switch.supportedNpadStyles = sup;
            }
            else
            {
                Debug.LogError("Couldn't Parse Supported Npad Style enum, please ensure that Player Settings/Supported Npad Styles are properly set up!");
            }

            PlayerSettings.Switch.enableTouchScreen = EnableTouchScreen;

        }
#endif
#endif
    }
}

