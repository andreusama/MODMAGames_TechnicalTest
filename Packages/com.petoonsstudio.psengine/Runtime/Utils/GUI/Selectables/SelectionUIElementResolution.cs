using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace PetoonsStudio.PSEngine.Utils
{
    [Serializable]
    public class SelectionUIElementResolution : SelectionUIElement<Resolution> 
    {
        protected override string OptionToString()
        {
            return $"{m_SelectionOptions[m_CurrentSelection].Value.width}x{m_SelectionOptions[m_CurrentSelection].Value.height} {(int)m_SelectionOptions[m_CurrentSelection].Value.refreshRateRatio.value} Hz";
        }
    }
}