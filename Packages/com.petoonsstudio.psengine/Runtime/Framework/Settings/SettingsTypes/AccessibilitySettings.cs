using PetoonsStudio.PSEngine.Utils;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.Rendering.Universal;

namespace PetoonsStudio.PSEngine.Framework
{
    [Serializable]
    public class AccessibilitySettings : IAccessibilitySettings
    {
        public enum ColorBlindnessSetting
        {
            Off = 0,
            Deuteranopia = 1,
            Protanopia = 2,
            Tritanopia = 3
        }

        public const string k_RendererFeaturesPropertyName = "rendererFeatures";

        public ColorBlindnessSetting ColorBlindness = ColorBlindnessSetting.Off;
        public bool ControllerVibration = true;

        public bool GetVibration()
        {
            return ControllerVibration;
        }
        public ColorBlindnessSetting GetColorBlindness()
        {
            return ColorBlindness;
        }
        public void SetVibration(bool vibration)
        {
            ControllerVibration = vibration;
        }
        public void SetColorBlindness(ColorBlindnessSetting colorBlindness)
        {
            ColorBlindness = colorBlindness;
        }

        public virtual void ApplySettings()
        {
            ApplyColorBlindness();
        }

        public void ApplyColorBlindness()
        {
            var renderer = (UnityEngine.Rendering.GraphicsSettings.currentRenderPipeline as UniversalRenderPipelineAsset).GetRenderer(0);
            var property = typeof(ScriptableRenderer).GetProperty(k_RendererFeaturesPropertyName, BindingFlags.NonPublic | BindingFlags.Instance);

            var features = property.GetValue(renderer) as List<ScriptableRendererFeature>;

            foreach (var feature in features)
            {
                if (feature is ColorBlindCorrectionFeature colorBlindCorrectionFeature)
                {
                    switch (GetColorBlindness())
                    {
                        case ColorBlindnessSetting.Off:
                            colorBlindCorrectionFeature.SetActive(false);
                            break;
                        case ColorBlindnessSetting.Deuteranopia:
                            colorBlindCorrectionFeature.BlindnessType = ColorBlindnessType.Deuteranopia;
                            colorBlindCorrectionFeature.SetActive(true);
                            colorBlindCorrectionFeature.Create();
                            break;
                        case ColorBlindnessSetting.Protanopia:
                            colorBlindCorrectionFeature.BlindnessType = ColorBlindnessType.Protanopia;
                            colorBlindCorrectionFeature.SetActive(true);
                            colorBlindCorrectionFeature.Create();
                            break;
                        case ColorBlindnessSetting.Tritanopia:
                            colorBlindCorrectionFeature.BlindnessType = ColorBlindnessType.Tritanopia;
                            colorBlindCorrectionFeature.SetActive(true);
                            colorBlindCorrectionFeature.Create();
                            break;
                    }
                }
            }
        }
    }

    public interface IAccessibilitySettings
    {
        public void ApplySettings();
    }

    [System.Serializable]
    public class SerializableAccesibilitySettings
    {
        public AccessibilitySettings.ColorBlindnessSetting ColorBlindness = AccessibilitySettings.ColorBlindnessSetting.Off;
        public bool ControllerVibration = true;

        public SerializableAccesibilitySettings()
        {

        }

        public SerializableAccesibilitySettings(AccessibilitySettings settings)
        {
            ColorBlindness = settings.GetColorBlindness();
            ControllerVibration = settings.GetVibration();
        }

        public virtual void Deserialize(AccessibilitySettings settings)
        {
            settings.SetColorBlindness(ColorBlindness);
            settings.SetVibration(ControllerVibration);
        }
    }
}