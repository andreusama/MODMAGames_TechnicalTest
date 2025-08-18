using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

namespace PetoonsStudio.PSEngine.Localization
{
    public class OutrightGenericStartupLocale : IStartupLocaleSelector
    {
#if UNITY_PS4 || UNITY_PS5 || UNITY_SWITCH
    readonly LocaleIdentifier LATIN_LOCALE_IDENTIFIER = new LocaleIdentifier("es-MX");
    readonly LocaleIdentifier ENGLISH_LOCALE_IDENTIFIER = new LocaleIdentifier("en");
    readonly LocaleIdentifier NSW_LATIN_LOCALE_NUMBER = "es-419";
#endif


        public Locale GetStartupLocale(ILocalesProvider availableLocales)
        {
            try
            {
                Locale selectedLocale = null;
#if UNITY_PS4
            switch (UnityEngine.PS4.Utility.systemLanguage)
            {
                case UnityEngine.PS4.Utility.SystemLanguage.PORTUGUESE_PT:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS4.Utility.SystemLanguage.CHINESE_T:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS4.Utility.SystemLanguage.SPANISH_LA:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(LATIN_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS4.Utility.SystemLanguage.FRENCH_CA:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
            }
#elif UNITY_PS5
            switch (UnityEngine.PS5.Utility.systemLanguage)
            {
                case UnityEngine.PS5.Utility.SystemLanguage.PORTUGUESE_PT:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS5.Utility.SystemLanguage.CHINESE_T:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS5.Utility.SystemLanguage.SPANISH_LA:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(LATIN_LOCALE_IDENTIFIER);
                    break;
                case UnityEngine.PS5.Utility.SystemLanguage.FRENCH_CA:
                    selectedLocale = LocalizationSettings.AvailableLocales?.GetLocale(ENGLISH_LOCALE_IDENTIFIER);
                    break;
            }
#elif UNITY_GAMECORE || UNITY_XBOXONE
            string localeName = System.Globalization.CultureInfo.CurrentCulture?.Name;

            selectedLocale = availableLocales.GetLocale(localeName);
            if (selectedLocale != null)
            {
                return selectedLocale;
            }

            var localeParentName = System.Globalization.CultureInfo.CurrentCulture?.Parent?.Name;
            selectedLocale = availableLocales.GetLocale(localeParentName);
            if (selectedLocale != null)
            {
                return selectedLocale;
            }
#elif UNITY_SWITCH
            string systemLanguage = nn.oe.Language.GetDesired();

            if (systemLanguage == NSW_LATIN_LOCALE_NUMBER)
            {
                LocalizationSettings.Instance?.SetSelectedLocale(LocalizationSettings.AvailableLocales?.GetLocale(LATIN_LOCALE_IDENTIFIER));
            }
#endif
                return selectedLocale;
            }
            catch
            {
                return null;
            }
        }
    }
}
