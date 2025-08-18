using MoreMountains.Tools;
using Newtonsoft.Json;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Rendering;
using KBCore.Refs;
using System;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "SettingsSO", menuName = "Petoons Studio/PSEngine/Framework/Settings/Settings SO")]
    public class SettingsSO : ScriptableObject
    {
        [Header("Settings")]
        public GameSettingsSO DefaultGameSettingsSO;
        public GraphicsSettingsSO DefaultGraphicsSettingsSO;
        public AccessibilitySettingsSO DefaultAccessibilitySettingsSO;
        public MMSoundManagerSettingsSO DefaultSoundSettingsSO;

        [Header("Save")]
        public string SaveFolderName = "/SaveData";
        public string SaveFileExtension = ".settings";
        public string SaveFileName = "Settings";

        public GameSettingsSO GameSettingsSO { get; private set; }
        public GraphicsSettingsSO GraphicsSettingsSO { get; private set; }
        public AccessibilitySettingsSO AccessibilitySettingsSO { get; private set; }
        public MMSoundManagerSettingsSO SoundSettingsSO { get; private set; }

        public virtual void ApplyDefaults()
        {
            ResetGameSettings();
            ResetGraphicsSettings();
            ResetAccessibilitySettings();
            ResetSoundSettings();
        }

        public void ResetGameSettings()
        {
            GameSettingsSO = Instantiate(DefaultGameSettingsSO);
            GameSettingsSO.Settings.ApplySettings();
        }

        public void ResetGraphicsSettings()
        {
            GraphicsSettingsSO = Instantiate(DefaultGraphicsSettingsSO);
            GraphicsSettingsSO.Settings.ApplySettings();
        }

        public void ResetAccessibilitySettings()
        {
            AccessibilitySettingsSO = Instantiate(DefaultAccessibilitySettingsSO);
            AccessibilitySettingsSO.Settings.ApplySettings();
        }

        public void ResetSoundSettings()
        {
            SoundSettingsSO = Instantiate(DefaultSoundSettingsSO);
            MMSoundManager.Instance.settingsSo = SoundSettingsSO;
        }
    }
}