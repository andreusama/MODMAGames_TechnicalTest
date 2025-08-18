using PetoonsStudio.PSEngine.Utils;
using System;
using UnityEngine;
using UnityEngine.Localization.Settings;

namespace PetoonsStudio.PSEngine.Framework
{
    [Serializable]
    public class GameSettings : IGameSettings
    {
        [ReadOnly] public string Language = "None";

        public virtual void ApplySettings()
        {
            ApplyLanguage();
        }

        public void ApplyLanguage()
        {
            if (Language == LocalizationSettings.SelectedLocale.Identifier.Code)
                return;

            if (string.IsNullOrEmpty(Language) || Language == "None")
            {
                Language = LocalizationSettings.SelectedLocale.Identifier.Code;
                return;
            }

            var newLocale = LocalizationSettings.AvailableLocales.GetLocale(Language);
            if (newLocale == null)
            {
                Debug.LogWarning($"Can't find selected locale {Language}");
                Language = LocalizationSettings.SelectedLocale.Identifier.Code;
                return;
            }

            LocalizationSettings.SelectedLocale = newLocale;
        }
    }

    public interface IGameSettings
    {
        public void ApplySettings();
    }

    [System.Serializable]
    public class SerializableGameSettings
    {
        public string Language;

        public SerializableGameSettings()
        {

        }

        public SerializableGameSettings(GameSettings settings)
        {
            Language = settings.Language;
        }

        public virtual void Deserialize(GameSettings settings)
        {
            settings.Language = Language;
        }
    }
}
