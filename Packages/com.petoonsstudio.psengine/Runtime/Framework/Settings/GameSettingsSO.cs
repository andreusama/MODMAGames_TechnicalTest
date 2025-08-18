using MoreMountains.Tools;
using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "GameSettingsSO", menuName = "Petoons Studio/PSEngine/Framework/Settings/Game Settings SO")]
    public class GameSettingsSO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] public IGameSettings Settings;

        public virtual void LoadSettings(IGameSettings settings)
        {
            if (settings != null)
            {
                Settings = settings;
                Settings.ApplySettings();
            }
        }
    }
}
