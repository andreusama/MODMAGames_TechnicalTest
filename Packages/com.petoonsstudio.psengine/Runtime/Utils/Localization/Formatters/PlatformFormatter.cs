using PetoonsStudio.PSEngine.Framework;
using System;
using System.Collections.Generic;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.Core.Extensions;
using UnityEngine.Localization.SmartFormat.Core.Parsing;

namespace PetoonsStudio.PSEngine.Utils
{
    [DisplayName("Text Formatter depending on platform")]
    public class PlatformFormatter : FormatterBase
    {
        public char SplitChar = '|';
        public override string[] DefaultNames => new string[] { "platformText" };

        /// <summary>
        /// Use of SmartString to format text depending of platform
        /// The format to use is {0:platformText(platform1|platform2):textPlatform1|textPlatform2|textDefault}
        /// </summary>
        /// <param name="formattingInfo"></param>
        /// <returns></returns>
        public override bool TryEvaluateFormat(IFormattingInfo formattingInfo)
        {
            if (formattingInfo.FormatterOptions == "") return false;
            var platformOptions = formattingInfo.FormatterOptions.Split(SplitChar);
            var platformFormats = formattingInfo.Format.Split(SplitChar);
            var chosenFormat = DetermineChosenFormat(formattingInfo, platformFormats, platformOptions);
            formattingInfo.Write(chosenFormat, formattingInfo.CurrentValue);

            return true;

        }

        private static Format DetermineChosenFormat(IFormattingInfo formattingInfo, IList<Format> platformFormats, string[] platformOptions)
        {
            var currentValue = PlatformManager.Instance.CurrentPlatform.ToString();
            var chosenIndex = Array.IndexOf(platformOptions, currentValue);

            // Validate the number of formats:
            if (platformFormats.Count < platformOptions.Length)
                throw formattingInfo.FormattingException("You must specify at least " + platformOptions.Length +
                    " choices");
            if (platformFormats.Count > platformOptions.Length + 1)
                throw formattingInfo.FormattingException("You cannot specify more than " + (platformOptions.Length + 1) +
                    " choices");
            if (chosenIndex == -1 && platformFormats.Count == platformOptions.Length)
                throw formattingInfo.FormattingException("\"" + currentValue +
                    "\" is not a valid choice, and a \"default\" choice was not supplied");

            if (chosenIndex == -1) chosenIndex = platformFormats.Count - 1;

            var chosenFormat = platformFormats[chosenIndex];
            return chosenFormat;
        }
    }
}
