using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace PetoonsStudio.PSEngine.Utils
{
    public static class LocalizationUtils
    {
        public const string LOCALIZATION_ERROR = "[ERROR]";

        public static void SetLocalizedGUIText(TextMeshProUGUI guiText, string table, string key)
        {
            guiText.text = "";
            LocalizationSettings.StringDatabase.GetLocalizedStringAsync(table, key).Completed += (x) =>
            {
                if (x.Succeeded())
                {
                    guiText.text = x.Result;
                }
                else
                {
                    Debug.LogError($"Error loading localized text in table: {table}, key: {key}");
                    guiText.text = LOCALIZATION_ERROR;
                }
            };
        }

        public static void SetLocalizedGUIText(TextMeshProUGUI guiText, LocalizedString localizedString)
        {
            guiText.text = "";
            localizedString.GetLocalizedStringAsync().Completed += (x) =>
            {
                if (x.Succeeded())
                {
                    guiText.text = x.Result;
                }
                else
                {
                    Debug.LogError($"Error loading localized text in localized string: {localizedString}");
                    guiText.text = LOCALIZATION_ERROR;
                }
            };
        }

        public static LocalizedString CreateLocalizedString(string table, string key)
        {
            return new LocalizedString
            {
                TableReference = table,
                TableEntryReference = key
            };
        }

        public static string GetNativeLocaleName(Locale locale)
        {
            return locale.Identifier.CultureInfo.NativeName;
        }

        public static void ChangeLanguage(Locale locale)
        {
            if (LocalizationSettings.InitializationOperation.IsDone)
                LocalizationSettings.SelectedLocale = locale;
            else
                LocalizationSettings.InitializationOperation.Completed += (x) => ChangeLanguage(locale);
        }

        public static void ChangeLanguage(string localeCode)
        {
            ChangeLanguage(LocalizationSettings.AvailableLocales.GetLocale(localeCode));
        }
    }
}