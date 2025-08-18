using PetoonsStudio.PSEngine.Utils;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;
using KBCore.Refs;
using System;

namespace PetoonsStudio.PSEngine.Framework
{
    [CreateAssetMenu(fileName = "AccessibilitySettingsSO", menuName = "Petoons Studio/X3/Framework/Settings/Accessibility Settings SO")]
    public class AccessibilitySettingsSO : ScriptableObject
    {
        [SerializeReference, SubclassSelector] public IAccessibilitySettings Settings;

        public virtual void LoadSettings(IAccessibilitySettings settings)
        {
            if (settings != null)
            {
                this.Settings = settings;
                this.Settings.ApplySettings();
            }
        }
    }
}